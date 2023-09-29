using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMulitplier;
    bool readytoJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public enum MovementStat
    {
        freeze,
        unlimited,
    }

    public bool freeze;
    public bool unlimited;
    public bool restricted;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readytoJump = true;
    }

    private void Update()
    {
        MyInput();

        //groundcheck
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        //handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        SpeedControl();

    }
     private void FixedUpdate()
     {
        MovePlayer();
     }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        //when to jump
        if(Input.GetKey(jumpKey) && readytoJump && grounded)
        {
            readytoJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
            Debug.Log("test");
        }
    }
   
    void MovePlayer()
    {

        if (restricted)
        {
            return;
        }
        //calculate move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on groud
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }    

        //in air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMulitplier, ForceMode.Force);
        }
    }

    void SpeedControl()
    {
        Vector3 flatvel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatvel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatvel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

   

    void Jump()
    {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readytoJump = true;
    }

    private void StateHandler()
    {
        if (freeze)
        {
            //state = MovementState.freeze;
            rb.velocity = Vector3.zero;
        }
        else if (unlimited)
        {
            moveSpeed = 999f;
            return;
        }
    }
}
