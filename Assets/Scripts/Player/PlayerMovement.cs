using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("General")]
    private Rigidbody rb;
    public Transform orientation;
    public Animator animator;

    [Header("Movement")]
    public float speed;
    public Vector2 move;

    [Header("Aiming")]
    public Vector2 look;

    [Header("Cutscene Control")]
    private bool movementFrozen = false;

    //For Dialogue System: 
    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

    [SerializeField] private AudioSource footsteps = null;
    [SerializeField] private AudioClip footstepsound;
    private bool isMoving = false;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (dialogueUI != null)
        {
            if (dialogueUI.IsOpen) return;
        }

        MovePlayer();
    }

    public void FreezeMovement(bool freeze)
    {
        movementFrozen = freeze;
        if (freeze)
        {
            rb.linearVelocity = Vector3.zero;
            animator.SetBool("IsWalking", false);
            if (footsteps.isPlaying) footsteps.Stop();
        }
    }

    void MovePlayer()
    {
        // Don't move if movement is frozen or in special states
        if (movementFrozen ||
            (dialogueUI != null && dialogueUI.IsOpen) ||
            animator.GetBool("IsPickingUp") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("playermodelsitting") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerLookingAround"))
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 moveDirection = orientation.forward * move.y + orientation.right * move.x;
        rb.linearVelocity = moveDirection * speed;

        // Update walking state
        bool isMoving = move.magnitude > 0.1f;
        animator.SetBool("IsWalking", isMoving);

        // Footstep sounds
        if (isMoving && !footsteps.isPlaying)
        {
            footsteps.clip = footstepsound;
            footsteps.loop = true;
            footsteps.Play();
        }
        else if (!isMoving && footsteps.isPlaying)
        {
            footsteps.loop = false;
            footsteps.Stop();
        }
    }

    private void Update()
    {
        if (dialogueUI != null)
        {
            if (dialogueUI.IsOpen) return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Interactable != null)
            {
                Interactable.Interact(this);
            }
        }
    }

    public void Save(ref PlayerSaveData data)
    {
        data.Position = transform.position;
    }

    public void Load(PlayerSaveData data)
    {
        transform.position = data.Position;
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 Position;
    public float Spirit;
    public float MaxSpirit;
}