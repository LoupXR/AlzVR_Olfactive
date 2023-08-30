using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.InputSystem.XR;

public class Grabber : MonoBehaviour
{
    public List<Grabbable> grabbables;
    public Grabbable grabbedObject;

    public Side hand;
    private XRControllerWithRumble controller
    {
        get{return hand == Side.Left ? (XRControllerWithRumble) XRController.leftHand : (XRControllerWithRumble) XRController.rightHand;}
    }

    // Start is called before the first frame update
    void Start()
    {
        grabbables = new List<Grabbable>();
        InputAction action = hand == Side.Left ? Player.Instance.controls.Main.GrabLeft : Player.Instance.controls.Main.GrabRight;
        action.performed += context => Grab();
        action.canceled += context => Release();
    }


    void Grab()
    {
        if(grabbables.Count > 0)
        {
            grabbedObject = grabbables[grabbables.Count-1];
            grabbedObject.Grab(this);
            controller?.SendImpulse(0.2f, 0.1f);
        }
    }
    public void Release()
    {
        grabbedObject?.Release();
        //Debug.Log("Released " + grabbedObject?.gameObject.name);
        grabbedObject = null;
    }

    public void RegisterGrabbable(Grabbable grabbable)
    {
        if(grabbables.Find(g => g == grabbable) == null)
        {
            if(grabbables.Count > 0) grabbables[grabbables.Count-1].Target(false);
            grabbables.Add(grabbable);
            grabbables[grabbables.Count-1].Target(true);
        }

    }
    public void UnregisterGrabbable(Grabbable grabbable)
    {
        if(grabbables.Count > 0)
        {
            int i = grabbables.FindIndex(g => g == grabbable);
            if (i == grabbables.Count-1) grabbables[i].Target(false);
            if (i >= 0) grabbables.RemoveAt(i);
            if(grabbables.Count > 0) grabbables[grabbables.Count-1].Target(true);
        }
    }

}
