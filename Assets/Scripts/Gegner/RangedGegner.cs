using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedGegner : Gegner
{
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
                //Wir wollen zu einem bereicht gehen an dem wir den Gegner angreifen können, falls wir es nicht schon sind
                if (Vector3.Distance(transform.position, Spieler.transform.position) < AttackRange)
                {
                    //Wir sind in attack Range und ein Ranged Attacker, wir wollen also nicht näher gehen, sondern uns eher entfernen,
                    Target = transform.position;
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
                }
            }
            else
            {
                //Wir sind quasi in ausgangsposition -> wir können patrolieren
            }
        }
        //Wir haben ein Ziel, nun wollen wir uns auf das Ziel zu bewegen
        MoveTo(Target);
    }
    private void ReturnToAgroPunkt()
    {
        Target = Agropunkt;
        if (Vector3.Distance(Agropunkt, transform.position) < ZielDistance + 1)
        {
            ReturningToAgroPunkt = false;
        }
    }
    public override void SpezilizedInitilize()
    {
        ZielDistance = 3;
        AttackRange = AgroRange * 0.75f;
    }
    private void Attack()
    {

    }
}
