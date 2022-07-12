using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Playercontroller : MonoBehaviour
{
    private Controls controls;
    private PlayerInput _playerInput;
    private Camera mainCamera;
    private Vector2 moveInput;
    private Rigidbody _rigidbody;
    private bool _isGrounded;
    public float moveMultiplier;
    public float maxVelocity;
    public float rayDistance;
    public LayerMask layerMask;
    public float jumpForce;

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        controls = new Controls();
        
        _playerInput = GetComponent<PlayerInput>();
        
        mainCamera = Camera.main;

        _playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnDisable()
    {
        _playerInput.onActionTriggered -= OnActionTriggered;
    }

    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        if (obj.action.name.CompareTo(controls.Gameplay.Movement.name) == 0)
        {
            moveInput = obj.ReadValue<Vector2>();
        }

        if (obj.action.name.CompareTo(controls.Gameplay.jump.name) ==0)
        {
            if (obj.performed) jump();
        }
    }

    private void Move()
    {

        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        
        
        _rigidbody.AddForce((camForward * moveInput.y + camRight * moveInput.x) * moveMultiplier * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LimitVelocity()
    {
        Vector3 velocity = _rigidbody.velocity;
        if (Mathf.Abs(velocity.x) > maxVelocity) velocity.x = Mathf.Sign(velocity.x) * maxVelocity;
        
        velocity.z = Mathf.Clamp(value: velocity.z, min: -maxVelocity, maxVelocity);
        

        _rigidbody.velocity = velocity;
    }

    private void CheckGround()
    {
        RaycastHit collision;

        if (Physics.Raycast(transform.position, Vector3.down, out collision, rayDistance, layerMask))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void jump()
    {
        if (_isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        CheckGround();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector3.down * rayDistance, Color.yellow);
    }
}
