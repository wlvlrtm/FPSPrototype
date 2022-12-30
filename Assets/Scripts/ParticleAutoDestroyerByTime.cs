using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroyerByTime : MonoBehaviour {
    private ParticleSystem particle;

    
    private void Init() {
        this.particle = GetComponent<ParticleSystem>();
    }

    private void Awake() {
        Init();
    }

    private void Update() {
        if (this.particle.isPlaying == false) {
            Destroy(gameObject);
        }
    }
}