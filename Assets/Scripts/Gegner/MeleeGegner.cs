using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGegner : Gegner
{
    public bool AttackRunning = false;
    public override void FixedUpdate()
    {
        //Gegner kann sich nicht w�hrend angriffes bewegen
        if (AttackRunning) return;
        //Bewegungsziel ausw�hlen
        if (Spieler != null)
        {
            //Spieler ist in unserer Agro range
            if (ReturningToAgroPunkt == true || Vector3.Distance(Agropunkt, Spieler.transform.position) > FollowRange)
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
    public override void SpezilizedAttack()
    {
        StartCoroutine(AttackProcess());
    }
    IEnumerator AttackProcess()
    {
        AttackRunning = true;
        yield return new WaitForSeconds(AttackSpeed);
        AttackRunning = false;
        if (Vector3.Distance(transform.position, Spieler.transform.position) <= AttackRange)
        {
            Spieler.GetComponent<PlayerController>().TakeDamage(1);
        }
    }

}
