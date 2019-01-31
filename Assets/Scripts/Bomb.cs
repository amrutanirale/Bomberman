using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosion;
    public LayerMask obstacleLayerMask;
    private bool exploded = false;
    // Use this for initialization
    void Start()
    {
        Invoke("BombExplosion", 5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void BombExplosion()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);

        StartCoroutine(CreateExplosions(Vector3.forward));
        StartCoroutine(CreateExplosions(Vector3.right));
        StartCoroutine(CreateExplosions(Vector3.back));
        StartCoroutine(CreateExplosions(Vector3.left));

        GetComponent<MeshRenderer>().enabled = false;
        //transform.Find("Collider").gameObject.SetActive(false);
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
        if (other.CompareTag("BrickWall"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
    private IEnumerator CreateExplosions(Vector3 direction)
    {
        for (int i = 1; i < 3; i++)
        {
            Physics.Raycast(transform.position , direction, out RaycastHit hit, i,obstacleLayerMask);

            if (!hit.collider)
            {
                Instantiate(explosion, transform.position + (i * direction), explosion.transform.rotation);
            }
            else
            if(hit.collider.tag!="HardWall")
            {
                Destroy(hit.collider.gameObject);
                
            }
            
            else
            {
                break;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}
