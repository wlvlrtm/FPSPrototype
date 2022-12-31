using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : InteractionObject {
    [SerializeField]
    private AudioClip clipTargetUp;
    [SerializeField]
    private AudioClip clipTargetDown;
    [SerializeField]
    private float targetUpDelayTime = 3;

    private AudioSource audioSource;
    private bool isPossibleHit = true;


    private void Init() {
        this.audioSource = GetComponent<AudioSource>();
    }

    private void Awake() {
        Init();
    }

    public override void TakeDamage(int damage) {
        currentHP -= damage;

        if (currentHP <= 0 && this.isPossibleHit == true) {
            this.isPossibleHit = false;
            StartCoroutine("OnTargetDown");
        }
    }

    private IEnumerator OnTargetDown() {
        this.audioSource.clip = this.clipTargetDown;
        this.audioSource.Play();

        yield return StartCoroutine(OnAnimation(0, 90));

        StartCoroutine("OnTargetUp");
    }

    private IEnumerator OnTargetUp() {
        yield return new WaitForSeconds(this.targetUpDelayTime);

        this.audioSource.clip = this.clipTargetUp;
        this.audioSource.Play();

        yield return StartCoroutine(OnAnimation(90, 0));
        
        this.isPossibleHit = true;
    }

    private IEnumerator OnAnimation(float start, float end) {
        float percent = 0;
        float current = 0;
        float time = 1;

        while (percent < 1) {
            current += Time.deltaTime;
            percent = current / time;

            transform.rotation = Quaternion.Slerp(Quaternion.Euler(start, 0, 0), Quaternion.Euler(end, 0, 0), percent);

            yield return null;
        }
    }
}