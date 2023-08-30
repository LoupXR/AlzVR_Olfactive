using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : Grabbable
{
    [Tooltip("The actual moving part of the drawer")]
    public Transform drawerObject;

    [Tooltip("Direction in which the drawer opens in local space")]
    public Vector3 openingDirection = Vector3.right;
    public float maxOpening = 0.66f;
    public float minOpening = 0f;

    protected override void FollowHand()
    {
        float distance = Vector3.Dot(transform.InverseTransformPoint(hand.position), openingDirection.normalized);
        float opening;
        if(distance > maxOpening)
        {
            opening = maxOpening;
        }
        else if(distance < minOpening)
        {
            opening = minOpening;
        }
        else
        {
            opening = distance;
        }
        drawerObject.localPosition = opening * openingDirection.normalized;
    }
}
