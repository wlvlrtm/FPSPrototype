using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingMemoryPool : MonoBehaviour {
    [SerializeField] 
    private GameObject casingPrefab;
    private MemoryPool memoryPool;

    private void Awake() {
        this.memoryPool = new MemoryPool(this.casingPrefab);
    }

    public void SpawnCasing(Vector3 position, Vector3 direction) {
        GameObject item = this.memoryPool.ActivatePoolItem();

        item.transform.position = position;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().Init(memoryPool, direction);
    }
}