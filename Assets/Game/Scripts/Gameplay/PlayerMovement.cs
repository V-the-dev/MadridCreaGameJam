using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private MessageManager messageManager;

    [Header("SFX")]
    [SerializeField] private float stepSoundDelay = 0.35f;

    private bool canPlayStep = true;

    [SerializeField] private float stepSoundMinPitch = 0.8f;
    [SerializeField] private float stepSoundMaxPitch = 1.2f;

    //Transform tr;
    Rigidbody2D rb;
    Animator animator;

    [SerializeField]
    float speed = 5f;
    [SerializeField]
    Vector2 movementMults = new Vector2(1f, 0.75f);
    [SerializeField]
    Vector2 currentMults ;
    //private Quaternion currentRotation;
    
    [Header("Animation lamp && chuzo")]
    private bool withLamp = false;
    private bool withChuzo = false;
    private RuntimeAnimatorController animWithNOTHING;
    [SerializeField] private AnimatorOverrideController animWithLamp;
    [SerializeField] private AnimatorOverrideController animWithChuzo;
    [SerializeField] private AnimatorOverrideController animWithLampAndChuzo;

    [SerializeField] private GameObject lampObject;
    [SerializeField] private Transform[] lampPositions;

    bool isMoving = false;
    private float animatorSpeed = 0f;

    public PlayerInput playerInput;
    private Vector2 inputVector;
    private Vector2 lastMoveDir = Vector2.down;

    public static InteractableObject nearest;
    private Dictionary<Collider2D, InteractableObject> interactuables = new Dictionary<Collider2D, InteractableObject>();

    private void Awake()
    {
        //tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        currentMults = movementMults;
        animWithNOTHING = animator.runtimeAnimatorController;
    }

    private void Update()
    {
        inputVector = playerInput.actions["Move"].ReadValue<Vector2>();
        
        animatorSpeed = inputVector.magnitude;
        isMoving = animatorSpeed > 0.001f;

        if (isMoving)
        {
            if (canPlayStep)
            {
                SoundManager.PlaySound(SoundType.FOOTSTEPS, useRandomPitch: true, minPitch: stepSoundMinPitch, maxPitch: stepSoundMaxPitch, volume:0.2f);
                StartCoroutine(SFX_stepSound());
            }
            lastMoveDir = inputVector.normalized;
            inputVector.Normalize();

            SortByDistance();
        }

        animator.SetFloat("MoveX", lastMoveDir.x);
        animator.SetFloat("MoveY", lastMoveDir.y);
        animator.SetFloat("Speed", animatorSpeed);

        if (interactuables.Count > 0 && playerInput.actions["Interact"].WasPressedThisFrame()&&Time.timeScale>0)
        {
            nearest.Trigger();

        }

        if (withLamp)
            UpdateLampPosition();
    }


    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + inputVector * currentMults * speed * Time.deltaTime);
    }

    private IEnumerator SFX_stepSound()
    {
        canPlayStep = false;
        yield return new WaitForSecondsRealtime(stepSoundDelay);
        canPlayStep = true;
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactuable"))
        {
            var interactuable = collision.GetComponentInParent<InteractableObject>();

            if (interactuable != null && !interactuables.ContainsKey(collision))
            {
                interactuables.Add(collision, interactuable);
                //Debug.Log("Entered Trigger Zone: " + collision.name);
            }
        }
        if (collision.CompareTag("Proximity"))
        {
            var interactuable = collision.GetComponentInParent<InteractableObject>();

            if (interactuable != null && !interactuables.ContainsKey(collision))
            {
                interactuable.AutoTrigger();
                //Debug.Log("Entered Proximity Zone: " + collision.name);
            }
        }
        if (collision.CompareTag("Stairs"))
        {
            var stairs = collision.GetComponentInParent<StairsZone>();

            if(stairs != null)
            {
                currentMults = stairs.newVec;
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
                //Debug.Log("Exited Trigger Zone: " + collision.name);
            }
        }
        if (collision.CompareTag("Stairs"))
        {
            var stairs = collision.GetComponentInParent<StairsZone>();

            if (stairs != null)
            {
                currentMults = movementMults;
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
        KeyValuePair<Collider2D, InteractableObject> nearestKvp = default;
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
            InteractableObject newNearest = nearestKvp.Value;

            // Solo si el m�s cercano ha cambiado sew actualiza el indicador
            if (nearest != newNearest)
            {
                // Desactivar indicador anterior (si exist�a)
                if (nearest != null)
                        nearest.NearestIndicator(false);

                // Activar indicador en el nuevo m�s cercano
                nearest = newNearest;
                if(nearest.GetComponent<NPCcontroller>().exclamate())
                    nearest.NearestIndicator(true);

                var mb = nearest as MonoBehaviour;
                Debug.Log("Nearest Interactuable: " + mb.gameObject.name);
            }
        }
    }

    public void RestartAnimator()
    {
        withLamp = false;
        lampObject.SetActive(false);
        
        withChuzo = false;
        animator.runtimeAnimatorController = animWithNOTHING;
    }

    public void AddChuzoAnimation()
    {
        withChuzo = true;
        if (withChuzo && withLamp)
        {
            animator.runtimeAnimatorController = animWithLampAndChuzo;
        }
        else
        {
            animator.runtimeAnimatorController = animWithChuzo;
        }
    }
    
    private void UpdateLampPosition()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // Get the state's full path hash (unique identifier)
        int hash = state.fullPathHash;

        // Get the readable name (if you need to compare or print)
        string name = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        switch (name)
        {
            //FRONT
            case "IdleFrontLAMP" or "WalkFrontLAMP":
                lampObject.transform.position = lampPositions[0].position;
                break;
            
            //RIGHT
            case "IdleSideLAMP_Right" or "WalkSideLAMP_Right":
                lampObject.transform.position = lampPositions[6].position;
                break;
            
            //LEFT
            case "IdleSideLAMP_Left" or "WalkSideLAMP_Left":
                lampObject.transform.position = lampPositions[7].position;
                break;
            
            //BACK
            case "IdleBackLAMP" or "WalkBackLAMP":
                lampObject.transform.position = lampPositions[3].position;
                break;
            
            //BACK RIGHT
            case "IdleDiagBackLAMP_Right" or "WalkDiagBackLAMP_Right":
                lampObject.transform.position = lampPositions[1].position;
                break;
            
            //BACK LEFT
            case "IdleDiagBackLAMP_Left" or "WalkDiagBackLAMP_Left":
                lampObject.transform.position = lampPositions[2].position;
                break;
        }
    }
    
    [ContextMenu("Give Player Lamp")]
    public void AddLampAnimation()
    {
        withLamp = true;
        
        lampObject.SetActive(true);

        if (withLamp && withChuzo)
        {
            animator.runtimeAnimatorController = animWithLampAndChuzo;
        }
        else
        {
            animator.runtimeAnimatorController = animWithLamp;
        }
    }

    public static void CheckIfMoreDialogues()
    {
        if(nearest.GetComponent<NPCcontroller>().exclamate())
            nearest.NearestIndicator(true);
        else
            nearest.NearestIndicator(false);
    }
}
