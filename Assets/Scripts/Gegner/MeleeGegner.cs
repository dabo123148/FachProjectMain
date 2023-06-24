using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGegner : Gegner
{
    public bool AttackRunning = false;
    public bool CanMove()
    {
        return !AttackRunning;
    }
    public void MoveToPlayer()
    {

        if (Vector3.Distance(transform.position, Spieler.transform.position) > AttackRange)
        {
            Debug.Log("MoveToPlayer");
            Target = Spieler.transform.position;
        }
        else
        {
            //Already at the player
            Target = transform.position;
        }
    }
    public override void SpezilizedAttack()
    {
        StartCoroutine(AttackProcess());
    }
    IEnumerator AttackProcess()
    {
        AttackRunning = true;
        yield return new WaitForSeconds(1/AttackSpeed);
        AttackRunning = false;
        GetComponent<Animator>().SetInteger("State", 0);
        GetComponent<Animator>().Play("Idle");
        if (Vector3.Distance(transform.position, Spieler.transform.position) <= AttackRange)
        {
            Spieler.GetComponent<PlayerController>().TakeDamage(1);
        }
    }

}
