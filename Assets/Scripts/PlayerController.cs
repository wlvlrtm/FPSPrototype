using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private RotateToMouse rotateToMouse;
    private Movement movement;
    private Status status;
    private PlayerAnimatorController playerAnimatorController;
    private AudioSource audioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip audioClipWalk;
    [SerializeField] private AudioClip audioClipRun;


    private void Init() {
        // 커서 숨김, 현재 위치에 고정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        this.rotateToMouse = gameObject.GetComponent<RotateToMouse>();
        this.movement = gameObject.GetComponent<Movement>();
        this.status = gameObject.GetComponent<Status>();
        this.playerAnimatorController = gameObject.GetComponent<PlayerAnimatorController>();
        this.audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Awake() {
        Init();
    }

    private void Update() {
        UpdateRotate(); // 마우스로 시점 회전
        UpdateMove();   // 키보드로 이동
        UpdateJump();   // Spacebar 점프
    }

    private void UpdateRotate() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        this.rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    private void UpdateMove() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        if (horizontal != 0 || vertical != 0) {     // 이동 중일 때
            bool isRun = false;
            
            if (vertical > 0 && Input.GetAxis("Run") > 0) {     // 앞으로 움직일 때만 달리기 가능
                isRun = true;
            }

            this.movement.MoveSpeed = isRun == true ? this.status.RunSpeed : this.status.WalkSpeed;
            this.playerAnimatorController.MoveSpeed = isRun == true ? 1 : 0.5f;
            this.audioSource.clip = isRun == true ? this.audioClipRun : this.audioClipWalk;

            if (!this.audioSource.isPlaying) {
                this.audioSource.loop = true;
                this.audioSource.Play();
            }
        }
        else {
            this.movement.MoveSpeed = 0;
            this.playerAnimatorController.MoveSpeed = 0;

            if (this.audioSource.isPlaying) {
                this.audioSource.Stop();
            }
        }

        this.movement.MoveTo(new Vector3(horizontal, 0, vertical));
    }

    private void UpdateJump() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            this.movement.Jump();
            // animation
        }
    }
}