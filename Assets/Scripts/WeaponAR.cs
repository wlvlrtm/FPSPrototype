using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAR : MonoBehaviour {
    [Header("Audio Clips")]
    [SerializeField] 
    private AudioClip audioClipTakeOutWeapon;   // 무기 장착 SFX
    [SerializeField]
    private AudioClip audioClipFireFX;
    
    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting;    // 무기 설정
    private float lastAttackTime = 0;       // 직전 발사 시간 

    [Header("Fire VFX")]
    [SerializeField]
    private GameObject muzzleFlashFX;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint;

    private AudioSource audioSource;
    private PlayerAnimatorController playerAnimatorController;
    private CasingMemoryPool casingMemoryPool;
    private bool isTaked = false;


    private void Init() {
        this.audioSource = GetComponent<AudioSource>();
        this.playerAnimatorController = GetComponentInParent<PlayerAnimatorController>();
        this.casingMemoryPool = GetComponent<CasingMemoryPool>();
    }

    private void Awake() {
        Init();
    }

    private void OnEnable() {
        PlaySound(this.audioClipTakeOutWeapon);
        this.muzzleFlashFX.SetActive(false);
    }

    private void PlaySound(AudioClip clip) {       
        this.audioSource.Stop(); 
        this.audioSource.clip = clip;
        this.audioSource.Play();
    }

    public void StartWeaponAction(int type = 0) {
        if (type == 0) {    // 좌측 마우스 버튼 클릭; 사격
            if (this.weaponSetting.isAuto == true) {
                StartCoroutine("OnAttackLoop");
            }
            else {
                OnAttack();
            }
        }
    }

    public void StopWeaponAction(int type = 0) {
        if (type == 0) {
            StopCoroutine("OnAttackLoop");
        }
    }

    private void OnAttack() {
        if (Time.time - this.lastAttackTime > this.weaponSetting.attackRate) {
            if (this.playerAnimatorController.MoveSpeed > 0.6f) {   // 달리고 있을 때는 공격 불가
                return;
            }

            this.lastAttackTime = Time.time;    // 발사 시간 업데이트
            this.playerAnimatorController.Play("Fire", -1, 0);  // 발사 애니메이션 재생
            StartCoroutine("OnMuzzleFlashFX");
            PlaySound(this.audioClipFireFX);
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
        }
    }

    private IEnumerator OnAttackLoop() {
        while (true) {
            OnAttack();
            yield return null;
        }
    }

    private IEnumerator OnMuzzleFlashFX() {
        this.muzzleFlashFX.SetActive(true);
        yield return new WaitForSeconds(this.weaponSetting.attackRate * 0.3f);
        this.muzzleFlashFX.SetActive(false);
    }
}
