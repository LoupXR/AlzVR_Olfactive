using UnityEngine;

public class Faucet : Grabbable
{
    [Tooltip("The child that will move")]
    public GameObject faucetObject;

    [Tooltip("Set the speed at which faucet is rotating")]
    public float rotationSpeed = 0.5f;

    [Tooltip("Final Angle Rotation the faucet will have after the button is pressed")]
    private float finalAngle = 0;
    [Tooltip("To know if the knob has already been pressed")]
    private bool hasRotated = false;
    [Tooltip("To know if the knob started rotating or not")]
    private bool hasStarted = false;
    [Tooltip("To know if the angle is already calculated for the actual rotation")]
    private bool angleCalculated = false;

    public FaucetWater faucetWater;
    public Animator faucetWaterAnimator;
    public AudioSource faucetWaterAudioSource;
    private AudioSource faucetHandleAudioSource;

    protected override void Start()
    {
        faucetHandleAudioSource = GetComponent<AudioSource>();
    }

    override public void Grab(Grabber grabber)
    {
        hasStarted = true;
        faucetHandleAudioSource.Play();
    }

    private void Update()
    {
        if (hasStarted)
        {
            faucetWater.SetStarted();

            if (!angleCalculated)
            {
                finalAngle = CalculAngle();
            }

            //If handle rotation point isn't near what we want, then rotate him
            if (faucetObject.transform.rotation.z < (finalAngle / 100) - 0.05 || faucetObject.transform.rotation.z > (finalAngle / 100) + 0.05)
            {
                if (finalAngle == 1)
                {
                    faucetObject.transform.Rotate(new Vector3(0, 0, finalAngle) * rotationSpeed * 50 * Time.deltaTime);                    
                }
                else
                {
                    faucetObject.transform.Rotate(new Vector3(0, 0, finalAngle) * rotationSpeed * Time.deltaTime);                    
                }
            }
            //When handle rotation is over
            else
            {
                hasStarted = false;
                angleCalculated = false;
            }
        }
    }

    // Calculate Angle based on previous angle and bool.
    public float CalculAngle()
    {
        if (hasRotated)
        {
            hasRotated = false;
            finalAngle = 1;
            faucetWater.SetOpen(false);
            faucetWaterAnimator.SetBool("isOpen", false);
            faucetWaterAudioSource.Stop();
        }
        else
        {
            hasRotated = true;
            finalAngle = -90;
            faucetWater.SetOpen(true);
            faucetWaterAnimator.SetBool("isOpen", true);
            faucetWaterAudioSource.Play();
        }
        angleCalculated = true;
        return finalAngle;
    }
}
