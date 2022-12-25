using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class Status : MonoBehaviour {
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    public float WalkSpeed {
        get {return this.walkSpeed;}
        set {this.walkSpeed = value;}
    }
    public float RunSpeed {
        get {return this.runSpeed;}
        set {this.runSpeed = value;}
    }

    [Header("HP")]
    [SerializeField]
    private int maxHP = 100;
    private int currentHP;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;


    private void Init() {
        this.currentHP = this.maxHP;
    }
    
    private void Awake() {
        Init();
    }

    public bool DecreaseHP(int damage) {
        int previousHP = this.currentHP;

        this.currentHP = this.currentHP - damage > 0 ? currentHP - damage : 0;
        this.onHPEvent.Invoke(previousHP, currentHP);

        if (currentHP == 0) {
            return true;
        }
        
        return false;
    }
}