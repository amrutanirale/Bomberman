using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    private int numberOfPlayersInGame = 2;
    public Pickups[] e_playerPickups;
    public GameObject[] playerGo;
    public GameObject playerPrefab, bombPrefab;
    private float[] f_RCBombTimer;
    private int[] i_tempBombAmount;
    private float f_levelTime;
    private bool isGameStarted = false;
    private bool isGameOver = false;
    private int deadPlayers = 0;
    private int deadPlayerNumber = -1;
    public Text levelTimerText, winingStatusText;
    public GameObject gameOverPanel;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {

        InitializeVariables();
        SpawnPlayers();
    }

    public void StartGame()
    {
        isGameStarted = true;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
    private void InitializeVariables()
    {
        playerGo = new GameObject[numberOfPlayersInGame];
        e_playerPickups = new Pickups[numberOfPlayersInGame];
        i_tempBombAmount = new int[numberOfPlayersInGame];
        f_RCBombTimer = new float[numberOfPlayersInGame];
        //Initialize player default pickups
        for (int i = 0; i < numberOfPlayersInGame; i++)
        {
            e_playerPickups[i] = new Pickups(1, 1, 1, false);
            i_tempBombAmount[i] = 1;
        }
        f_levelTime = GameEventManager.levelTime;
    }

    private void SpawnPlayers()
    {
        playerGo[0] = Instantiate(playerPrefab, new Vector3(0, 0.3f, 0), Quaternion.identity);
        playerGo[0].GetComponent<PlayerController>().playerNumber = 0;
        playerGo[0].name = "Player" + 0;

        playerGo[1] = Instantiate(playerPrefab, new Vector3(MapGeneration.Instance.row - 1, 0.3f, MapGeneration.Instance.column - 1), Quaternion.identity);
        playerGo[1].GetComponent<PlayerController>().playerNumber = 1;
        playerGo[1].name = "Player" + 1;
    }

    public void Dropbomb(int playerNumber, Vector3 playerPosition)
    {
        if (e_playerPickups[playerNumber].isRCBombActive)
        {
            if (i_tempBombAmount[playerNumber] >= 1)
            {
                playerGo[playerNumber].GetComponent<PlayerController>().canDropBombs = false;
                StartCoroutine(PlayerCanPlaceBombs(playerNumber));
                GameObject bomb = Instantiate(bombPrefab, new Vector3(Mathf.RoundToInt(playerPosition.x),
                                              bombPrefab.transform.position.y, Mathf.RoundToInt(playerPosition.z)), bombPrefab.transform.rotation);
                bomb.GetComponent<Bomb>().placedByPlayer = playerNumber;
                bomb.GetComponent<Bomb>().isRCBomb = true;
                bomb.GetComponent<Bomb>().blastRadius = e_playerPickups[playerNumber].bombBlastRadius;
                bomb.GetComponent<Renderer>().material.color = Color.green;
                bomb.name = "Bomb";
                i_tempBombAmount[playerNumber]--;
                if (i_tempBombAmount[playerNumber] < 0)
                {
                    i_tempBombAmount[playerNumber] = 0;
                }
            }
        }
        else
        {
            if (e_playerPickups[playerNumber].bombAmount >= 1)
            {
                playerGo[playerNumber].GetComponent<PlayerController>().canDropBombs = false;
                StartCoroutine(PlayerCanPlaceBombs(playerNumber));
                GameObject bomb = Instantiate(bombPrefab, new Vector3(Mathf.RoundToInt(playerPosition.x), Mathf.RoundToInt(playerPosition.y),
                    Mathf.RoundToInt(playerPosition.z)), Quaternion.identity);
                bomb.GetComponent<Bomb>().placedByPlayer = playerNumber;
                bomb.GetComponent<Bomb>().blastRadius = e_playerPickups[playerNumber].bombBlastRadius;
                bomb.gameObject.name = "Bomb";
                e_playerPickups[playerNumber].bombAmount--;
            }
        }
    }

    private IEnumerator PlayerCanPlaceBombs(int playerNumber)
    {
        yield return new WaitForSeconds(3);
        playerGo[playerNumber].GetComponent<PlayerController>().canDropBombs = true;
    }

    public void BombExploded(int playerNumber, bool isRCBomb) // Bomb exploaded incrementing the counter
    {
        if (isRCBomb)
        {
            i_tempBombAmount[playerNumber]++;
            if (i_tempBombAmount[playerNumber] > 1)
            {
                i_tempBombAmount[playerNumber] = 1;
            }
        }
        else
        {
            e_playerPickups[playerNumber].bombAmount++;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (isGameStarted == true)
        {
            f_levelTime -= Time.deltaTime;
            string minutes = Mathf.Floor(f_levelTime / 60).ToString("00");
            string seconds = Mathf.RoundToInt(f_levelTime % 60).ToString("00");



            levelTimerText.text = "Timer : " + minutes + ":" + seconds;

            //print(f_levelTime);
            if (f_levelTime <= 0)
            {
                isGameOver = true;
                levelTimerText.text = "Level End";
                Invoke("CheckPlayerDeath", 1);
            }
        }
    }

    private void LateUpdate()
    {

        // Updating the players score to UI elements
        for (int i = 0; i < numberOfPlayersInGame; i++)
        {
            if (e_playerPickups[i].isRCBombActive == true)
            {
                f_RCBombTimer[i] -= Time.deltaTime;
                if (f_RCBombTimer[i] <= 0)
                {
                    e_playerPickups[i].isRCBombActive = false;
                    f_RCBombTimer[i] = 10;                  // rcBombtimer
                }
            }
        }
    }
    public void PlayerPickedupPowerUP(PickupTypes pickup, int playerNumber)
    {
        switch (pickup)
        {
            case PickupTypes.LongBlast:
                e_playerPickups[playerNumber].bombBlastRadius++;
                print("LongBlast");
                break;
            case PickupTypes.MoreBombs:
                e_playerPickups[playerNumber].bombAmount++;
                print("MoreBombs");
                break;
            case PickupTypes.RCBomb:
                e_playerPickups[playerNumber].isRCBombActive = true;
                print("RCBomb");
                break;
            case PickupTypes.SpeedBoost:
                e_playerPickups[playerNumber].playerSpeed++;
                playerGo[playerNumber].GetComponent<PlayerController>().moveSpeed += 1;
                print("SpeedBoost");
                break;
            default:
                break;
        }
    }

    public void PlayerDead(int playerNumber)
    {
        isGameOver = true;
        playerGo[playerNumber].SetActive(false);
        deadPlayers++;
        if (deadPlayers == 1)
        {
            deadPlayerNumber = playerNumber;
            Invoke("CheckPlayerDeath", 1);
        }
    }

    public void CheckPlayerDeath()
    {

        gameOverPanel.gameObject.SetActive(true);
        if (deadPlayers == 1)
        {
            if (deadPlayerNumber == 1)
            {
                winingStatusText.text = "player 1 wins";
            }
            else
            {
                winingStatusText.text = "player 2 wins";
            }
        }
        else
        {
            winingStatusText.text = "Match Draw";
        }
    }
}
public class Pickups
{
    public int bombAmount;
    public int bombBlastRadius;
    public float playerSpeed;
    public bool isRCBombActive;

    public Pickups(int p_bombAmount, int p_bombRadius, float p_playerSpeed, bool p_isRCBombActive)
    {
        bombAmount = p_bombAmount;
        bombBlastRadius = p_bombRadius;
        playerSpeed = p_playerSpeed;
        isRCBombActive = p_isRCBombActive;
    }

    public Pickups()
    {
        bombAmount = 1;
        bombBlastRadius = 1;
        playerSpeed = 1;
        isRCBombActive = false;
    }
}
