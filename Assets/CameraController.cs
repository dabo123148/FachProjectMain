using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float RotationSpeedX = 10;
    public float MaxDownAngle = 45;
    public float MaxUpAngle = -45;
    void FixedUpdate()
    {
        float NewXAngle = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * RotationSpeedX;
        if (NewXAngle > 180) NewXAngle -= 360;
        if (NewXAngle < MaxUpAngle) NewXAngle = MaxUpAngle;
        if (NewXAngle > MaxDownAngle) NewXAngle = MaxDownAngle;
        transform.localEulerAngles = new Vector3(NewXAngle, 0, 0);
    }
}
