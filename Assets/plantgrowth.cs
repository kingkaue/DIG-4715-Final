using UnityEngine;

public class plantgrowth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f); // Makes the object 2x bigger
        }

    }
}
