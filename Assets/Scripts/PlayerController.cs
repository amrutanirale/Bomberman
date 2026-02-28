using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1;
    public int playerNumber = 1;
    public bool canDropBombs = true;
    public bool isPlayerDead = false;

    private Rigidbody rb;
    private Transform myTransform;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        myTransform = transform;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isPlayerDead || !GameManager.Instance.isGameStarted)
            return;

        if (playerNumber == 0)
            Player1Movement();
        else
            Player2Movement();
    }

    public void Player1Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector3(-moveSpeed, rb.velocity.y, rb.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 270, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector3(moveSpeed, rb.velocity.y, rb.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space) && canDropBombs)
        {
            GameManager.Instance.Dropbomb(playerNumber, myTransform.position);
        }
    }

    public void Player2Movement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector3(-moveSpeed, rb.velocity.y, rb.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 270, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity = new Vector3(moveSpeed, rb.velocity.y, rb.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.RightShift) && canDropBombs)
        {
            GameManager.Instance.Dropbomb(playerNumber, myTransform.position);
        }
    }

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
        if (!isPlayerDead && (other.CompareTag("Explosion") || other.CompareTag("AI")))
        {
            isPlayerDead = true;
            GameManager.Instance.PlayerDead(playerNumber);
        }
    }
}
