using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Fire : MonoBehaviour
{
    [Tooltip("No more fires will be instantiated by this one once there are that many instances or more in the scene")]
    public int maxInstances = 10;
    public VisualEffect fire;
    public float maxFlamesSize = 3;
    public new Light light;
    public float maxLightRange = 1;
    public AudioSource sound;
    public float maxSoundVolume = 0.25f;

    [Tooltip("How long it takes for each new fire to grow to its final size")]
    public float maxGrowthTime = 30;
    [Tooltip("A new fire will be created from this one every SpreadPeriod seconds on average (ie the number of instances doubles over that period)")]
    public float spreadPeriod = 10;
    [Tooltip("New fires are projected with a velocity ranging between X and Y in a random direction")]
    public Vector2 spreadVelocityRange = new Vector2(1,3);

    public bool stopFire;

    private new Rigidbody rigidbody;
    private new SphereCollider collider;
    private bool stuck;
    private static List<Fire> fires;

    void Start()
    {
        GameManager.Instance.StopFire += StopFire;
    }

    public void StartFire()
    {
        collider = GetComponent<SphereCollider>();
        rigidbody = GetComponent<Rigidbody>();
        GameManager.Instance.StopFire += StopFire;
        if(fires == null) fires = new List<Fire>();
        fires.Add(this);
        gameObject.SetActive(true);
        StartCoroutine(FireGrowth());
        StartCoroutine(FireMultiplication());
        GameManager.Instance.StopFire += StopFire;
    }

    IEnumerator FireGrowth()
    {
        float flamesSize = 0.05f;
        fire.SetFloat("FlamesSize", flamesSize);
        light.range = 0.1f;
        sound.volume = 0;
        gameObject.SetActive(true);
        float startTime = Time.time;
        while(Time.time < startTime + maxGrowthTime)
        {
            yield return null;
            float incrementCoef = Time.deltaTime/maxGrowthTime;
            sound.volume += maxSoundVolume * incrementCoef;
            light.range += (maxLightRange - 0.1f) *incrementCoef;
            flamesSize += (maxFlamesSize - 0.05f) * incrementCoef;
            fire.SetFloat("FlamesSize", flamesSize);
            transform.localPosition += Vector3.up * maxFlamesSize / 10 * incrementCoef;
            collider.center -= Vector3.up * maxFlamesSize / 10 * incrementCoef;
        }
        
    }
    IEnumerator FireMultiplication()
    {
        while(!stopFire && fires.Count < maxInstances)
        {
            yield return new WaitForSeconds(Random.Range(spreadPeriod * 0.5f, spreadPeriod * 1.5f));

            GameObject fireOffShoot = Instantiate(gameObject, transform.position, transform.rotation);
            fireOffShoot.transform.localScale = transform.lossyScale;
            Fire fire = fireOffShoot.GetComponent<Fire>();
            fire.StartFire();
            fire.Jump();
        }
    }

    public void Jump()
    {
        transform.SetParent(null);
        transform.position += Vector3.up * 0.05f;
        if (rigidbody == null) rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        Vector3 velocity = Random.onUnitSphere * Random.Range(spreadVelocityRange.x, spreadVelocityRange.y);
        velocity.y = Mathf.Abs(velocity.y) + 0.3f;
        rigidbody.velocity = velocity;
    }

    void OnTriggerEnter(Collider other)
    {
        if(!stuck)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            transform.SetParent(other.transform);
        }
    }

    public void StopFire()
    {
        stopFire = true;
        if(fires.Count == 1)
        {
            GameManager.Instance.End();
        }
        fires.Remove(this);
        gameObject.SetActive(false);
    }
}
