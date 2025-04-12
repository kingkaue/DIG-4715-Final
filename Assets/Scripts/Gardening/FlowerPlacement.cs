using UnityEngine;

public class FlowerPlacement : MonoBehaviour
{
    public GameObject flower;
    public PlayerManager playerManager;

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
            playerManager.SetSpirit(10f);
            flower.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
        }
    }
}
