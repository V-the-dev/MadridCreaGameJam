using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;



public class PlayerMovement : MonoBehaviour
{
    Transform tr;
    Rigidbody2D rb;
    Animator animator;

    [SerializeField]
    float speed = 5f;
    [SerializeField]
    Vector2 movementMults = new Vector2(1f, 0.75f);

    private PlayerInput playerInput;
    private Vector2 inputVector;

    private void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        inputVector=playerInput.actions["Move"].ReadValue<Vector2>();

        if (inputVector.magnitude > 0)
            inputVector.Normalize();


        animator.SetFloat("MoveX", inputVector.x);
        animator.SetFloat("MoveY", inputVector.y);
        animator.SetBool("IsMoving", inputVector.sqrMagnitude>0.01f);

    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + inputVector * movementMults * speed * Time.deltaTime);
    }


}
