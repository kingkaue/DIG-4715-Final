using System.Collections;
using UnityEngine;

public class WateringCanTilt : MonoBehaviour
{
    [Header("References")]
    public GameObject wateringCan;
    public Transform waterDispensePoint;
    public GameObject waterSpherePrefab;

    [Header("Tilt Settings")]
    public float tiltAngle = 40f;
    public float tiltDuration = 0.5f;
    public float holdDuration = 2f;
    public float returnDuration = 0.5f;

    [Header("Water Settings")]
    public float pourDuration = 1.5f;
    public float spawnInterval = 0.1f;
    public float sphereLifetime = 2f;
    public Vector3 spawnRandomness = new Vector3(0.02f, 0f, 0.02f);

    private ObjectGrabable objectGrabable;
    private bool isTilting = false;
    private Quaternion originalRotation;

    void Start()
    {
        objectGrabable = GetComponentInChildren<ObjectGrabable>();
        if (wateringCan == null) wateringCan = gameObject;
        originalRotation = wateringCan.transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) &&
            objectGrabable != null &&
            objectGrabable.isGrabbed &&
            !isTilting)
        {
            StartCoroutine(WateringProcess());
        }
    }

    private IEnumerator WateringProcess()
    {
        isTilting = true;

        // Tilt forward
        Quaternion startRot = wateringCan.transform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(tiltAngle, 0, 0);

        float elapsed = 0f;
        while (elapsed < tiltDuration)
        {
            wateringCan.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / tiltDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Start pouring water
        if (waterSpherePrefab && waterDispensePoint)
        {
            StartCoroutine(PourWater());
        }

        // Hold while pouring
        yield return new WaitForSeconds(holdDuration);

        // Return to original position
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            wateringCan.transform.rotation = Quaternion.Slerp(targetRot, originalRotation, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        wateringCan.transform.rotation = originalRotation;
        isTilting = false;
    }

    private IEnumerator PourWater()
    {
        float startTime = Time.time;
        float nextSpawnTime = 0f;

        while (Time.time - startTime < pourDuration)
        {
            if (Time.time >= nextSpawnTime)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnRandomness.x, spawnRandomness.x),
                    Random.Range(-spawnRandomness.y, spawnRandomness.y),
                    Random.Range(-spawnRandomness.z, spawnRandomness.z)
                );

                GameObject waterSphere = Instantiate(
                    waterSpherePrefab,
                    waterDispensePoint.position + randomOffset,
                    Quaternion.identity
                );

                Destroy(waterSphere, sphereLifetime);
                nextSpawnTime = Time.time + spawnInterval;
            }
            yield return null;
        }
    }
}