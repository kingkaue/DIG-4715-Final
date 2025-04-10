using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    [Header("Growth Settings")]
    public float growthAmount = 0.1f; // How much to grow per water hit
    public float maxScale = 2f;       // Maximum size the plant can reach
    public float growthDuration = 1f; // How long growth animation takes
    public Vector3 maxRotation = new Vector3(0, 360, 0); // Optional wiggle rotation

    private Vector3 targetScale;
    private bool isGrowing;
    private float growthProgress;
    private Vector3 initialScale;
    private Quaternion initialRotation;

    public PlayerManager playerManager;

    void Start()
    {
        initialScale = transform.localScale;
        initialRotation = transform.localRotation;
        targetScale = initialScale;
    }

    void Update()
    {
        if (isGrowing)
        {
            growthProgress += Time.deltaTime / growthDuration;

            // Smooth growth
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, growthProgress);

            // Optional wiggle effect
            if (maxRotation != Vector3.zero)
            {
                float wiggleAmount = Mathf.Sin(growthProgress * Mathf.PI * 4) * 5f;
                transform.localRotation = initialRotation * Quaternion.Euler(
                    maxRotation.x * wiggleAmount * 0.01f,
                    maxRotation.y * wiggleAmount * 0.01f,
                    maxRotation.z * wiggleAmount * 0.01f
                );
               
            }

            if (growthProgress >= 1f)
            {
                isGrowing = false;
                transform.localRotation = initialRotation; // Reset rotation
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            GrowPlant();
            playerManager.SetSpirit(10f);
            Destroy(other.gameObject); // Remove the water sphere
        }
    }

    public void GrowPlant()
    {
        if (transform.localScale.x >= maxScale) return;

        targetScale = Vector3.Min(
            transform.localScale + (Vector3.one * growthAmount),
            initialScale * maxScale
        );

        growthProgress = 0f;
        isGrowing = true;

        // Optional: Play growth sound
        // AudioSource.PlayClipAtPoint(growthSound, transform.position);
    }
}