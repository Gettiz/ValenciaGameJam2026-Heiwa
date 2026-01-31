using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColliderGrabPosition : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject player;
    private Vector2 rawInput;
    private bool IsRight;

    private void Update()
    {
        rawInput = playerInput.actions["Move"].ReadValue<Vector2>();
        
        if (rawInput.x > 0.1)
        {
            IsRight = true;
        }
        else if (rawInput.x < -0.1)
        {
            IsRight = false;
        }
        
        if (IsRight) transform.position = new Vector3(player.transform.position.x + 1.5f,player.transform.position.y,player.transform.position.z);
        else transform.position = new Vector3(player.transform.position.x + -1.5f,player.transform.position.y,player.transform.position.z);
    }
}
