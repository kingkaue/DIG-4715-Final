using UnityEngine;

public class FlowerPlacement : MonoBehaviour
{
    public GameObject flower;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "gardenplane")
        {
            flower.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
        }
    }
}
