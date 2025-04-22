using System.Collections;
using UnityEngine;
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
    public bool rotationFrozen = false;
    private Quaternion frozenRotation;
    private float originalAngularDrag;
    private Vector2 lookInput;

    [Header("Cutscene Control")]
    private bool movementFrozen = false;

    [Header("Dialogue")]
    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

    [Header("Audio")]
    [SerializeField] private AudioSource footsteps = null;
    [SerializeField] private AudioClip footstepsound;

    

    private GameManager gameManager;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        if (!rotationFrozen)
        {
            look = lookInput;
        }
        else
        {
            look = Vector2.zero;
        }
    }

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        originalAngularDrag = rb.angularDamping;
    }

    void FixedUpdate()
    {
        if (dialogueUI != null && dialogueUI.IsOpen) return;

        if (rotationFrozen)
        {
            rb.angularVelocity = Vector3.zero;
            transform.rotation = frozenRotation;
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

    public void FreezeRotation(bool freeze)
    {
        rotationFrozen = freeze;

        if (freeze)
        {
            frozenRotation = transform.rotation;
            rb.angularVelocity = Vector3.zero;
            rb.angularDamping = float.MaxValue;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            look = Vector2.zero;
        }
        else
        {
            rb.angularDamping = originalAngularDrag;
            rb.constraints = RigidbodyConstraints.None;
            rb.freezeRotation = true;
            look = lookInput;
        }
    }

    void MovePlayer()
    {
        if (movementFrozen || rotationFrozen ||
            (dialogueUI != null && dialogueUI.IsOpen) ||
            animator.GetBool("IsPickingUp") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("playermodelsitting") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerLookingAround"))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        

        Vector3 moveDirection = orientation.forward * move.y + orientation.right * move.x;
        rb.linearVelocity = moveDirection * speed;

        bool isMoving = move.magnitude > 0.1f;
        animator.SetBool("IsWalking", isMoving);

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

    private IEnumerator SwingThatNet()
    {
        animator.SetBool("IsSwinging", true);
        yield return new WaitForSeconds(2f);
        animator.SetBool("IsSwinging", false);
    }

    private void Update()
    {
        if (dialogueUI != null && dialogueUI.IsOpen) return;
        if (rotationFrozen) return;

        if (Input.GetKeyDown(KeyCode.F) && Interactable != null)
        {
            Interactable.Interact(this);
        }

        if (gameManager.inbugscene && Input.GetKeyDown("k"))
        {
            StartCoroutine(SwingThatNet());
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