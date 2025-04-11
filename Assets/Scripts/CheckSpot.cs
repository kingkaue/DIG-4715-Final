using UnityEngine;

public class CheckSpot : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup Item")
        {
            Debug.Log("Item Placed Correctly");
            other.transform.position = this.transform.position;
        }
    }
}
