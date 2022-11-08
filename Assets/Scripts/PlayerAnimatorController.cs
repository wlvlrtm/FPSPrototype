using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour {
    private Animator animator;
    public float MoveSpeed {
        set => this.animator.SetFloat("Speed", value, 0.05f, Time.deltaTime);
        get => this.animator.GetFloat("Speed");    
    }


    private void Init() {
        this.animator = gameObject.GetComponentInChildren<Animator>();
    }

    private void Awake() {
        Init();
    }
}
