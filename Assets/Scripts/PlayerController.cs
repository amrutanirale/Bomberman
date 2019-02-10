using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance = null;
    public float moveSpeed = 1;
    public GameObject bombPrefab;
    public bool isBombActive = false;
    public int playerNumber = 1;
    private new Rigidbody rigidbody;
    private Transform myTransform;
    public bool canDropBombs = true;
    public bool isPlayerDead = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        myTransform = transform;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.Instance.isGameStarted == true)
        {
            if (playerNumber == 0)
            {
                Player1Movement();
            }
            else
            {
                Player2Movement();
            }
        }

    }
    public void Player1Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.velocity = new Vector3(-moveSpeed, rigidbody.velocity.y, rigidbody.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 270, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigidbody.velocity = new Vector3(moveSpeed, rigidbody.velocity.y, rigidbody.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.Dropbomb(playerNumber, myTransform.position);
        }
    }

    public void Player2Movement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigidbody.velocity = new Vector3(-moveSpeed, rigidbody.velocity.y, rigidbody.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 270, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rigidbody.velocity = new Vector3(moveSpeed, rigidbody.velocity.y, rigidbody.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            GameManager.Instance.Dropbomb(playerNumber, myTransform.position);
        }
    }
    //public void DropBomb()
    //{
    //    GameObject bomb;
    //    Vector3 bombPosition= new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y-0.2f, Mathf.RoundToInt(transform.position.z));
    //    bomb = Instantiate(bombPrefab, bombPosition, Quaternion.identity);
    //    isBombActive = true;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickups"))
        {
            string pickupName = other.gameObject.name;

            switch (pickupName)
            {
                case "LongBlast":
                    GameManager.Instance.PlayerPickedupPowerUP(PickupTypes.LongBlast, playerNumber);
                    break;
                case "MoreBombs":
                    GameManager.Instance.PlayerPickedupPowerUP(PickupTypes.MoreBombs, playerNumber);
                    break;
                case "SpeedBoost":
                    GameManager.Instance.PlayerPickedupPowerUP(PickupTypes.SpeedBoost, playerNumber);
                    break;
                case "RCBomb":
                    GameManager.Instance.PlayerPickedupPowerUP(PickupTypes.RCBomb, playerNumber);
                    break;
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Explosion") || other.CompareTag("AI"))
        {
            GameManager.Instance.PlayerDead(playerNumber);
            isPlayerDead = true;
        }
    }
    private void RegularSpeed()
    {
        moveSpeed -= 1f;
    }
}

