using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float dampTime = 0.15f;
    [SerializeField]
    Transform target;

    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    //private PlayerInput playerInput;
    //private Vector3 camInput = Vector3.zero;

    Rigidbody2D rb;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody2D>();
        //playerInput = GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        if (target)
        {
            Vector3 point = cam.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;

            rb.MovePosition(Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime));       //realmente mueve un collider con la camara acoplada

        }

        //if (playerInput.actions["RightClick"].IsPressed())
        //{
        //    camInput = playerInput.actions["Look"].ReadValue<Vector2>();
        //    camInput.Normalize();
        //    rb.MovePosition(Vector3.SmoothDamp(destination, destination + camInput, ref velocity, dampTime));
        //}
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}