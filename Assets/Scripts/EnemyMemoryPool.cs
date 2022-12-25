using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour {
    [SerializeField]
    private Transform target;
    [SerializeField]
    private GameObject enemySpawnPointPrefab;
    [SerializeField] 
    private GameObject enemyPrefab;
    [SerializeField]
    private float enemySpawnTime = 1;
    [SerializeField]
    private float enemySpawnLatency = 1; 

    private MemoryPool spawnPointMemoryPool;
    private MemoryPool enemyMemoryPool;
    private int numberOfEnemiesSpawnedAtOnce = 1;
    private Vector2Int mapSize = new Vector2Int(100, 100);

    
    private void Init() {
        this.spawnPointMemoryPool = new MemoryPool(this.enemySpawnPointPrefab);
        this.enemyMemoryPool = new MemoryPool(this.enemyPrefab);

        StartCoroutine("SpawnTile");
    }

    private void Awake() {
        Init();
    }

    private IEnumerator SpawnTile() {
        int currentNumber = 0;
        int maximumNumber = 50;

        while(true) {
            for (int i = 0; i < this.numberOfEnemiesSpawnedAtOnce; ++i) {
                GameObject item = this.spawnPointMemoryPool.ActivatePoolItem();

                item.transform.position = new Vector3(Random.Range(-this.mapSize.x * 0.5f, mapSize.x * 0.5f), 1, Random.Range(-this.mapSize.y * 0.5f, mapSize.y * 0.5f));
                StartCoroutine("SpawnEnemy", item);
            }

            currentNumber++;

            if (currentNumber >= maximumNumber) {
                currentNumber = 0;
                this.numberOfEnemiesSpawnedAtOnce++;
            }

            yield return new WaitForSeconds(this.enemySpawnTime);
        }
    }


    private IEnumerator SpawnEnemy(GameObject point) {
        yield return new WaitForSeconds(this.enemySpawnLatency);

        GameObject item = this.enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;

        item.GetComponent<EnemyFSM>().Setup(target, this);

        this.spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    public void DeactivateEnemy(GameObject enemy) {
        this.enemyMemoryPool.DeactivatePoolItem(enemy);
    }


}