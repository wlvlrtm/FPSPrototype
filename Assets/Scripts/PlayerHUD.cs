using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private WeaponAR weapon;

    [Header("Weapon Base")]
    [SerializeField] private TextMeshProUGUI textWeaponName;
    
    [Header("Ammo")]
    [SerializeField] private TextMeshProUGUI textAmmo;


    private void Init() {
        SetupWeapon();
        this.weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
    }   

    private void Awake() {
        Init();
    }

    private void SetupWeapon() {
        this.textWeaponName.text = this.weapon.weaponName.ToString();
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo) {
        this.textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }

}