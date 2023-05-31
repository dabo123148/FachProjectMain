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
    public KeyCode Shoot = KeyCode.Space;
    public float RotationSpeedY = 10;
    public bool ShootReady = true;
    public float ShootDelay = 0.6f;
    public float BulletSpeed = 1;
    public float BulletLifeTime = 5;
    public Transform Bullet;
    public int HP = 5;
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
        if (ShootReady && Input.GetKey(Shoot))
        {
            StartCoroutine(ShootIdle());
            SpawnBullet();
        }
        //Rotation Oben/Untern wird in Kamera gehandhabt die im Kind Object ist
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + Input.GetAxis("Mouse X") * RotationSpeedY, 0);
    }
    IEnumerator ShootIdle()
    {
        ShootReady = false;
        yield return new WaitForSeconds(ShootDelay);
        ShootReady = true;
    }
    public void SpawnBullet()
    {
        GameObject obj = GameObject.Instantiate(Bullet.gameObject);
        obj.GetComponent<Bullet>().Initilize(true, transform.forward, BulletSpeed, BulletLifeTime);
        obj.transform.position = transform.position;// + new Vector3(-0.6f,0.8f,1);
    }
    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
