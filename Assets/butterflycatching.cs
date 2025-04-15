using System.Collections;
using UnityEngine;

public class butterflycatching : MonoBehaviour
{

    private float butterflyscaught = 0;
    public GameObject butterflyprefab;
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
        if(other.tag == "butterfly")
        {
           StartCoroutine(addbutterflyscore());
        }
    }

    private IEnumerator addbutterflyscore()
    {
        butterflyscaught++;
        yield return new WaitForSeconds(1f);
        yield return null;
        Debug.Log(butterflyscaught);
    }
}
