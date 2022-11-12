using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour {
    [SerializeField] 
    private float deactivateTime = 5.0f;
    [SerializeField]
    private float casingSpin = 1.0f;
    [SerializeField]
    private AudioClip[] audioClips;

    private Rigidbody rigidbody;
    private AudioSource audioSource;
    private MemoryPool memoryPool;


    public void Init(MemoryPool pool, Vector3 direction) {
        this.rigidbody = GetComponent<Rigidbody>();
        this.audioSource = GetComponent<AudioSource>();
        this.memoryPool = pool;

        this.rigidbody.AddForce(new Vector3(direction.x, 1.0f, direction.z), ForceMode.VelocityChange);
        this.rigidbody.angularVelocity = new Vector3(Random.Range(-this.casingSpin, this.casingSpin), Random.Range(-this.casingSpin, this.casingSpin), Random.Range(-this.casingSpin, this.casingSpin));
        StartCoroutine("DeactivateAfterTime");
    }

    private void OnCollisionEnter(Collision other) {
        int index = Random.Range(0, this.audioClips.Length);

        this.audioSource.clip = this.audioClips[index];
        this.audioSource.Play();
    }

    private IEnumerator DeactivateAfterTime() {
        yield return new WaitForSeconds(this.deactivateTime);
        memoryPool.DeactivatePoolItem(this.gameObject);
    }
}