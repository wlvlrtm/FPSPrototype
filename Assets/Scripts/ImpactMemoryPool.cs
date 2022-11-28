using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactType {
    Normal = 0,
    Obstacle
}
public class ImpactMemoryPool : MonoBehaviour {
    [SerializeField]
    private GameObject[] impactPrefab;
    private MemoryPool[] memoryPool;


    private void Init() {
        this.memoryPool = new MemoryPool[this.impactPrefab.Length];

        for (int i = 0; i < this.impactPrefab.Length; ++i) {
            this.memoryPool[i] = new MemoryPool(this.impactPrefab[i]);
        }
    }

    private void Awake() {
        Init();
    }

    public void SpawnImpact(RaycastHit hit) {
        if (hit.transform.CompareTag("ImpactNormal")) {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactObstacle")) {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation) {
        GameObject item = memoryPool[(int)type].ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);
    }


}