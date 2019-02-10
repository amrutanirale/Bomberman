using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField]
    private float aiSpeed = 1;
    private CharacterController charController;

    private void Start()
    {
        charController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        charController.SimpleMove(forward * aiSpeed);
        //transform.position += transform.forward * aiSpeed * Time.deltaTime;
        if (Vector3.SqrMagnitude(charController.velocity) <= 0.1f)
        {

            transform.rotation = Quaternion.LookRotation(CheckCollision());

        }
    }
    private Vector3 CheckCollision()
    {
        Vector3 direction = Vector3.forward;

        transform.position += transform.forward * aiSpeed * Time.deltaTime;
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.back, out hit);
        if (!hit.collider.CompareTag("Player"))
        {
            if (hit.distance > 1)
            {
                direction = Vector3.back;
                return direction;
            }
        }
        Physics.Raycast(transform.position, Vector3.left, out hit);
        if (!hit.collider.CompareTag("Player"))
        {
            if (hit.distance > 1)
            {
                direction = Vector3.left;
                return direction;
            }
        }
        Physics.Raycast(transform.position, Vector3.right, out hit);
        if (!hit.collider.CompareTag("Player"))
        {
            if (hit.distance > 1)
            {
                direction = Vector3.right;
                return direction;
            }
        }
        Physics.Raycast(transform.position, Vector3.forward, out hit);
        if (!hit.collider.CompareTag("Player"))
        {
            if (hit.distance > 1)
            {
                direction = Vector3.forward;
                return direction;
            }
        }


        return direction;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
        {
            return;
        }

        GameManager.Instance.PlayerDead(body.gameObject.GetComponent<PlayerController>().playerNumber);
    }
}