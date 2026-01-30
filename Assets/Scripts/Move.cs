using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Move : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rb;
    private Vector3 moveVector;

    private bool abilityAir;
    [SerializeField] private float speed;
    [SerializeField] private float JumpSpeed;
    [SerializeField] private float DownJumpForce;
    
    private bool isJumping;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveVector = playerInput.actions["Move"].ReadValue<Vector2>().normalized;
    }
    
    private void FixedUpdate()
    {
        Vector3 newmove = new Vector3(moveVector.x * speed, 0, 0); 
        if (isJumping) rb.AddForce(Vector3.down * DownJumpForce);
        
        rb.AddForce(newmove * Time.fixedDeltaTime);
    }

    // Update is called once per frame
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            
        }
        
        if (context.performed)
        {
            rb.AddForce(Vector3.up * JumpSpeed, ForceMode.Impulse);
            isJumping = true;
        }

        if (context.canceled)
        {
            isJumping = false;
        }
    }
    
    public void Fly(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            
        }
    }
}
