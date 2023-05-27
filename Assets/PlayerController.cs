using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Movementspeed = 100;
    public KeyCode MoveForward = KeyCode.W;
    public KeyCode MoveBack = KeyCode.S;
    public KeyCode MoveLeft = KeyCode.A;
    public KeyCode MoveRight = KeyCode.D;
    public float RotationSpeedY = 10;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void FixedUpdate()
    {
        if(Input.GetKey(MoveForward))
            GetComponent<Rigidbody>().AddForce(transform.forward*Movementspeed);
        if (Input.GetKey(MoveBack))
            GetComponent<Rigidbody>().AddForce(transform.forward*-1 * Movementspeed);
        if (Input.GetKey(MoveRight))
            GetComponent<Rigidbody>().AddForce(transform.right * Movementspeed);
        if (Input.GetKey(MoveLeft))
            GetComponent<Rigidbody>().AddForce(transform.right * -1 * Movementspeed);
        //Rotation Oben/Untern wird in Kamera gehandhabt die im Kind Object ist
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + Input.GetAxis("Mouse X") * RotationSpeedY, 0);
    }
}
