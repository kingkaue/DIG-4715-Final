using System.Collections;
using UnityEngine;

public class AnimalFeed : MonoBehaviour
{
    [SerializeField] private AudioSource animaleating = null;
    [SerializeField] private AudioClip animaleatingsound;
    [SerializeField] private float scoreIncreaseAmount = 1f; // How much to increase the deer score
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Animal")
        {

            StartCoroutine(eatingapple());
        }
    }

    private IEnumerator eatingapple()
    {
        animaleating.PlayOneShot(animaleatingsound);

        // Increase the deer score in GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IncreaseDeerScore(scoreIncreaseAmount);
        }

        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }
}
