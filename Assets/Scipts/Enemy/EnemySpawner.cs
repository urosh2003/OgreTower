using UnityEngine;

public class EnemySpawner: MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject spawnedObject;
    [SerializeField] private bool spawnOnTrigger;

    private void Start()
    {
        CheckpointManager.refreshLevel += RespawnEnemy;
        RespawnEnemy();
    }

    private void OnDestroy()
    {
        CheckpointManager.refreshLevel -= RespawnEnemy;
    }

    private void RespawnEnemy()
    {
        if(spawnedObject!=null)
            Destroy(spawnedObject);
        if(!spawnOnTrigger)
            SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        spawnedObject = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}