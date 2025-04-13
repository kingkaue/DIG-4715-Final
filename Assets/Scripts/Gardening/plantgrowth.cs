using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    [Header("Growth Settings")]
    public float growthAmount = 0.1f;
    public Vector3 absoluteMaxScale = new Vector3(8f, 8f, 8f); // Use absolute max scale
    public float growthDuration = 1f;
    public Vector3 maxRotation = new Vector3(0, 360, 0);

    private Vector3 targetScale;
    private bool isGrowing;
    private float growthProgress;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private bool hasReachedMaxSize = false;

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
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, growthProgress);

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
                transform.localRotation = initialRotation;

                // Check if any axis reached max (or use Mathf.Approximately)
                if (transform.localScale.x >= absoluteMaxScale.x - 0.001f ||
                    transform.localScale.y >= absoluteMaxScale.y - 0.001f ||
                    transform.localScale.z >= absoluteMaxScale.z - 0.001f)
                {
                    hasReachedMaxSize = true;
                    transform.localScale = absoluteMaxScale; // Snap to exact max size
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            GrowPlant();
            Destroy(other.gameObject);

            if (!hasReachedMaxSize)
            {
                WateringCanTilt wateringCan = FindObjectOfType<WateringCanTilt>();
                if (wateringCan != null && wateringCan.TryGetWaterReward())
                {
                    playerManager.SetSpirit(5f);
                }
            }
        }
    }

    public void GrowPlant()
    {
        if (hasReachedMaxSize) return;

        Vector3 newScale = transform.localScale + (Vector3.one * growthAmount);
        targetScale = Vector3.Min(newScale, absoluteMaxScale);

        // Early check if this growth will max out the plant
        if (Vector3.Distance(targetScale, absoluteMaxScale) < 0.001f)
        {
            hasReachedMaxSize = true;
        }

        growthProgress = 0f;
        isGrowing = true;
    }
}