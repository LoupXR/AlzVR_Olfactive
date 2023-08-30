using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Knob : MonoBehaviour
{
    [Tooltip("The child that will move")]
    public GameObject knobObject;
    [Tooltip("The burner that the knob turn on")]
    public GameObject burnerObject;
    [Tooltip("Audio to play when turning knobs")]
    public AudioSource audioToPlay;

    [Tooltip("Set the speed at which knob is rotating")]
    public float RotationSpeed = 0.5f;

    [Tooltip("Final Angle Rotation the Knob will have after the button is pressed")]
    private float finalAngle = 0;
    [Tooltip("To know if the knob has already been pressed")]
    private bool hasRotated = false;
    [Tooltip("To know if the knob started rotating or not")]
    private bool hasStarted = false;
    [Tooltip("To know if the angle is already calculated for the actual rotation")]
    private bool angleCalculated = false;
    [Tooltip("To know if the knob is in position for light the burner or not")]
    private bool lighted = false;
    [Tooltip("To know if the user wants to interact with the knob")]
    private bool entered = false;
    [Tooltip("To know if the knob is doing its thing")]
    private bool onGoing = false;
    [Tooltip("To know if the audio of knob turning has already been played or not")]
    private bool audioPlayed = false;

    [Tooltip("Material used for the knob")]
    public Material normalKnobMaterial;
    [Tooltip("Material used when knob highlighted")]
    public Material glowingKnobMaterial;

    [Tooltip("Material used for the burner")]
    public Material normalBurnerMaterial;
    [Tooltip("Material used when burner is turned on")]
    public Material turnOnBurnerbMaterial;

    private Side hand;

    private void Update()
    {
        if (entered || onGoing)
        {
            if (hasStarted)
            {
                onGoing = true;
                if (!angleCalculated)
                {
                    finalAngle = CalculAngle();
                }
                //If Knob rotation point isn't near what we want, then rotate him
                if (knobObject.transform.localRotation.z < (finalAngle / 100) - 0.05 || knobObject.transform.localRotation.z > (finalAngle / 100) + 0.05)
                {
                    if (finalAngle == -1)
                    {
                        knobObject.transform.Rotate(new Vector3(0, 0, finalAngle) * RotationSpeed * 50 * Time.deltaTime);
                    }
                    else
                    {
                        knobObject.transform.Rotate(new Vector3(0, 0, finalAngle) * RotationSpeed * Time.deltaTime);
                        if (!audioPlayed)
                        {
                            audioToPlay.Play();
                            audioPlayed = true;
                        }
                    }
                }
                //When Knob rotation is over check if we have to turn on the burner or turn it off
                else
                {
                    if (knobObject.transform.localRotation.z >= (finalAngle / 100) - 0.05 && knobObject.transform.localRotation.z <= (finalAngle / 100) + 0.05 && finalAngle == 90)
                    {
                        lighted = true;
                        burnerObject.GetComponent<MeshRenderer>().material = turnOnBurnerbMaterial;
                        burnerObject.GetComponent<Burner>().turnedOn = true;
                    }
                    else
                    {
                        lighted = false;
                        burnerObject.GetComponent<MeshRenderer>().material = normalBurnerMaterial;
                        burnerObject.GetComponent<Burner>().turnedOn = false;
                    }
                    hasStarted = false;
                    angleCalculated = false;
                    onGoing = false;
                    audioPlayed = false;
                }
            }
        } else
        {
            hasStarted = false;
        }
    }

    // Calculate Angle based on previous angle and bool.
    public float CalculAngle()
    {
        if (hasRotated)
        {
            hasRotated = false;
            finalAngle = -1;
        }
        else
        {
            hasRotated = true;
            finalAngle = 90;
        }
        angleCalculated = true;
        return finalAngle; 
    }

    private void OnTriggerEnter(Collider other)
    {
        knobObject.GetComponent<Glow>().Activate();

        entered = true;

        //If inside collision zone and press trigger then start rotating
        InputAction actionLeft = hand == Side.Left ? Player.Instance.controls.Main.GrabLeft : Player.Instance.controls.Main.GrabRight;
        actionLeft.performed += context => hasStarted = true;

        InputAction actionRight = hand == Side.Right ? Player.Instance.controls.Main.GrabLeft : Player.Instance.controls.Main.GrabRight;
        actionRight.performed += context => hasStarted = true;
    }

    private void OnTriggerExit(Collider other)
    {
        knobObject.GetComponent<Glow>().Stop();

        entered = false;
    }

    public bool GetLighted()
    {
        return lighted;
    }
}
