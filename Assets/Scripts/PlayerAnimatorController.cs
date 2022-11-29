using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour {
    private Animator animator;
    public float MoveSpeed {
        set => this.animator.SetFloat("Speed", value, 0.05f, Time.deltaTime);
        get => this.animator.GetFloat("Speed");    
    }
    public bool AimModeIs {
        set => this.animator.SetBool("isAimMode", value);
        get => this.animator.GetBool("isAimMode");
    }
    public float Aiming {
        set => this.animator.SetFloat("Aiming", value, 0.05f, Time.deltaTime);
        get => this.animator.GetFloat("Aiming");
    }

    private void Init() {
        this.animator = gameObject.GetComponentInChildren<Animator>();
    }

    private void Awake() {
        Init();
    }

    public void Play(string stateName, int layer, float normalizedTime) {
        animator.Play(stateName, layer, normalizedTime);
    }

    public void OnReload() {
        this.animator.SetTrigger("onReload");
    }

    public void OnJump() {
        this.animator.SetTrigger("onJump");
    }

    public bool CurrentAnimationIs(string name) {
        return this.animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
}
