using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAR : MonoBehaviour {
    [Header("Audio Clips")]
    [SerializeField] 
    private AudioClip audioClipTakeOutWeapon;
    private AudioSource audioSource;


    private void Init() {
        this.audioSource = GetComponent<AudioSource>();
    }

    private void Awake() {
        Init();
    }

    private void OnEnable() {
        PlaySound(this.audioClipTakeOutWeapon);
    }

    private void PlaySound(AudioClip clip) {
        this.audioSource.Stop();
        this.audioSource.clip = clip;
        this.audioSource.Play();
    }








}
