using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAR : MonoBehaviour {
    [Header("Audio Clips")]
    [SerializeField] 
    private AudioClip audioClipTakeOutWeapon;   // 무기 장착 SFX
    
    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting;    // 무기 설정
    private float lastAttackTime = 0;       // 직전 발사 시간 

    private AudioSource audioSource;
    private PlayerAnimatorController playerAnimatorController;

    private bool isTaked = false;


    private void Init() {
        this.audioSource = GetComponent<AudioSource>();
        this.playerAnimatorController = GetComponentInParent<PlayerAnimatorController>();
    }

    private void Awake() {
        Init();
    }

    private void OnEnable() {
        PlaySound(this.audioClipTakeOutWeapon);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            this.audioSource.Stop();
            this.audioSource.clip = this.audioClipTakeOutWeapon;
            this.audioSource.Play();
        }
    }

    private void PlaySound(AudioClip clip) {        
        this.audioSource.Stop();
        this.audioSource.clip = clip;
        this.audioSource.Play();
        Debug.Log($"{audioSource.isPlaying}");
    }

    public void StartWeaponAction(int type = 0) {
        if (type == 0) {    // 좌측 마우스 버튼 클릭; 사격
            if (this.weaponSetting.isAuto == true) {
                StartCoroutine(OnAttackLoop());
            }
            else {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0) {
        if (type == 0) {
            StopCoroutine(OnAttackLoop());
        }
    }

    private void OnAttack() {
        if (Time.time - this.lastAttackTime > this.weaponSetting.attackRate) {
            if (this.playerAnimatorController.MoveSpeed > 0.5f) {   // 달리고 있을 때는 공격 불가
                return;
            }

            this.lastAttackTime = Time.time;    // 발사 시간 업데이트
            this.playerAnimatorController.Play("Fire", -1, 0);
        }
    }

    private IEnumerator OnAttackLoop() {
        while (true) {
            OnAttack();
            yield return null;
        }
    }








}
