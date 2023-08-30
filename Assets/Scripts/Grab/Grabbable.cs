using UnityEngine;

//Only works if the gameObject has a rigidBody
// ReSharper disable once CheckNamespace
public abstract class Grabbable : MonoBehaviour
{
    //the hand currently holding the handle if any
    protected Transform hand;

    private Grabber _grabber;
    public bool IsGrabbed {get; private set;}

    public Glow glow;
    private int _grabberCount = 0;
    private bool _overrideGlow;

    /*[SerializeField] protected GameObject pull;
    [SerializeField] protected Material objectMaterial;
    [SerializeField] protected Material glowingMaterial; */


    // Update is called once per frame

    protected virtual void Start()
    {
        if(glow == null)glow = GetComponent<Glow>();
        if(glow == null) glow = GetComponentInChildren<Glow>();
    }
    void Update()
    {
        if(_grabber != null)
        {
            FollowHand();
        }
    }

    public void OverrideGlow(bool value)
    {
        _overrideGlow = value;
        if(!value) Glow(_grabberCount > 1 || (!IsGrabbed && _grabberCount > 0));
    }

    public virtual void Grab(Grabber grabber)
    {
        IsGrabbed = true;
        _grabber?.Release();
        hand = grabber.transform;
        _grabber = grabber;
        if(!_overrideGlow) Glow(false);
    }

    public virtual void Release()
    {
        IsGrabbed = false;
        _grabber = null;
        hand = null;
        if(!_overrideGlow) Glow(_grabberCount > 1 || (!IsGrabbed && _grabberCount > 0));
    }

    protected virtual void FollowHand()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Grabber grabber = other.GetComponent<Grabber>();
        if(grabber != null)
        {
            grabber.RegisterGrabbable(this);
        }
    }
    void OnTriggerExit(Collider other)
    {
        Grabber otherGrabber = other.GetComponent<Grabber>();
        if(otherGrabber != null)
        {
            otherGrabber.UnregisterGrabbable(this);
        }
    }
    private void Glow(bool value)
    {
        if(value) glow?.Activate();
        else glow?.Stop();
    }
    public void Target(bool value)
    {
        _grabberCount += value ? 1 : -1;
        if(!_overrideGlow) Glow(_grabberCount > 1 || (!IsGrabbed && _grabberCount > 0));
    }
}
