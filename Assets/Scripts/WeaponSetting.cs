using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName
{
    AssaultRife = 0
}

[System.Serializable]
public struct WeaponSetting {
    public WeaponName weaponName;   // 무기 이름
    public int currentAmmo;         // 현재 탄약 수
    public int maxAmmo;             // 최대 탄약 수
    public int currentMagazine;     // 현재 탄창 수
    public int maxMagazine;         // 최대 탄창 수
    public float attackRate;        // 공격 속도
    public float attackDistance;    // 공격 사거리
    public bool isAuto;             // 연속 공격 여부
}
