using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Creates a split-screen on-screen control overlay for two players.
///
/// Layout (landscape):
///   P1 (left half)                     P2 (right half)
///   [↑]              |  HUD  |              [↑]
///   [←][↓][→] [B][R] |       | [R][B] [←][↓][→]
///
/// Other scripts read the public arrays each frame:
///   moveInput[p]   – current direction (Vector2, one of the 4 cardinals or zero)
///   bombPressed[p] – true while the BOMB button is held
///   rcPressed[p]   – true while the RC button is held
/// </summary>
public class MobileControls : MonoBehaviour
{
    public static MobileControls Instance;

    // Input state – read by PlayerController (move/bomb) and Bomb (rc)
    public Vector2[] moveInput  = new Vector2[2];
    public bool[]    bombPressed = new bool[2];
    public bool[]    rcPressed   = new bool[2];

    // Visual constants (in reference-resolution units: 1920 × 1080)
    private const float BTN      = 110f;   // standard button size
    private const float BOMB_BTN = 135f;   // BOMB button is larger (easier to tap)
    private const float GAP      = 14f;    // gap between adjacent buttons
    private const float MARGIN   = 35f;    // screen-edge margin

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BuildCanvas();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // -------------------------------------------------------------------------
    // Canvas construction
    // -------------------------------------------------------------------------

    private void BuildCanvas()
    {
        // Ensure an EventSystem exists so UI buttons respond to touch/mouse
        if (FindObjectOfType<EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();
        }

        var canvasGO = new GameObject("MobileControlsCanvas");

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;   // always on top

        // Scale buttons proportionally on all screen sizes / densities
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Player 1 – left side
        BuildPlayerControls(canvas.transform, playerIndex: 0, fromLeft: true);
        // Player 2 – right side (mirrored)
        BuildPlayerControls(canvas.transform, playerIndex: 1, fromLeft: false);
    }

    /// <summary>
    /// Builds the D-pad + BOMB + RC buttons for one player.
    /// fromLeft=true  → anchored to bottom-left corner
    /// fromLeft=false → anchored to bottom-right corner (mirrored)
    /// </summary>
    private void BuildPlayerControls(Transform parent, int playerIndex, bool fromLeft)
    {
        // Anchor corner and horizontal direction sign
        Vector2 anchor = fromLeft ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
        float   s      = fromLeft ? 1f : -1f;   // sign flips X for right side

        // D-pad centre position (in reference-resolution space relative to anchor)
        //   Horizontally: one button-width + gap + margin from the edge
        //   Vertically:   centre row of D-pad sits one button + gap above margin
        float dCX = s * (MARGIN + BTN + GAP);
        float dCY = MARGIN + BTN + GAP;

        Color dpadColor = new Color(1f, 1f, 1f, 0.30f);
        Color bombColor = new Color(1f, 0.55f, 0.05f, 0.80f);
        Color rcColor   = new Color(0.35f, 0.65f, 1f,  0.75f);

        // ── D-Pad ────────────────────────────────────────────────────────────
        MakeBtn(parent, "↑", anchor,
            new Vector2(dCX, dCY + BTN + GAP), BTN, dpadColor,
            () => moveInput[playerIndex] = Vector2.up,
            () => { if (moveInput[playerIndex] == Vector2.up)   moveInput[playerIndex] = Vector2.zero; });

        MakeBtn(parent, "↓", anchor,
            new Vector2(dCX, dCY - BTN - GAP), BTN, dpadColor,
            () => moveInput[playerIndex] = Vector2.down,
            () => { if (moveInput[playerIndex] == Vector2.down) moveInput[playerIndex] = Vector2.zero; });

        MakeBtn(parent, "←", anchor,
            new Vector2(dCX - (BTN + GAP), dCY), BTN, dpadColor,
            () => moveInput[playerIndex] = Vector2.left,
            () => { if (moveInput[playerIndex] == Vector2.left) moveInput[playerIndex] = Vector2.zero; });

        MakeBtn(parent, "→", anchor,
            new Vector2(dCX + (BTN + GAP), dCY), BTN, dpadColor,
            () => moveInput[playerIndex] = Vector2.right,
            () => { if (moveInput[playerIndex] == Vector2.right) moveInput[playerIndex] = Vector2.zero; });

        // ── Action buttons ───────────────────────────────────────────────────
        // Placed to the inner side of the D-pad so each player's thumb can reach
        float aX = s * (MARGIN + (BTN + GAP) * 3.8f);

        MakeBtn(parent, "BOMB", anchor,
            new Vector2(aX, dCY - (BOMB_BTN - BTN) * 0.5f), BOMB_BTN, bombColor,
            () => bombPressed[playerIndex] = true,
            () => bombPressed[playerIndex] = false);

        MakeBtn(parent, "RC", anchor,
            new Vector2(aX, dCY + BOMB_BTN * 0.5f + GAP * 2f), BTN, rcColor,
            () => rcPressed[playerIndex] = true,
            () => rcPressed[playerIndex] = false);
    }

    // -------------------------------------------------------------------------
    // Button factory
    // -------------------------------------------------------------------------

    private void MakeBtn(Transform parent, string label, Vector2 anchor,
                         Vector2 anchoredPos, float size, Color color,
                         Action onDown, Action onUp)
    {
        var go = new GameObject("MBtn_" + label);
        go.transform.SetParent(parent, false);

        var rt            = go.AddComponent<RectTransform>();
        rt.anchorMin      = anchor;
        rt.anchorMax      = anchor;
        rt.pivot          = new Vector2(0.5f, 0.5f);
        rt.sizeDelta      = new Vector2(size, size);
        rt.anchoredPosition = anchoredPos;

        var img   = go.AddComponent<Image>();
        img.color = color;

        AddLabel(go.transform, label, Mathf.RoundToInt(size * 0.30f));

        var et = go.AddComponent<EventTrigger>();
        AddEvt(et, EventTriggerType.PointerDown, onDown);
        AddEvt(et, EventTriggerType.PointerUp,   onUp);
        // Cancel if the finger slides off the button
        AddEvt(et, EventTriggerType.PointerExit, onUp);
    }

    private static void AddLabel(Transform parent, string text, int fontSize)
    {
        var go = new GameObject("Lbl");
        go.transform.SetParent(parent, false);

        var rt         = go.AddComponent<RectTransform>();
        rt.anchorMin   = Vector2.zero;
        rt.anchorMax   = Vector2.one;
        rt.offsetMin   = rt.offsetMax = Vector2.zero;

        var t          = go.AddComponent<Text>();
        t.text         = text;
        t.font         = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.fontSize     = fontSize;
        t.fontStyle    = FontStyle.Bold;
        t.alignment    = TextAnchor.MiddleCenter;
        t.color        = Color.white;
    }

    private static void AddEvt(EventTrigger et, EventTriggerType type, Action cb)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(_ => cb());
        et.triggers.Add(entry);
    }
}
