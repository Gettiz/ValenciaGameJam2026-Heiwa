using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpritePosition : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject player;

    public float rotationRight;
    public float rotationLeft;
    public float rotationUp;



    private Vector2 rawInput;
    private bool IsRight = true;
    private float currentXOffset;
    

    private void Update()
    {
        if (!PauseBehavior.isPaused)
        {
            rawInput = playerInput.actions["Move"].ReadValue<Vector2>();
        
            if (rawInput.x > 0.1f)
            {
                IsRight = true;
            }
            else if (rawInput.x < -0.1f)
            {
                IsRight = false;
            }
        
            if (IsRight)
                transform.rotation = Quaternion.Euler(rotationUp, rotationRight,transform.rotation.z);
            else
                transform.rotation = Quaternion.Euler(rotationUp, rotationLeft, transform.rotation.z);
        }
    }
}
