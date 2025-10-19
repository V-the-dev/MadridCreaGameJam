using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;




public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private MessageManager messageManager;

    //Transform tr;
    Rigidbody2D rb;
    Animator animator;

    [SerializeField]
    float speed = 5f;
    [SerializeField]
    Vector2 movementMults = new Vector2(1f, 0.75f);
    bool isMoving = false;

    private PlayerInput playerInput;
    private Vector2 inputVector;

    public IInteractuable nearest;
    private Dictionary<Collider2D, IInteractuable> interactuables = new Dictionary<Collider2D, IInteractuable>();

    private void Awake()
    {
        //tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        inputVector = playerInput.actions["Move"].ReadValue<Vector2>();

        if (isMoving=inputVector.magnitude > 0)
        {
            inputVector.Normalize();
        }

        animator.SetFloat("MoveX", inputVector.x);
        animator.SetFloat("MoveY", inputVector.y);
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            SortByDistance();
        }

        if (interactuables.Count > 0 && playerInput.actions["Interact"].WasPressedThisFrame())
        {
            nearest.Interact(messageManager);

        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + inputVector * movementMults * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactuable"))
        {
            var interactuable = collision.GetComponentInParent<IInteractuable>();

            if (interactuable != null && !interactuables.ContainsKey(collision))
            {
                interactuables.Add(collision, interactuable);
                Debug.Log("Entered Trigger Zone: " + collision.name);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactuable"))
        {
            var toRemove = interactuables[collision];
            if (toRemove != null)
            {
                interactuables[collision].NearestIndicator(false);
                interactuables.Remove(collision);
                SortByDistance();
                Debug.Log("Exited Trigger Zone: " + collision.name);
            }
        }
    }

    private void SortByDistance()
    {
        if (interactuables == null || interactuables.Count == 0)
        {
            nearest = null;
            return;
        }

        float minDist = float.MaxValue;
        KeyValuePair<Collider2D, IInteractuable> nearestKvp = default;
        bool found = false;

        foreach (var kvp in interactuables)
        {
            var mb = kvp.Key as Collider2D;
            if (mb == null)
                continue;

            float dist = Vector2.Distance(rb.position, mb.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestKvp = kvp;
                found = true;
            }
        }

        if (found)
        {
            IInteractuable newNearest = nearestKvp.Value;

            // Solo si el más cercano ha cambiado sew actualiza el indicador
            if (nearest != newNearest)
            {
                // Desactivar indicador anterior (si existía)
                if (nearest != null)
                    nearest.NearestIndicator(false);

                // Activar indicador en el nuevo más cercano
                nearest = newNearest;
                nearest.NearestIndicator(true);

                var mb = nearest as MonoBehaviour;
                Debug.Log("Nearest Interactuable: " + mb.gameObject.name);
            }
        }
    }
}
