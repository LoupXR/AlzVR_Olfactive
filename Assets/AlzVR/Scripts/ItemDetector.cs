using System;
using Unity.Mathematics;
using UnityEngine;

public class ItemDetector : MonoBehaviour {
    [SerializeField] private GameObject expectedGameObject;

    [Header("Debug utilities")]
    [SerializeField] private bool isDebugging;
    
    private void OnTriggerEnter(Collider other) {
        if (isDebugging) Debug.Log("OnTriggerEnter triggered by" + other.gameObject.name);
        
        if (other.gameObject != expectedGameObject) return;
        
        // Does it have the right orientation? Is it rightside-up?
        // quaternion rightOrientation = quaternion.identity; 
        // if (other.gameObject.transform.rotation - rightOrientation) 

        Vector3 boxColliderPosition = gameObject.GetComponent<BoxCollider>().transform.position;
        other.gameObject.transform.position = boxColliderPosition + 3*Vector3.up;
    }
    private void OnTriggerStay(Collider other) {
        if (isDebugging) Debug.Log("OnTriggerStay triggered by " + other.gameObject.name);
    }
    
    private void OnTriggerExit(Collider other) {
        if (isDebugging) Debug.Log("OnTriggerExit triggered by " + other.gameObject.name);
    }
}
