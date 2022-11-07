using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour {
    [SerializeField] private float rotCamXAxisSpeed = 5; // X축 회전 속도
    [SerializeField] private float rotCamYAxisSpeed = 5; // Y축 회전 속도

    private float limitMinX = -80;  // X축 회전 최소 범위 (아래)
    private float limitMaxX = 50;   // X축 회전 최대 범위 (위)
    private float eulerAngleX;
    private float eulerAngleY;


    public void UpdateRotate(float mouseX, float mouseY) {
        this.eulerAngleY += mouseX * this.rotCamYAxisSpeed;     // 시점 좌/우; 카메라 y축 기준 회전
        this.eulerAngleX -= mouseY * this.rotCamXAxisSpeed;     // 시점 위/아래; 카메라 x축 기준 회전

        this.eulerAngleX = ClampAngle(this.eulerAngleX, this.limitMinX, this.limitMaxX);
        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    private float ClampAngle(float angle, float min, float max) {
        if (angle < -360) {
            angle += 360;
        }
        if (angle > 360) {
            angle -= 360;
        }

        return Mathf.Clamp(angle, min, max);
    }
}