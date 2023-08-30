using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lid : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Fire")
        {
            other.GetComponent<Fire>()?.StopFire();
        }
    }
}
