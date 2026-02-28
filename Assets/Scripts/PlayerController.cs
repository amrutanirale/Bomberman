using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1;
    public int playerNumber = 1;
    public bool canDropBombs = true;
    public bool isPlayerDead = false;

    private Rigidbody rb;
    private Transform myTransform;

    // Tracks the previous frame's mobile bomb-button state so we fire only on
    // the rising edge (PointerDown → true for the first time), matching the
    // behaviour of Input.GetKeyDown.
    private bool prevMobileBombDown = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        myTransform = transform;
    }

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
        // Read mobile directional input (zero when MobileControls not present)
        Vector2 mobile = MobileControls.Instance != null
            ? MobileControls.Instance.moveInput[playerNumber]
            : Vector2.zero;

        if (Input.GetKey(KeyCode.W) || mobile.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.A) || mobile.x < 0)
        {
            rb.velocity = new Vector3(-moveSpeed, rb.velocity.y, rb.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 270, 0);
        }
        if (Input.GetKey(KeyCode.S) || mobile.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetKey(KeyCode.D) || mobile.x > 0)
        {
            rb.velocity = new Vector3(moveSpeed, rb.velocity.y, rb.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 90, 0);
        }

        // Rising-edge detection: only trigger bomb on the frame the button goes down
        bool curBomb = MobileControls.Instance != null && MobileControls.Instance.bombPressed[playerNumber];
        bool mobileBombJustPressed = curBomb && !prevMobileBombDown;
        prevMobileBombDown = curBomb;

        if ((Input.GetKeyDown(KeyCode.Space) || mobileBombJustPressed) && canDropBombs)
            GameManager.Instance.Dropbomb(playerNumber, myTransform.position);
    }

    public void Player2Movement()
    {
        Vector2 mobile = MobileControls.Instance != null
            ? MobileControls.Instance.moveInput[playerNumber]
            : Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow) || mobile.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || mobile.x < 0)
        {
            rb.velocity = new Vector3(-moveSpeed, rb.velocity.y, rb.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 270, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow) || mobile.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow) || mobile.x > 0)
        {
            rb.velocity = new Vector3(moveSpeed, rb.velocity.y, rb.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 90, 0);
        }

        bool curBomb = MobileControls.Instance != null && MobileControls.Instance.bombPressed[playerNumber];
        bool mobileBombJustPressed = curBomb && !prevMobileBombDown;
        prevMobileBombDown = curBomb;

        if ((Input.GetKeyDown(KeyCode.RightShift) || mobileBombJustPressed) && canDropBombs)
            GameManager.Instance.Dropbomb(playerNumber, myTransform.position);
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
