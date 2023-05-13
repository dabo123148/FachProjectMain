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
    /// Die location an der der Gegner war als der Spieler in AgroRange gegangen ist, Gegner bewegt sich hierhin zur�ck sobald spieler aus followrange ist
    /// </summary>
    private Vector2 Agropunkt;
    /// <summary>
    /// Punkte zwischen denen Gegner hin und her l�uft(falls wir so etwas machen wollen), startet immer wieder auf Index0
    /// </summary>
    public Vector2[] PatrolPoints = new Vector2[0];
    /// <summary>
    /// Falls wir Patroling drinnen haben, so gibt dies an, an welchem Punkt wir uns gerade befinden
    /// </summary>
    private int CurrentPatrolposition = 0;
    /// <summary>
    /// Falls wir Patroling drinnen haben, dann l�uft der Gegner zu einem Punkt und wartet dort IdleTime bevor er zum n�chsten Punkt l�uft
    /// </summary>
    public int IdleTime = 5;
    /// <summary>
    /// Wir m�ssen wissen wo der spieler ist um uns zu ihm bewegen zu k�nnen
    /// </summary>
    private GameObject Spieler;
    /// <summary>
    /// Wir bewegen uns nicht n�her als Zieldistance an target, da wir sonst in z.b. Spieler laufen k�nnen
    /// </summary>
    public float ZielDistance = 1.5f;
    /// <summary>
    /// Die Koordinaten an die der Gegner im Moment l�uft, l�uft nicht wenn in Zieldistance vom Target, muss beim spawnen auf gespawnte Coordinaten gesetzt werden, sonst versuchter er nach 0/0 zu laufen
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
    /// Pr�ft ob spieler in agro range ist, Kugeln m�ssen anders gehandthabt werden, oder vl gehen 2 collider und beide triggern braucht pr�fung
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Spieler)
        {

        }
        //Pr�fen ob auch mit 2 Kollider Kugeln gehandhabt werden k�nnen
    }
    /// <summary>
    /// Entscheidet target f�r movement, m�ssen hier in RangedGegner und MeleeGegner anders bewegen, Patrolling ist gleich
    /// </summary>
    public virtual void Update()
    {
        //movement ist hier
    }
}
