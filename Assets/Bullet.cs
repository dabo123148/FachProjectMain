using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool PlayerBullet = false;
    public Vector3 direction;
    public float Movementspeed = 1;
    public float LifeTime = 5;
    private bool initilized = false;
    public void Initilize(bool pPlayerBullet, Vector3 pDirection, float pMovementspeed, float pLifeTime)
    {
        PlayerBullet = pPlayerBullet;
        direction = pDirection;
        Movementspeed = pMovementspeed;
        Destroy(gameObject, pLifeTime);
        initilized = true;
    }
    private void FixedUpdate()
    {
        transform.Translate(direction*Movementspeed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (initilized)
        {
            if (other.GetComponent<PlayerController>() && PlayerBullet == false)
            {
                //We hit the Player
                other.GetComponent<PlayerController>().TakeDamage(1);
            }
            else
            {
                if (!other.GetComponent<PlayerController>() && !other.GetComponent<Gegner>() && !other.GetComponent<Bullet>())
                {
                    //We hit a wall
                    Destroy(gameObject);
                }
            }
        }
    }
}
