using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour {
    [SerializeField] private float fadeSpeed = 4;
    private MeshRenderer meshRenderer;


    private void Init() {
        this.meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Awake() {
        Init();
    }

    private void OnEnable() {
        StartCoroutine("OnFadeEffect");
    }

    private void OnDisable() {
        StopCoroutine("OnFadeEffect");
    }

    private IEnumerator OnFadeEffect() {
        while(true) {
            Color color = this.meshRenderer.material.color;
            color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * this.fadeSpeed, 1));
            meshRenderer.material.color = color;
            
            yield return null;
        }
    }








}