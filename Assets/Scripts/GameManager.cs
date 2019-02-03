using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    int numberOfPlayersInGame    = 2;
    public Pickups[] e_playerPickups;
    public GameObject[] playerGo;
    public GameObject playerPrefab, bombPrefab;
    float[] f_RCBombTimer;
    int[] i_tempBombAmount;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeVariables();
        SpawnPlayers();
    }

    void InitializeVariables()
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
    }

    void SpawnPlayers()
    {
        playerGo[0] = Instantiate(playerPrefab, new Vector3(0, 0.3f, 1), Quaternion.identity);
        playerGo[0].GetComponent<PlayerController>().playerNumber = 0;
        playerGo[0].name = "Player" + 0;

        playerGo[1] = Instantiate(playerPrefab, new Vector3(0, 0.3f, -1), Quaternion.identity);
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
                GameObject bomb = Instantiate(bombPrefab,new Vector3(Mathf.RoundToInt( playerPosition.x),Mathf.RoundToInt(playerPosition.y),
                    Mathf.RoundToInt(playerPosition.z)), Quaternion.identity);
                bomb.GetComponent<Bomb>().placedByPlayer = playerNumber;
                bomb.GetComponent<Bomb>().blastRadius = e_playerPickups[playerNumber].bombBlastRadius;
                bomb.gameObject.name = "Bomb";
                e_playerPickups[playerNumber].bombAmount--;
            }
        }
    }

    IEnumerator PlayerCanPlaceBombs(int playerNumber)
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
    void Update()
    {
        
    }
    void LateUpdate()
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
    public void PlayerPickedupPowerUP(PickupTypes pickup,int playerNumber)
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
