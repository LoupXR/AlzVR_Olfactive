using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance{get; private set;}

    void Awake()
    {
        if(Instance != null) Destroy(this);
        Instance = this;

        controls = new Controls();
        controls.Main.Enable();
    }

    public Controls controls;
}
