using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Glow : MonoBehaviour
{
    private Material material;
    public LocalKeyword GlowKeyword;

    // Start is called before the first frame update
    void Awake()
    {
        material = GetComponent<Renderer>().material;
        GlowKeyword = material.shader.keywordSpace.FindKeyword("_GLOW");
    }

    public void Activate()
    {
        //DebugConsole.Log("Called Activate in glow");
        if(GlowKeyword.isValid) material.EnableKeyword(GlowKeyword);
    }

    public void Stop()
    {
        if(GlowKeyword.isValid) material.DisableKeyword(GlowKeyword);
    }
}
