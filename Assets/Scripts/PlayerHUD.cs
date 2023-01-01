using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {
    [Header("Components")]
    [SerializeField] 
    private WeaponAR weapon;
    [SerializeField] 
    private Status status;

    [Header("Weapon Base")]
    [SerializeField] 
    private TextMeshProUGUI textWeaponName;

    [Header("Ammo")]
    [SerializeField] 
    private TextMeshProUGUI textAmmo;

    [Header("Magazie")]
    [SerializeField]
    private GameObject magazineUIPrefab;
    [SerializeField]
    private Transform magazineParent;
    private List<GameObject> magazineList;

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP;
    [SerializeField]
    private Image imageBloodScreen;
    [SerializeField]
    private AnimationCurve curveBloodScreen;


    private void Init() {
        SetupWeapon();
        SetupMagazine();

        // Listener 등록
        this.weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        this.weapon.onMagzineEvent.AddListener(UpdateMagazineHUD);
        this.status.onHPEvent.AddListener(UpdateHPHUD);
    }   

    private void Awake() {
        Init();
    }

    private void SetupWeapon() {
        this.textWeaponName.text = this.weapon.WeaponName.ToString();
    }

    private void SetupMagazine() {
        this.magazineList = new List<GameObject>();

        for (int i = 0; i < this.weapon.MaxMagazine; ++i) {
            GameObject clone = Instantiate(this.magazineUIPrefab);

            clone.transform.SetParent(this.magazineParent);
            clone.SetActive(false);
            this.magazineList.Add(clone);
        }

        for (int i = 0; i < this.weapon.CurrentMagazine; ++i) { // 초기화
            this.magazineList[i].SetActive(true);
        }
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo) {
        this.textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }

    private void UpdateMagazineHUD(int currentMagazine) {
        for (int i = 0; i < this.magazineList.Count; ++i) {
            this.magazineList[i].SetActive(false);
        }

        for (int i = 0; i < currentMagazine; ++i) {
            this.magazineList[i].SetActive(true);
        }
    }
    private void UpdateHPHUD(int previous, int current) {
        this.textHP.text = "HP: " + current;

        if (previous <= current) {  // HP 증가
            return;
        }

        if (previous - current > 0) {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    private IEnumerator OnBloodScreen() {
        float percent = 0;

        while (percent < 1) {
            percent += Time.deltaTime;

            Color color = this.imageBloodScreen.color;
            color.a = Mathf.Lerp(0.5f, 0, this.curveBloodScreen.Evaluate(percent));
            this.imageBloodScreen.color = color;

            yield return null;
        }
    }

}