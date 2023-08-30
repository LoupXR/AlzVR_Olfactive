using UnityEngine;

public class SaucepanFilling : MonoBehaviour
{
    public AudioSource audioSource;

    private bool hasStarted = false;
    private bool isFilled = false;

    private float elapsedTime = 0;
    [SerializeField] private float waterSpeed = 0.1f;
    [SerializeField] private Vector3 saucepanWaterScaling;
    [SerializeField] private float risingDuration = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(hasStarted)
        {
            if(!isFilled)
            {
                audioSource.Play();
            }

            isFilled = true;

            if (elapsedTime < risingDuration)
            {
                transform.Translate(Vector3.up * waterSpeed * Time.deltaTime);
                transform.localScale += saucepanWaterScaling;
                elapsedTime += Time.deltaTime;
            }
            else
            {
                hasStarted = false;
            }
        }
    }

    public bool GetIsFilled()
    {
        return isFilled;
    }

    public void SetStarted()
    {
        hasStarted = true;
    }
}
