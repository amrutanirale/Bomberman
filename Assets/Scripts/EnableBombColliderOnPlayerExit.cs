using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBombColliderOnPlayerExit : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider>().isTrigger = false;
        }
    }
}
