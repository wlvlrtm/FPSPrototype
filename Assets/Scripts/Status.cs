using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour {
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
}