 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Grabbable
{
    [Tooltip("The child that will move")]
    public Transform doorObject;

    //The direction that will follow the hand in the door object's space
    private Vector3 forward;
    [Tooltip("The direction of the axis of rotation in the door object's space")]
    public Vector3 axis = Vector3.up;
    [Tooltip("Signed left handed min angle of the door around the axis, relative to this object")]
    public float minAngle = 0;
    [Tooltip("Signed left handed max angle of the door around the axis, relative to this object")]
    public float maxAngle = 120;
    
    public override void Grab(Grabber grabber)
    {
        base.Grab(grabber);
        Vector3 relativePosition = doorObject.InverseTransformPoint(hand.position);
        forward = relativePosition - Vector3.Dot(relativePosition, axis.normalized) * axis.normalized;
    }

    protected override void FollowHand()
    {
        PointAt(hand.position);
    }

    //Turns the door so that the forward vector points at position, within the constraints.
    private void PointAt(Vector3 position)
    {
        Vector3 worldAxis = doorObject.TransformDirection(axis);
        Vector3 direction = position-transform.position;
        direction = direction - Vector3.Dot(direction, worldAxis.normalized) * worldAxis.normalized;
        Quaternion baseRotation = Quaternion.Inverse(Quaternion.LookRotation(forward, axis));
        if(direction.magnitude > 0.05f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, worldAxis);
            doorObject.rotation = rotation * baseRotation;
            if(Vector3.SignedAngle(transform.TransformDirection(forward), doorObject.TransformDirection(forward), worldAxis) < minAngle)
            {
                doorObject.localRotation = Quaternion.AngleAxis(minAngle, axis);
            }
            else if(Vector3.SignedAngle(transform.TransformDirection(forward), doorObject.TransformDirection(forward), worldAxis) > maxAngle)
            {
                doorObject.localRotation = Quaternion.AngleAxis(maxAngle, axis);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("CollisionStay " + gameObject.name);
        Debug.Log("isGrabbed: " + IsGrabbed);
        if(!IsGrabbed)
        {
            ContactPoint contact = collision.contacts[0];
            if(contact.separation < 0)
            {
                Vector3 relativePosition = doorObject.InverseTransformPoint(contact.point);
                forward = relativePosition - Vector3.Dot(relativePosition, axis.normalized) * axis.normalized;
                PointAt(contact.point - contact.normal * contact.separation);
            }
        }
    }
}
