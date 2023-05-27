using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedGegner : Gegner
{
    public float EvadeRange = 3;
    public override void FixedUpdate()
    {
        //Bewegungsziel ausw�hlen
        if(Spieler != null)
        {
            //Spieler ist in unserer Agro range
            if (ReturningToAgroPunkt == true ||Vector3.Distance(Agropunkt, Spieler.transform.position) > FollowRange)
            {
                //Spieler ist zu weit von unserem agro punkt entfernt, wir wollen zur�ckgehen sonst werden wir gekitet
                ReturnToAgroPunkt();
            }
            else
            {
                float Spielerdistance = Vector3.Distance(transform.position, Spieler.transform.position);
                //Wir wollen zu einem bereicht gehen an dem wir den Gegner angreifen k�nnen, falls wir es nicht schon sind
                if (Spielerdistance < AttackRange)
                {
                    //Wir sind in attack Range und ein Ranged Attacker, wir wollen also nicht n�her gehen, sondern uns eher am Rand der Angriff Range befinden
                    Debug.Log(Spielerdistance + " - " + (AttackRange - EvadeRange));
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
                //Wir wurden gekitet und wollen zur�ckgehen
                ReturnToAgroPunkt();
                if (Vector3.Distance(Agropunkt, transform.position) < ZielDistance + 1)
                {
                    //Dies f�hrt zu einer Verschiebung des Ausgangspunktes bei jedem engagement um einen kleinen wert, aber mit patrolling ist es vernachl�ssigbar
                    //wenn spieler sich den Aufwand macht gegner zu kiten mit dieser distanz, dann sein es ihm geg�nnt
                    ReturnedToAgroPunkt = true;
                    //Wir haben die bewegung zum Patrol punkt abgebrochen, wir nehmen nun also wieder die Patol auf
                    //Hier k�nnte man noch einf�gen das wir den patrol punkt zur�cksetzen, ansonsten skippen wir den angesteuert vor agro
                    ReadyForPatrol = true;
                }
            }
            else
            {
                //Wir sind quasi in ausgangsposition -> wir k�nnen patrolieren
                Patrol();
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
