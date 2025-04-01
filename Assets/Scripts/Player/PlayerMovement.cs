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

    [Header("Movement")]
    public float speed;
    public Vector2 move;

    [Header("Aiming")]
    public Vector2 look;

    //For Dialogue System: 
    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }


    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dialogueUI.IsOpen) return;

        MovePlayer();
    }

    void MovePlayer()
    {
        Vector3 moveDirection = orientation.forward * move.y + orientation.right * move.x;
        rb.linearVelocity = moveDirection * speed;
    }

    private void Update()
    {
        if (dialogueUI.IsOpen) return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(Interactable != null)
            {
                Interactable.Interact(this);
            }
        }
    }
}