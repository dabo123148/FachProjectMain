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
    public float LoadingDelay = 0.21f;
    public override void FixedUpdate()
    {
        //Bewegungsziel auswählen
        if(Spieler != null)
        {
            //Spieler ist in unserer Agro range
            if (ReturningToAgroPunkt == true ||Vector3.Distance(Agropunkt, Spieler.transform.position) > FollowRange)
            {
                //Spieler ist zu weit von unserem agro punkt entfernt, wir wollen zurückgehen sonst werden wir gekitet
                ReturnToAgroPunkt();
            }
            else
            {
                float Spielerdistance = Vector3.Distance(transform.position, Spieler.transform.position);
                //Wir wollen zu einem bereicht gehen an dem wir den Gegner angreifen können, falls wir es nicht schon sind
                if (Spielerdistance < AttackRange)
                {
                    //Wir sind in attack Range und ein Ranged Attacker, wir wollen also nicht näher gehen, sondern uns eher am Rand der Angriff Range befinden
                    if (Spielerdistance> AttackRange-EvadeRange)
                    {
                        Target = transform.position;
                    }
                    else
                    {
                        //Wir bewegen uns vom Spieler weg, hier ist noch verbesserungspotential(es kann klappen muss aber nicht)
                        Target = Agropunkt;
                    }
                    Attack();
                }
                else
                {
                    //Wir wollen zum gegner gehen
                    Target = Spieler.transform.position;
                }
            }
        }
        else
        {
            //Kein Gegner in Range
            if (!ReturnedToAgroPunkt)
            {
                //Wir wurden gekitet und wollen zurückgehen
                ReturnToAgroPunkt();
                if (Vector3.Distance(Agropunkt, transform.position) < ZielDistance + 1)
                {
                    //Dies führt zu einer Verschiebung des Ausgangspunktes bei jedem engagement um einen kleinen wert, aber mit patrolling ist es vernachlässigbar
                    //wenn spieler sich den Aufwand macht gegner zu kiten mit dieser distanz, dann sein es ihm gegönnt
                    ReturnedToAgroPunkt = true;
                    //Wir haben die bewegung zum Patrol punkt abgebrochen, wir nehmen nun also wieder die Patol auf
                    //Hier könnte man noch einfügen das wir den patrol punkt zurücksetzen, ansonsten skippen wir den angesteuert vor agro
                    ReadyForPatrol = true;
                }
            }
            else
            {
                //Wir sind quasi in ausgangsposition -> wir können patrolieren
                Patrol();
            }
        }
        //Wir haben ein Ziel, nun wollen wir uns auf das Ziel zu bewegen
        if (Vector3.Distance(Target, transform.position) > ZielDistance)
        {
            MoveTo(Target);
        }
        else
        {
            GetComponent<Animator>().SetInteger("State", 0);
            GetComponent<Animator>().Play("Idle");
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
}
