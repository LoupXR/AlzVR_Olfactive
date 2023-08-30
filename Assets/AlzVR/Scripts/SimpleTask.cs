using System;
using System.Collections;
using UnityEngine;

public class SimpleTask : MonoBehaviour {
    private const float MaxIterations = 1e2f;
    private const float Tolerance = 1e-2f;
    
    private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
    public GameObject target;
    public AudioSource playerAudioSource;
    public bool loopTask = true;
    private Grabbable _grabbable;
    private Color[] _originalTargetColors;
    private Renderer _renderer;
    private Glow _selfGlow;
    private Glow[] _targetGlows;
    public bool Complete { get; private set; }

    // Start is called before the first frame update
    private void Awake() {
        _grabbable = GetComponent<Grabbable>();
        _selfGlow = GetComponent<Glow>();
        if (_selfGlow == null) _selfGlow = GetComponentInChildren<Glow>();
        _targetGlows = target.GetComponentsInChildren<Glow>();
        _renderer = _selfGlow.GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == target) {
            Complete = true;
            playerAudioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == target) Complete = false;
    }


    public void StartTask() { StartCoroutine(Task()); }

    private IEnumerator Task() {
        _grabbable.OverrideGlow(true);
        Color originalColor = _renderer.material.GetColor(GlowColor);
        _renderer.material.SetColor(GlowColor, Color.magenta);
        ChangeTargetColor(false);
        while (!Complete) {
            IEnumerator selfFlicker = FlickerSelf();
            StartCoroutine(selfFlicker);
            yield return new WaitUntil(() => _grabbable.IsGrabbed);
            StopCoroutine(selfFlicker);
            _selfGlow.Stop();
            IEnumerator targetFlicker = FlickerTarget();
            StartCoroutine(targetFlicker);
            yield return new WaitUntil(() => Complete || !_grabbable.IsGrabbed);
            StopCoroutine(targetFlicker);
            LightUpTarget(false);
        }

        _renderer.material.SetColor(GlowColor, originalColor);
        _grabbable.OverrideGlow(false);
        ChangeTargetColor(true);
        if (loopTask) {
            yield return new WaitUntil(() => !Complete);
            StartTask();
        }
        else {
            enabled = false;
        }
    }

    private void LightUpTarget(bool value) {
        foreach (Glow glow in _targetGlows)
            if (value) glow.Activate();
            else glow.Stop();
    }

    private void ChangeTargetColor(bool changeBack) {
        if (!changeBack) {
            _originalTargetColors = new Color[_targetGlows.Length];
            for (var i = 0; i < _targetGlows.Length; i++) {
                var component = _targetGlows[i].GetComponent<Renderer>();
                _originalTargetColors[i] = component.material.GetColor(GlowColor);
                component.material.SetColor(GlowColor, Color.magenta);
            }
        }
        else {
            for (var i = 0; i < _targetGlows.Length; i++) {
                var component = _targetGlows[i].GetComponent<Renderer>();
                component.material.SetColor(GlowColor, _originalTargetColors[i]);
            }
        }
    }

    private IEnumerator FlickerSelf() {
        var iterationNumber = 0;

        while (true) {
            ++iterationNumber;

            _selfGlow.Activate();
            yield return new WaitForSeconds(0.5f);
            _selfGlow.Stop();
            yield return new WaitForSeconds(0.3f);

            if (Math.Abs(iterationNumber - MaxIterations) < Tolerance) break;
        }
    }

    private IEnumerator FlickerTarget() {
        var iterationNumber = 0;

        while (true) {
            ++iterationNumber;

            LightUpTarget(true);
            yield return new WaitForSeconds(0.5f);
            LightUpTarget(false);
            yield return new WaitForSeconds(0.3f);

            if (Math.Abs(iterationNumber - MaxIterations) < Tolerance) break;
        }
    }
}