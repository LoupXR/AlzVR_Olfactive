using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillingPan : MonoBehaviour
{
    [SerializeField] private GameObject onion;
    [SerializeField] private GameObject onionSlices;
    [SerializeField] private GameObject baconInBowl;
    [SerializeField] private GameObject bacon;

    [SerializeField] private Grabber rightHand;
    [SerializeField] private Grabber leftHand;

    private void OnCollisionEnter(Collision collision)
    {
        if (rightHand.grabbedObject == null && leftHand.grabbedObject == null)
        {
            if (collision.gameObject == onion)
            {
                if (rightHand.grabbables.Count > 0)
                {
                    rightHand.UnregisterGrabbable(rightHand.grabbables[0]);
                }
                if (leftHand.grabbables.Count > 0)
                {
                    leftHand.UnregisterGrabbable(leftHand.grabbables[0]);
                }
                onion.GetComponent<BasicGrabbable>().Release();
                onion.SetActive(false);
                onionSlices.SetActive(true);
            }
            if (collision.gameObject == baconInBowl)
            {
                if (rightHand.grabbables.Count > 0)
                {
                    rightHand.UnregisterGrabbable(rightHand.grabbables[0]);
                }
                if (leftHand.grabbables.Count > 0)
                {
                    leftHand.UnregisterGrabbable(leftHand.grabbables[0]);
                }
                baconInBowl.GetComponent<BasicGrabbable>().Release();
                baconInBowl.SetActive(false);
                bacon.SetActive(true);
            }
        }
    }
}
