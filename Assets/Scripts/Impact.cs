using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour {
    private ParticleSystem particle;
    private MemoryPool memoryPool;


    private void Init() {
        this.particle = GetComponent<ParticleSystem>();
    }

    private void Awake() {
        Init();
    }

    public void Setup(MemoryPool pool) {
        this.memoryPool = pool;
    }

    private void Update() {
        if (!this.particle.isPlaying) {
            this.memoryPool.DeactivatePoolItem(gameObject);
        }
    }
 
}