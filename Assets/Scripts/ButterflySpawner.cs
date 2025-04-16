using UnityEngine;
using System.Collections.Generic;

public class ButterflySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject butterflyPrefab;
    [SerializeField] private int numberOfButterflies = 10;
    [SerializeField] private float spawnRadius = 1f;
    [SerializeField] private float minHeight = 1f;
    [SerializeField] private float maxHeight = 3f;

    [Header("Movement Settings")]
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private float directionChangeInterval = 3f;
    [SerializeField] private float maxWanderDistance = 5f;

    private List<Butterfly> butterflies = new List<Butterfly>();

    void Start()
    {
        SpawnButterflies();
    }

    void SpawnButterflies()
    {
        for (int i = 0; i < numberOfButterflies; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject butterflyObj = Instantiate(butterflyPrefab, spawnPos, Quaternion.identity);
            butterflyObj.transform.parent = transform; // Parent to spawner

            Butterfly butterfly = butterflyObj.AddComponent<Butterfly>();
            butterfly.Initialize(
                Random.Range(minSpeed, maxSpeed),
                directionChangeInterval,
                maxWanderDistance,
                transform // Pass spawner's transform as reference
            );

            butterflies.Add(butterfly);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        float height = Random.Range(minHeight, maxHeight);
        return transform.position + new Vector3(randomCircle.x, height, randomCircle.y);
    }
}