using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movespeed;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontaleInput;
    float verticalInput;

    Vector3 movementDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        //handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        } else
        {
            rb.drag = 0;
        }

        MyInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontaleInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        //Calculate move direction
        movementDirection = orientation.forward * verticalInput + orientation.right * horizontaleInput;

        rb.AddForce(movementDirection.normalized * movespeed * 10, ForceMode.Force);
    }
}
