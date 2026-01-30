using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    /*
    public float moveSpeed = 6f;
    public float turnSpeed = 12f;
    public float jumpForce = 6f;
    Rigidbody _body;
    Vector2 _input;
    

    void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _body.freezeRotation = true;
    }

    void Update()
    {
        _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _input = Vector2.ClampMagnitude(_input, 1f);
        
    }

    void FixedUpdate()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
        Vector3 targetDir = camForward * _input.y + camRight * _input.x;

        if (targetDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir, Vector3.up);
            _body.MoveRotation(Quaternion.Slerp(_body.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
            Vector3 move = targetDir * moveSpeed;
            _body.MovePosition(_body.position + move * Time.fixedDeltaTime);
        }
        else
        {
            _body.linearVelocity = new Vector3(0f, _body.linearVelocity.y, 0f);
        }
        if (Input.GetButtonDown("Jump"))
            _body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }*/
}