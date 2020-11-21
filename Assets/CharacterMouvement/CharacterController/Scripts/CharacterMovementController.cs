using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField]
    float moveSpeed = 10.0f;
    [Range(1.0f, 5.0f)]
    [SerializeField]
    float divideMoveSpeedInAir = 2.0f;
    [Tooltip("in unit")]
    [SerializeField]
    float jumpHeight = 3.0f;
    [Header("\"Physics\"")]
    [SerializeField]
    float gravity = -9.81f;
    [Range(-1.0f, 5.0f)]
    [SerializeField]
    float gravityModifier = 1.0f;
    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    float groundDistance = 0.4f;
    [SerializeField]
    LayerMask groundMask;

    bool isGrounded = false;

    CharacterController cc;


    Vector3 velocity;

    // Awake est appelé quand l'instance de script est chargée
    private void Awake()
    {
        if (!TryGetComponent(out cc))
            throw new System.Exception("I need CharacterController Component !!");

        if (gravityModifier == 0)
            gravityModifier = 1;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float calculedMoveSpeed = moveSpeed;
        isGrounded = Physics.CheckSphere(groundCheck.position, this.groundDistance, this.groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -1.0f;
        else
            calculedMoveSpeed /= divideMoveSpeedInAir;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Deplacement 
        Vector3 motion = transform.right * x + transform.forward * z;
        cc.Move(motion * calculedMoveSpeed * Time.deltaTime);

        if (CanJump() && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * (gravity * gravityModifier));
        }

        //Gravity
        velocity.y += (gravity * gravityModifier) * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }


    private bool CanJump()
    {
        //If we want implement double jump or somthing ...
        return isGrounded;
    }
}
