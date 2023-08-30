using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AvatarAnimationController : MonoBehaviour
{
    //[SerializeField] private InputActionReference move;
    [SerializeField] private float speedTreshold = 0.1f;

    private Animator animator;
    private AvatarController avatarController;
    private Vector3 previousPos;

    private InputDevice leftDevice;
    private InputDevice rightDevice;

    /* Moving the head */
    private void Start()
    {
        TryInitialize();

        animator = GetComponent<Animator>();
        avatarController = GetComponent<AvatarController>();
        previousPos = avatarController.head.vrTarget.position;
    }

    private void Update()
    {
        if (!leftDevice.isValid || !rightDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            UpdateHandAnimation();
        }

        //Compute the speed
        Vector3 headsetSpeed = (avatarController.head.vrTarget.position - previousPos) / Time.deltaTime;
        headsetSpeed.y = 0;
        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
        previousPos = avatarController.head.vrTarget.position;

        //Set Animator values
        animator.SetBool("isWalking", headsetLocalSpeed.magnitude > speedTreshold);
    }

    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            leftDevice = devices[0];
        }
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            rightDevice = devices[0];
        }
    }

    void UpdateHandAnimation()
    {
        float triggerValue;
        if (leftDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue))
        {
            animator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            animator.SetFloat("Trigger", 0);
        }
        if (rightDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue))
        {
            animator.SetFloat("Trigger2", triggerValue);
        }
        else
        {
            animator.SetFloat("Trigger2", 0);
        }
    }

    /* Moving the joystick */
    /*
    private void OnEnable()
    {
        move.action.started += AnimateLegs;
        move.action.canceled += StopAnimation;
    }
    private void OnDisable()
    {
        move.action.started -= AnimateLegs;
        move.action.canceled -= StopAnimation;
    }

    private void StopAnimation(InputAction.CallbackContext obj)
    {
        bool isMovingForward = move.action.ReadValue<Vector2>().y > 0;

        if(isMovingForward)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
    }

    private void AnimateLegs(InputAction.CallbackContext obj)
    {
        animator.SetBool("isWalking", false);
    }
    */
}
