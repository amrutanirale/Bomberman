using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [SerializeField]
    private float aiSpeed = 1;
    private NavMeshAgent navMesh;

    private void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        transform.position += transform.forward * aiSpeed * Time.deltaTime;

        RaycastHit hit;

        Physics.Raycast(transform.position, transform.forward, out hit);
        if (hit.collider)
        {
            if (hit.distance > 1)
            {
                transform.rotation = Quaternion.LookRotation(transform.forward);
                transform.Rotate(new Vector3(0, 90, 0));
            }
        }


    }


}