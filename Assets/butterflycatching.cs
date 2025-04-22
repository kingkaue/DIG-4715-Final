using System.Collections;
using UnityEngine;

public class butterflycatching : MonoBehaviour
{
    private float butterflyscaught = 0;
    public GameObject butterflyprefab;
    private PlayerManager playerManager;

    void Start()
    {
        // Find the PlayerManager in the scene
        playerManager = FindObjectOfType<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Butterfly"))
        {
            StartCoroutine(addbutterflyscore(other.gameObject));
        }
    }

    private IEnumerator addbutterflyscore(GameObject butterfly)
    {
        butterflyscaught++;

        // Increase spirit by 5
        if (playerManager != null)
        {
            playerManager.SetSpirit(2f);
        }

        // Optional: Disable the butterfly instead of destroying it
        butterfly.SetActive(false);

        yield return new WaitForSeconds(1f);
        Debug.Log("Butterfly caught! Total: " + butterflyscaught);

        // Optional: Respawn butterfly if desired
        // Instantiate(butterflyprefab, randomPosition, Quaternion.identity);
    }
}