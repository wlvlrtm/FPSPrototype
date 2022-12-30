using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionObject : MonoBehaviour {
    [Header("Interaction Object")]
    [SerializeField]
    protected int maxHP = 100;
    protected int currentHP;


    private void Init() {
        this.currentHP = this.maxHP;
    }

    private void Awake() {
        Init();
    }

    public abstract void TakeDamage(int damage);
}
