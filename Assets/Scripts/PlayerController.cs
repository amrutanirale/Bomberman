using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance = null;
    public float speed = 1;
    public GameObject bombPrefab;
    public bool isBombActive = false;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalMove = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float VerrticalMove = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        transform.position += new Vector3(horizontalMove, 0, VerrticalMove);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropBomb();
        }
    }

    public void DropBomb()
    {
        GameObject bomb;
        Vector3 bombPosition= new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, Mathf.RoundToInt(transform.position.z));
        bomb = Instantiate(bombPrefab, bombPosition, Quaternion.identity);
        isBombActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Explosion")
        {
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Speedboost")
        {
            Destroy(other.gameObject);
            speed +=1;
            Invoke("RegularSpeed", 5f);
        }
    }
    private void RegularSpeed()
    {
        speed -= 1f;
    }
}

