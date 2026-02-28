using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfBompExplosion : MonoBehaviour
{
    public float bombTimer=2f;

    void Start()
    {
        Destroy(gameObject, bombTimer);
    }
}
