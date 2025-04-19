using UnityEngine;

public class FlowerGarden : MonoBehaviour
{
    [SerializeField] private GameObject[] flowerSpot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceFlower(GameObject flower)
    {
        for (int i = 0;  i < flowerSpot.Length; i++)
        {
            if (flowerSpot[i].GetComponent<FlowerSpot>().hasFlower == false)
            {
                Rigidbody rb = flower.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }

                flower.transform.position = flowerSpot[i].transform.position;
                flower.transform.SetParent(flowerSpot[i].transform);
                flowerSpot[i].GetComponent<FlowerSpot>().hasFlower = true;
                return;
            }
        }
    }
}
