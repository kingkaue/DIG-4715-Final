using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class butterflycatching : MonoBehaviour
{
    private float butterflyscaught = 0;
    public GameObject butterflyprefab;
    private PlayerManager playerManager;
    private string originalSceneName; // Store the scene where the net belongs

    void Start()
    {
        originalSceneName = SceneManager.GetActiveScene().name;


        // Find the PlayerManager in the scene
        playerManager = FindObjectOfType<PlayerManager>();

        // Start checking if we're still in the correct scene
        StartCoroutine(CheckSceneCoroutine());
    }

    private IEnumerator CheckSceneCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f); // Check every 0.5 seconds

            // If we're no longer in the original scene, despawn the net
            if (SceneManager.GetActiveScene().name != originalSceneName)
            {
                Destroy(gameObject);
            }
        }
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

       
    }
}