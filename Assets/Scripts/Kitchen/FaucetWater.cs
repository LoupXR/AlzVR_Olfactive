using UnityEngine;

public class FaucetWater : MonoBehaviour
{
    private SaucepanFilling saucepanWater;
    public AudioSource audioSource;

    private bool isReached = false;
    private bool isOpen = false;
    private bool hasStarted = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (hasStarted)
        {
            if (isOpen && isReached)
            {
                if(!saucepanWater.GetIsFilled())
                {
                    saucepanWater.SetStarted();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "SaucepanWater")
        {
            isReached = true;
            saucepanWater = other.GetComponent<SaucepanFilling>();          
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "SaucepanWater")
        {
            isReached = false;
        }
    }

    public void SetStarted()
    {
        hasStarted = true;
    }

    public void SetOpen(bool value)
    {
        isOpen = value;
    }
}
