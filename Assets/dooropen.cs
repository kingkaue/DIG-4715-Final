using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    private bool playerInTrigger = false;

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // Toggle door state
            bool isOpen = doorAnimator.GetBool("IsOpen");
            doorAnimator.SetBool("IsOpen", !isOpen);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}