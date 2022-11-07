using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private RotateToMouse rotateToMouse;


    private void Init() {
        // 커서 숨김, 현재 위치에 고정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rotateToMouse = gameObject.GetComponent<RotateToMouse>();
    }

    private void Awake() {
        Init();
    }

    private void Update() {
        UpdateRotate(); // 마우스로 시점 회전
    }

    private void UpdateRotate() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }







}