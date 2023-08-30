using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TidyingTask : MonoBehaviour
{
    public GameObject target;
    private new Renderer renderer;
    private Grabbable grabbable;
    private Glow selfGlow;
    private Glow[] targetGlows;
    private Color[] originalTargetColors;
    public AudioSource playerAudioSource;
    public bool complete {get; private set;}
    public bool loopTask = true;

    // Start is called before the first frame update
    void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        selfGlow = GetComponent<Glow>();
        if(selfGlow == null) selfGlow = GetComponentInChildren<Glow>();
        targetGlows = target.GetComponentsInChildren<Glow>();
        renderer = selfGlow.GetComponent<Renderer>();
    }


    public void StartTask()
    {
        StartCoroutine(Task());
    }

    private IEnumerator Task()
    {
        grabbable.OverrideGlow(true);
        Color originalColor = renderer.material.GetColor("_GlowColor");
        renderer.material.SetColor("_GlowColor", Color.magenta);
        ChangeTargetColor(false);
        while(!complete)
        {
            IEnumerator selfFlicker = FlickerSelf();
            StartCoroutine(selfFlicker);
            yield return new WaitUntil(()=>grabbable.IsGrabbed);
            StopCoroutine(selfFlicker);
            selfGlow.Stop();
            IEnumerator targetFlicker = FlickerTarget();
            StartCoroutine(targetFlicker);
            yield return new WaitUntil(()=>complete || !grabbable.IsGrabbed);
            StopCoroutine(targetFlicker);
            LightUpTarget(false);
        }
        renderer.material.SetColor("_GlowColor", originalColor);
        grabbable.OverrideGlow(false);
        ChangeTargetColor(true);
        if(loopTask)
        {
            yield return new WaitUntil(()=>!complete);
            StartTask();
        }
        else
        {
            this.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == target)
        {
            complete = true;
            playerAudioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == target)
        {
            complete = false;
        }
    }

    private void LightUpTarget(bool value)
    {
        foreach(Glow glow in targetGlows)
        {
            if(value) glow.Activate();
            else glow.Stop();
        }
    }
    private void ChangeTargetColor(bool changeBack)
    {
        if(!changeBack)
        {
            originalTargetColors = new Color[targetGlows.Length];
            for(int i=0; i<targetGlows.Length; i++)
            {
                Renderer renderer = targetGlows[i].GetComponent<Renderer>();
                originalTargetColors[i] = renderer.material.GetColor("_GlowColor");
                renderer.material.SetColor("_GlowColor", Color.magenta);
            }
        }
        else
        {
            for(int i=0; i<targetGlows.Length; i++)
            {
                Renderer renderer = targetGlows[i].GetComponent<Renderer>();
                renderer.material.SetColor("_GlowColor", originalTargetColors[i]);
            }
        }
    }

    private IEnumerator FlickerSelf()
    {
        while(true)
        {
            selfGlow.Activate();
            yield return new WaitForSeconds(0.5f);
            selfGlow.Stop();
            yield return new WaitForSeconds(0.3f);
        }
    }
    private IEnumerator FlickerTarget()
    {
        while(true)
        {
            LightUpTarget(true);
            yield return new WaitForSeconds(0.5f);
            LightUpTarget(false);
            yield return new WaitForSeconds(0.3f);
        }
    }

}
