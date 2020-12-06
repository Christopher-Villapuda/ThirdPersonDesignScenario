using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyCharacterController : MonoBehaviour
{
    [SerializeField]
    private float accelerationForce = 10;

    [SerializeField]
    private float maxSpeed = 2;

    [SerializeField]
    private PhysicMaterial stoppingPhysicsMaterial, movingPhysicsMaterial;

    [SerializeField]
    [Tooltip("0 = no turning,1 = instant turning")]
    [Range (0,1) ]
    private float turnSpeed =.1f;


    private new Rigidbody rigidbody;
    private Vector2 input;
    private new Collider collider;
    private readonly int movementInputAnimParam = Animator.StringToHash("movementInput");
    private Animator animator;
    

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        Vector3 cameraRelativeInputDirection = GetCameraRelativeInputDirection();
        UpdatePhysicsMaterial();
        Move(cameraRelativeInputDirection);
        RotateToFaceInputDirection(cameraRelativeInputDirection);
    }

    /// <summary>
    /// Turning the character to face the direction it wants to move in.
    /// </summary>
    /// <param name="movementDirection">The direction the character is trying to move in.</param>
    private void RotateToFaceInputDirection(Vector3 movementDirection)
    {
        if (movementDirection.magnitude > 0)
        {
            var targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed);
        }
    }


    /// <summary>
    /// Moves player in a direction based on max speed and acceleration
    /// </summary>
    /// <param name="moveDirection">The direction to move in.</param>
    private void Move(Vector3 moveDirection)
    {
        if (rigidbody.velocity.magnitude < maxSpeed)
        {
            rigidbody.AddForce(moveDirection * accelerationForce, ForceMode.Acceleration);
        }
    }


    /// <summary>
    /// Updates physics material to a low friction option if player is moving
    /// or high friction if they are trying to stop.
    /// </summary>
    private void UpdatePhysicsMaterial()
    {
        collider.material = input.magnitude > 0 ? movingPhysicsMaterial : stoppingPhysicsMaterial;
    }


    /// <summary>
    /// Uses the input of vector to create a camera relatve version
    /// so the player can move based on camera forward
    /// </summary>
    /// <returns>Returns the camera relative input direction</returns>
    private Vector3 GetCameraRelativeInputDirection()
    {
        var inputDirection = new Vector3(input.x, 0, input.y);

        Vector3 cameraFlattenedForward = Camera.main.transform.forward;
        cameraFlattenedForward.y = 0;
        var cameraRotation = Quaternion.LookRotation(cameraFlattenedForward);

        Vector3 cameraRelativeInputDirectionToReturn = (cameraRotation * inputDirection);
        return cameraRelativeInputDirectionToReturn;
    }


    /// <summary>
    /// This Event handler is called from the Player Input component
    /// using the new input system.
    /// </summary>
    /// <param name="context">Vector2 representing move input</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
        animator.SetFloat(movementInputAnimParam, input.magnitude);
    }
}
