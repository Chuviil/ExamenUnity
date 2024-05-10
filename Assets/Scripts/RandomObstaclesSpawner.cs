using UnityEngine;

public class RandomObstaclesSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] obstaclesSpawnLocations;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField][Range(0, 100)]private int spawnProbability = 50; // Define your probability here (0 - 100)

    private void Start()
    {
        SpawnEnemies();
    }


    private void SpawnEnemies()
    {
        foreach (var location in obstaclesSpawnLocations)
        {
            if (Random.Range(0, 100) <= spawnProbability)
            {
                Instantiate(obstaclePrefab, location.position, Quaternion.identity, location);
            }
        }
    }
}