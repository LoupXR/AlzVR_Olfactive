using UnityEngine;

// ReSharper disable once CheckNamespace
public class BasicGrabbable : Grabbable
{
    [Tooltip("The place at which the object will be grabbed. Must be a child of the object.")]
    public Transform handle;
    [Tooltip("Should the handle be aligned with the hand when grabbed? If false, the position in hand will depend on how it was grabbed.")]
    public bool alignHandleWithHand;
    public bool enablePhysicsOnRelease = true;
    private Quaternion _relativeRotation;
    private Vector3 _relativePosition;
    private Rigidbody _rigidbody;
    private Vector3 _previousPosition;
    private Quaternion _previousRotation;

    protected override void Start()
    {
        base.Start();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // ReSharper disable once ParameterHidesMember
    public override void Grab(Grabber grabber)
    {
        base.Grab(grabber);
        _rigidbody.useGravity = false;

        if(handle != null && alignHandleWithHand)
        {
            _relativeRotation = handle.localRotation;
        }
        else
        {
            _relativeRotation = Quaternion.Inverse(transform.rotation) * hand.rotation;
        }
        _relativePosition = handle != null ? handle.localPosition : transform.InverseTransformPoint(hand.position);
    }

    public override void Release()
    {
        base.Release();
        if(enablePhysicsOnRelease)
        {
            var transform1 = transform;
            _rigidbody.velocity = (transform1.position - _previousPosition)/Time.deltaTime;
            _rigidbody.angularVelocity = (transform1.rotation * Quaternion.Inverse(_previousRotation)).eulerAngles / Time.deltaTime;
            _rigidbody.useGravity = true;
        }
        
    }

    //make the handle follow the hand
    protected override void FollowHand()
    {
        var transform1 = transform;
        
        _previousPosition = transform1.position;
        _previousRotation = transform1.rotation;
        transform1.rotation = hand.rotation * Quaternion.Inverse(_relativeRotation);
        transform1.position = hand.position - transform.TransformVector(_relativePosition);
    }
}
