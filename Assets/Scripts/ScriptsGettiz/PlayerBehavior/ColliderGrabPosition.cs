using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColliderGrabPosition : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject player;
    
    [Header("Interpolation Settings")]
    public float sideDistance = 1.5f;
    public float smoothSpeed = 10f;

    private Vector2 rawInput;
    private bool IsRight = true;
    private float currentXOffset;
    

    private void Update()
    {
        //TODO ADD A WAY TO AVOID OBJECTS CLIPPING THOUGH WALLS
        
        rawInput = playerInput.actions["Move"].ReadValue<Vector2>();
        
        if (rawInput.x > 0.1f)
        {
            IsRight = true;
        }
        else if (rawInput.x < -0.1f)
        {
            IsRight = false;
        }
        
        float targetX;
        if (IsRight)
            targetX = sideDistance;
        else
            targetX = -sideDistance;
        
        currentXOffset = Mathf.Lerp(currentXOffset, targetX, Time.deltaTime * smoothSpeed);
        
        transform.position = new Vector3(player.transform.position.x + currentXOffset, player.transform.position.y, player.transform.position.z);
    }
}
