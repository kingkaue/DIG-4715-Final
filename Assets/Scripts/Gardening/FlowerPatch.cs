using UnityEngine;
using UnityEngine.AI;

public class FlowerPatch : MonoBehaviour
{
    [Header ("Flower Spawning")]
    [SerializeField] private BoxCollider[] flowerPatches;
    [SerializeField] private GameObject[] flowers;
    [SerializeField] private int avgFlowersPerPatch;

    void Awake()
    {
        SpawnFlowers();
    }
    
    public void SpawnFlowers()
    {
        // Chooses a random number of flowers to spawn
        int numFlowersInPatch = Random.Range(avgFlowersPerPatch - 1, avgFlowersPerPatch + 2);
        foreach (BoxCollider patch in flowerPatches)
        {
            // Spawns flowers in random points of each box collider
            for (int i = 0; i < numFlowersInPatch; i ++)
            {
                GameObject flower = flowers[Random.Range(0, flowers.Length)];
                Vector3 randomPoint = GetRandomPointFlower(patch);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
                {
                    Instantiate(flower, hit.position, Quaternion.identity);
                }
            }
        }
    }

    // Method to get the random point
    Vector3 GetRandomPointFlower(BoxCollider patch)
    {
        Vector3 center = patch.bounds.center;
        Vector3 size = patch.bounds.size;

        float randomX = Random.Range(center.x - size.z / 2, center.x + size.x / 2);
        float randomZ = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(randomX, center.y, randomZ);
    }
}
