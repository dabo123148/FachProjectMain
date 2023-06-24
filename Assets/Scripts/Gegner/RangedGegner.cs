using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedGegner : Gegner
{
    /// <summary>
    /// Wir versuchen nicht näher als Attackrange-Evaderange an spieler zu gehen sonst entfernen wir uns von spieler um am rand der attackrange zu bleiben
    /// </summary>
    public float EvadeRange = 3;
    /// <summary>
    /// Spell oder Arraw mit denen der RangedGegner angreift
    /// </summary>
    public Transform RangedAttack;
    /// <summary>
    /// Koordinaten wo rangedattack spawned, z.b. bogen oder zauberstab
    /// </summary>
    public Vector3 AttackOffset;
    /// <summary>
    /// Time between start of Animation and Attack spawn
    /// </summary>
    public float LoadingDelay = 0.21f;
    public void AttackRangeManagement()
    {
        float Spielerdistance = Vector3.Distance(transform.position, Spieler.transform.position);
        //Wir wollen zu einem bereicht gehen an dem wir den Gegner angreifen können, falls wir es nicht schon sind
        if (Spielerdistance < AttackRange)
        {
            //Wir sind in attack Range und ein Ranged Attacker, wir wollen also nicht näher gehen, sondern uns eher am Rand der Angriff Range befinden
            if (Spielerdistance > AttackRange - EvadeRange)
            {
                Target = transform.position;
            }
            else
            {
                //Wir bewegen uns vom Spieler weg, hier ist noch verbesserungspotential(es kann klappen muss aber nicht)
                Target = Agropunkt;
            }
        }
        else
        {
            //Wir wollen zum gegner gehen
            Target = Spieler.transform.position;
        }
    }

    public override void SpezilizedInitilize()
    {
        ZielDistance = 3;
        AttackRange = AgroRange * 0.75f;
        GetComponent<Animator>().SetFloat("AttackSpeedMultipler", AttackSpeed);
    }
    public override void SpezilizedAttack()
    {
        StartCoroutine(AttackDelay());
    }
    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(LoadingDelay);
        transform.LookAt(new Vector3(Spieler.transform.position.x, transform.position.y, Spieler.transform.position.z));
        GameObject obj = GameObject.Instantiate(RangedAttack.gameObject);
        obj.GetComponent<Bullet>().Initilize(false, transform.forward, AttackSpeed, AttackRange / AttackSpeed);
        obj.transform.position = transform.position + AttackOffset;
    }
    public void PlayerInRangeTest()
    {
        Debug.Log("PlayerInRangeTest");
    }
    public void PlayerNotInRangeTest()
    {
        Debug.Log("PlayerNotInRangeTest");
    }
    public void AttackTest()
    {
        Debug.Log("AttackTest");
    }
}
