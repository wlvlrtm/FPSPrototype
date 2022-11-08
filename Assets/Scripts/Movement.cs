using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour {
    [SerializeField] private float moveSpeed;
        public float MoveSpeed {
            get {return this.moveSpeed;}
            set {this.moveSpeed = Mathf.Max(0, value);}
        }
    private Vector3 moveForce;
    private CharacterController characterController;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;


    private void Init() {
        this.characterController = gameObject.GetComponent<CharacterController>();
    }

    private void Awake() {
        Init();
    }

    private void Update() {
        if (!this.characterController.isGrounded) {
            this.moveForce.y += this.gravity * Time.deltaTime;
        }

        this.characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 dir) {
        dir = transform.rotation * new Vector3(dir.x, 0, dir.z);
        this.moveForce = new Vector3(dir.x * this.moveSpeed, this.moveForce.y, dir.z * this.moveSpeed);
    }

    public void Jump() {
        if (this.characterController.isGrounded) {
            this.moveForce.y = this.jumpForce;  // this.moveForce.y -> 0(default)
        }
    }
}
