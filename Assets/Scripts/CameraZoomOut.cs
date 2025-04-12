using UnityEngine;

public class CameraZoomOutTransform : MonoBehaviour
{
    public float zoomDistance = 2f;
    public float duration = 8f;

    void OnEnable() // Use OnEnable instead of Start
    {
        Debug.Log("CameraZoomOutTransform enabled");
        StartCoroutine(MoveBack());
    }

    private System.Collections.IEnumerator MoveBack()
    {
        Debug.Log("Zoom started");
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position - transform.forward * zoomDistance;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, t / duration);
            yield return null;
        }

        Debug.Log("Zoom finished");
        transform.position = targetPos;
    }
}