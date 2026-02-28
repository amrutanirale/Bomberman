using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionPrefab;
    public LayerMask obstacleLayerMask;

    public int blastRadius = 1;
    public int placedByPlayer = 1;
    public bool isRCBomb;
    private bool exploded = false;
    // Tracks previous mobile RC-button state for rising-edge detection
    private bool prevMobileRCDown = false;

    // Use this for initialization
    private void Start()
    {
        if (!isRCBomb)
        {
            Invoke("BombExplosion", 5f);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isRCBomb) return;

        bool keyDown = (Input.GetKeyDown(KeyCode.LeftControl)  && placedByPlayer == 0)
                    || (Input.GetKeyDown(KeyCode.RightControl) && placedByPlayer == 1);

        // Rising-edge detection for the mobile RC button so a single tap detonates
        // exactly once, even though rcPressed stays true for multiple frames.
        bool curRC = MobileControls.Instance != null
                     && MobileControls.Instance.rcPressed[placedByPlayer];
        bool mobileRCJustPressed = curRC && !prevMobileRCDown;
        prevMobileRCDown = curRC;

        if (keyDown || mobileRCJustPressed)
            BombExplosion();
    }

    private void BombExplosion()
    {
        if (exploded) return;
        exploded = true;

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        StartCoroutine(CreateExplosions(Vector3.forward));
        StartCoroutine(CreateExplosions(Vector3.right));
        StartCoroutine(CreateExplosions(Vector3.back));
        StartCoroutine(CreateExplosions(Vector3.left));

        GetComponent<MeshRenderer>().enabled = false;
        transform.Find("Collider").gameObject.SetActive(false);
        GameManager.Instance.BombExploded(placedByPlayer, isRCBomb);
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!exploded && other.CompareTag("Explosion"))
        {
            CancelInvoke("BombExplosion");
            BombExplosion();
        }
    }

    private IEnumerator CreateExplosions(Vector3 direction)
    {
        for (int i = 1; i <= blastRadius; i++)
        {
            Physics.Raycast(transform.position, direction, out RaycastHit hit, i, obstacleLayerMask);

            if (hit.collider)
            {
                if (hit.collider.CompareTag("DestructibleWall"))
                {
                    Destroy(hit.collider.gameObject);
                    Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
                    i = blastRadius;
                    if (Random.value <= GameEventManager.spawnPickupProbability)
                    {
                        SpwanPickups.Instance.SpawnPickup(hit.collider.gameObject.transform.position);
                    }
                }
                else if (hit.collider.CompareTag("Pickups"))
                {
                    Destroy(hit.collider.gameObject);
                    Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
                }
                else if (hit.collider.CompareTag("AI"))
                {
                    Destroy(hit.collider.gameObject);
                    Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
                }
                else if (hit.collider.CompareTag("Player"))
                {
                    PlayerController pc = hit.collider.GetComponent<PlayerController>();
                    if (pc != null && !pc.isPlayerDead)
                    {
                        Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
                        GameManager.Instance.PlayerDead(pc.playerNumber);
                    }
                }
                else
                {
                    break;
                }
            }
            else
            {
                Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
