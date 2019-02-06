using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public static Bomb Instance = null;
    public GameObject explosionPrefab;
    public LayerMask obstacleLayerMask;

    public int blastRadius = 1;
    public int placedByPlayer = 1;
    public bool isRCBomb;
    private bool exploded = false;

    private void Awake()
    {
        Instance = this;
    }

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
        if (isRCBomb)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) && placedByPlayer == 0)
            {
                BombExplosion();
            }
            if (Input.GetKeyDown(KeyCode.RightControl) && placedByPlayer == 1)
            {
                BombExplosion();
            }
        }
    }

    private void BombExplosion()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        StartCoroutine(CreateExplosions(Vector3.forward));
        StartCoroutine(CreateExplosions(Vector3.right));
        StartCoroutine(CreateExplosions(Vector3.back));
        StartCoroutine(CreateExplosions(Vector3.left));

        GetComponent<MeshRenderer>().enabled = false;
        transform.Find("Collider").gameObject.SetActive(false);
        GameManager.Instance.BombExploded(placedByPlayer, isRCBomb);
        Destroy(gameObject, 0.5f);
        PlayerController.Instance.isBombActive = false;
        exploded = true;
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
                if (hit.collider.tag == "DestructibleWall")
                {
                    Destroy(hit.collider.gameObject);
                    Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
                    i = blastRadius;
                    if (Random.value <= 0.3)
                    {
                        SpwanPickups.Instance.SpawnPickup(hit.collider.gameObject.transform.position);
                    }
                }
                else if (hit.collider.tag == "Pickups")
                {
                    Destroy(hit.collider.gameObject);
                    Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);

                }
                else if (hit.collider.tag == "AI")
                {
                    Destroy(hit.collider.gameObject);
                    Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
                }
                else if (hit.collider.tag == "Player")
                {
                    Destroy(hit.collider.gameObject);
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
