using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfBompExplosion : MonoBehaviour
{
    public float bombTimer=2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, bombTimer);    
    }
}
