using UnityEngine;

public class CameraZoomOutTransform : MonoBehaviour
{
    public float zoomDistance = 2f;  // How far back to move
    public float duration = 8f;      // How long the movement takes

    void Start()
    {
        StartCoroutine(MoveBack());
    }

    private System.Collections.IEnumerator MoveBack()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position - transform.forward * zoomDistance;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, t / duration);
            yield return null;
        }

        transform.position = targetPos;
    }
}
