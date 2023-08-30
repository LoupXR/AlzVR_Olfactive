using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    private Grabbable grabbable;
    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<Grabbable>();
    }

    // Update is called once per frame
    void Update()
    {
        if(grabbable.IsGrabbed)
        {
            GameManager.Instance.End();
        }
    }


}
