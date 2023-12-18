using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;


// sobre el asunto del authoritative en el player
// https://docs-multiplayer.unity3d.com/netcode/current/components/networktransform/index.html#owner-authoritative-mode
public class PlayerMovement : NetworkBehaviour
{
    public Transform cameraTransform;
    public float walkSpeed = 5.0f;
    public float runningSpeed = 5.0f;
    public float rotationSpeed = 3.0f;
    public float jumpForce = 5.0f; // Fuerza del salto

    private Rigidbody rb;
    private bool isGrounded = true;
    private Animator animComp;
    [SerializeField] GameObject cam;
    private float speed;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (!IsOwner || !IsClient) return;
        animComp = GetComponent<Animator>();
        cam.SetActive(true);
        transform.position = new Vector3(0, 1, 0);
        speed = walkSpeed;
        
        rb = GetComponent<Rigidbody>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }



    private void Update()
    {
        if (!IsOwner || !IsClient) return;
        // Movimiento del personaje
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        //Animacion Caminar
        if (horizontalInput > 0 || verticalInput > 0)
        {
            animComp.SetBool("IsWalking", true);
        }
        else if (horizontalInput < 0 || verticalInput < 0)
        {
            animComp.SetBool("IsWalkingRev", true);
        }
        else
        {
            animComp.SetBool("IsWalking", false);
            animComp.SetBool("IsWalkingRev", false);
        }

        //Animacion Correr
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animComp.SetBool("IsRunning", true);
            speed = runningSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animComp.SetBool("IsRunning", false);
            speed = walkSpeed;
        }

        Vector3 inputDirection = new Vector3(horizontalInput, 0, verticalInput);
        inputDirection = cameraTransform.TransformDirection(inputDirection);
        inputDirection.y = 0;
        inputDirection.Normalize();

        Vector3 movement = inputDirection * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);


        
        // Rotación de la cámara con el mouse
        float mouseX = Input.GetAxis("Mouse X");
        Vector3 rotation = new Vector3(0, mouseX * rotationSpeed, 0);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            animComp.SetTrigger("IsJumping");
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Detectar si el personaje está en el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
