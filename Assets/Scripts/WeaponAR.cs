using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class WeaponAR : MonoBehaviour {

    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent onMagzineEvent = new MagazineEvent();

    [SerializeField]
    private Animator ARAnimator;
    [Header("Audio Clips")]
    [SerializeField] 
    private AudioClip audioClipTakeOutWeapon;   // 무기 장착 SFX
    [SerializeField]
    private AudioClip audioClipFireFX;          // 무기 격발 SFX
    [SerializeField]
    private AudioClip audioClipReloadFX;        // 무기 재장전 SFX
    
    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting;    // 무기 설정

    [Header("Fire VFX")]
    [SerializeField]
    private GameObject muzzleFlashFX;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint;
    [SerializeField]
    private Transform bulletSpawnPoint;

    [Header("Aim UI")]
    [SerializeField]
    private Image imageAim;

    private bool isTaked = false;
    private bool isReload = false;
    private bool isAttack = false;  // 공격 여부 체크
    private bool isModeChange = false;  // 정조준 여부 체크
    private float defaultModeFOV = 60;
    private float aimModeFOV = 30;  // 정조준 시 FOV
    private float lastAttackTime = 0;       // 직전 발사 시간 

    private AudioSource audioSource;
    private PlayerAnimatorController playerAnimatorController;
    private CasingMemoryPool casingMemoryPool;
    private ImpactMemoryPool impactMemoryPool;
    private Camera mainCamera;
    
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;


    private void Init() {
        this.audioSource = GetComponent<AudioSource>();
        this.casingMemoryPool = GetComponent<CasingMemoryPool>();
        this.impactMemoryPool = GetComponent<ImpactMemoryPool>();
        this.playerAnimatorController = GetComponentInParent<PlayerAnimatorController>();
        this.weaponSetting.currentAmmo = this.weaponSetting.maxAmmo;
        this.weaponSetting.currentMagazine = this.weaponSetting.maxMagazine;
        this.mainCamera = Camera.main;
    }

    private void Awake() {
        Init();
    }

    private void OnEnable() {
        PlaySound(this.audioClipTakeOutWeapon);
        this.muzzleFlashFX.SetActive(false);
        this.onAmmoEvent.Invoke(this.weaponSetting.currentAmmo, this.weaponSetting.maxAmmo);
        this.onMagzineEvent.Invoke(this.weaponSetting.currentMagazine);
        ResetValiables();
    }

    private void Update() {
        if (this.playerAnimatorController.AimModeIs) {
            this.playerAnimatorController.Aiming = 1;
        }
        else if (!this.playerAnimatorController.AimModeIs) {
            this.playerAnimatorController.Aiming = 0;
        }
    }

    private void PlaySound(AudioClip clip) {       
        //this.audioSource.Stop(); 
        //this.audioSource.clip = clip;
        this.audioSource.PlayOneShot(clip); // 오디오 중첩 플레이 허용
    }

    public void StartWeaponAction(int type = 0) {
        if (this.isReload == true) {    // 재장전 시 무기 액션 불가
            return;
        }

        if (this.isModeChange == true) {    // 정조준 도중에는 무기 액션 불가
            return;
        }

        if (type == 0) {    // 좌측 마우스 버튼 클릭; 사격
            if (this.weaponSetting.isAuto == true) {
                this.isAttack = true;
                StartCoroutine("OnAttackLoop");
            }
            else {
                OnAttack();
            }
        }
        else if (type == 1) {  // 우측 마우스 버튼 클릭; 정조준
            if (this.isAttack == true) {
                return;
            }

            this.playerAnimatorController.AimModeIs = true;
            this.imageAim.enabled = false;
            //StartCoroutine("OnFOVChange");
        }
    }

    public void StartReload() {
        if (this.isReload == true || this.weaponSetting.currentAmmo == this.weaponSetting.maxAmmo || this.weaponSetting.currentMagazine <= 0) {
            return;
        }
        StopWeaponAction(); // 무기 액션 중지
        StartCoroutine("OnReload");
    }

    public void StopWeaponAction(int type = 0) {
        if (type == 0) {    // 좌측 마우스 버튼 클릭 해제; 사격 종료
            this.isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
        else if (type == 1) {
            this.playerAnimatorController.AimModeIs = false;
            this.imageAim.enabled = true;
            //StartCoroutine("OnFOVChange");
        }
    }

    private void OnAttack() {
        if (Time.time - this.lastAttackTime > this.weaponSetting.attackRate) {
            if (this.playerAnimatorController.MoveSpeed > 0.6f) {   // 달리고 있을 때는 공격 불가
                return;
            }

            this.lastAttackTime = Time.time;    // 발사 시간 업데이트

            if (this.weaponSetting.currentAmmo <= 0) {   // 탄 수 측정
                return;
            }
            this.weaponSetting.currentAmmo -= 1;
            this.onAmmoEvent.Invoke(this.weaponSetting.currentAmmo, this.weaponSetting.maxAmmo);
            
            // 발사 애니메이션 재생
            this.playerAnimatorController.Play("Fire", 1, 0);  
            
            /*
            string animation = this.playerAnimatorController.AimModeIs == true ? "AimFire" : "Fire";
            int layer = animation.Equals("Fire") == true ? 1 : 0;
            this.playerAnimatorController.Play(animation, layer, 0);
            */
            StartCoroutine("OnMuzzleFlashFX");
            PlaySound(this.audioClipFireFX);
            this.casingMemoryPool.SpawnCasing(this.casingSpawnPoint.position, transform.right);
            TwoStepRayCast();
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

    private IEnumerator OnReload() {
        if (!this.playerAnimatorController.CurrentAnimationIs("Aim_Pose")) {        
            this.isReload = true;
            this.playerAnimatorController.OnReload();
            this.ARAnimator.SetTrigger("onReload");
            PlaySound(this.audioClipReloadFX);    
        }

        while(this.isReload) {
            this.isReload = false;
                
            this.weaponSetting.currentMagazine -= 1;    // 탄창 -1개
            this.onMagzineEvent.Invoke(this.weaponSetting.currentMagazine);

            this.weaponSetting.currentAmmo = this.weaponSetting.maxAmmo;    // 탄약 충전
            this.onAmmoEvent.Invoke(this.weaponSetting.currentAmmo, this.weaponSetting.maxAmmo);
            
            yield return null;
        }
    }

    private IEnumerator OnFOVChange() {
        float current = 0;
        float percent = 0;
        float time = 0.35f;

        float start = this.mainCamera.fieldOfView;
        float end = this.playerAnimatorController.AimModeIs == true ? this.aimModeFOV : this.defaultModeFOV;

        this.isModeChange = true;

        while (percent < 1) {
            current += Time.deltaTime;
            percent = current / time;

            this.mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);
            yield return null;
        }

        this.isModeChange = false;
    }

    private void ResetValiables() {
        this.isReload = false;
        this.isAttack = false;
        this.isModeChange = false;
    }

    private void TwoStepRayCast() {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;
        
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        
        if (Physics.Raycast(ray, out hit, this.weaponSetting.attackDistance)) {
            targetPoint = hit.point;
        }
        else {
            targetPoint = ray.origin + ray.direction * this.weaponSetting.attackDistance;
        }

        Debug.DrawRay(ray.origin, ray.direction * this.weaponSetting.attackDistance, Color.red);

        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;

        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, this.weaponSetting.attackDistance)) {
            this.impactMemoryPool.SpawnImpact(hit);
        }

        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * this.weaponSetting.attackDistance, Color.blue);
    }
}
