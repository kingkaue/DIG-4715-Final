using UnityEngine;

public class setparentofnet : MonoBehaviour
{

    [SerializeField] private GameObject holdposition;
     private Transform holdpositiontrans;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        holdpositiontrans = holdposition.transform;
        transform.SetParent(holdpositiontrans);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
