using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gegner : MonoBehaviour
{
    public int HP = 5;
    public float MovementSpeed = 1;
    /// <summary>
    /// Range ab der wir den Spieler angreifen
    /// </summary>
    public float AgroRange = 10;
    /// <summary>
    /// Sobald der Spieler mehr als Follow range von agropunkt weg ist, returnen wir zu agropunkt
    /// </summary>
    public float FollowRange = 30;
    /// <summary>
    /// Die location an der der Gegner war als der Spieler in AgroRange gegangen ist, Gegner bewegt sich hierhin zurück sobald spieler aus followrange ist
    /// </summary>
    private Vector2 Agropunkt;
    /// <summary>
    /// Punkte zwischen denen Gegner hin und her läuft(falls wir so etwas machen wollen), startet immer wieder auf Index0
    /// </summary>
    public Vector2[] PatrolPoints = new Vector2[0];
    /// <summary>
    /// Falls wir Patroling drinnen haben, so gibt dies an, an welchem Punkt wir uns gerade befinden
    /// </summary>
    private int CurrentPatrolposition = 0;
    /// <summary>
    /// Falls wir Patroling drinnen haben, dann läuft der Gegner zu einem Punkt und wartet dort IdleTime bevor er zum nächsten Punkt läuft
    /// </summary>
    public int IdleTime = 5;
    /// <summary>
    /// Wir müssen wissen wo der spieler ist um uns zu ihm bewegen zu können
    /// </summary>
    private GameObject Spieler;
    /// <summary>
    /// Wir bewegen uns nicht näher als Zieldistance an target, da wir sonst in z.b. Spieler laufen können
    /// </summary>
    public float ZielDistance = 1.5f;
    /// <summary>
    /// Die Koordinaten an die der Gegner im Moment läuft, läuft nicht wenn in Zieldistance vom Target, muss beim spawnen auf gespawnte Coordinaten gesetzt werden, sonst versuchter er nach 0/0 zu laufen
    /// </summary>
    public Vector2 Target;
    /// <summary>
    /// Liste anderer Gegner die mit diesem Gegner zusammen gespawned sind, dient dazu das falls ein Gegner aus gruppe agro hat andere mitzunehmen
    /// </summary>
    private GameObject[] Group = new GameObject[0];
    public void Initilize(GameObject pSpieler)
    {
        Spieler = pSpieler;
    }
    public void AddGegnerToGroup(GameObject obj)
    {
        System.Array.Resize(ref Group, Group.Length + 1);
        Group[Group.Length - 1] = obj;
    }
    /// <summary>
    /// Prüft ob spieler in agro range ist, Kugeln müssen anders gehandthabt werden, oder vl gehen 2 collider und beide triggern braucht prüfung
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Spieler)
        {

        }
        //Prüfen ob auch mit 2 Kollider Kugeln gehandhabt werden können
    }
    /// <summary>
    /// Entscheidet target für movement, müssen hier in RangedGegner und MeleeGegner anders bewegen, Patrolling ist gleich
    /// </summary>
    public virtual void Update()
    {
        //movement ist hier
    }
}
