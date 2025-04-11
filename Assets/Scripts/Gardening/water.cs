using UnityEngine;

public class water : MonoBehaviour
{

    [Header("Physics Settings")]
    public float downwardForce = 2f;
    public float lifetime = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.down * downwardForce, ForceMode.Impulse);

        Destroy(gameObject, lifetime);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "testpickupitem")
        {
            Destroy(this.gameObject);
        }

    }
}
