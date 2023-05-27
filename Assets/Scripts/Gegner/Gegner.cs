using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gegner : MonoBehaviour
{
    public int HP = 5;
    public float MovementSpeed = 1;
    /// <summary>
    /// Range ab der wir den Spieler angreifen
    /// </summary>
    public float AgroRange = 30;
    /// <summary>
    /// Sobald der Spieler mehr als Follow range von agropunkt weg ist, returnen wir zu agropunkt
    /// </summary>
    public float FollowRange = 30;
    /// <summary>
    /// Die location an der der Gegner war als der Spieler in AgroRange gegangen ist, Gegner bewegt sich hierhin zur�ck sobald spieler aus followrange ist
    /// </summary>
    public Vector3 Agropunkt;
    /// <summary>
    /// Wen wir jedes mal den Agro punkt �berschreiben wenn der Spieler in Range kommt, werden wir von unserem ursprungsort entfernt, wir wollen den also nicht immer �berschreiben
    /// </summary>
    public bool ReturnedToAgroPunkt = true;
    /// <summary>
    /// Wir wollen verhindern, das wir in dem Punkt stecken bleiben an dem wir zwischen follow und return hin und herschwanken, wir laufen also zur�ck solange dies true ist
    /// </summary>
    public bool ReturningToAgroPunkt = false;
    /// <summary>
    /// Punkte zwischen denen Gegner hin und her l�uft(falls wir so etwas machen wollen), startet immer wieder auf Index0
    /// </summary>
    public Vector3[] PatrolPoints = new Vector3[0];
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
    public GameObject Spieler;
    /// <summary>
    /// Wir bewegen uns nicht n�her als Zieldistance an target, da wir sonst in z.b. Spieler laufen k�nnen
    /// </summary>
    public float ZielDistance = 1.5f;
    /// <summary>
    /// Die Koordinaten an die der Gegner im Moment l�uft, l�uft nicht wenn in Zieldistance vom Target, muss beim spawnen auf gespawnte Coordinaten gesetzt werden, sonst versuchter er nach 0/0 zu laufen
    /// </summary>
    public Vector3 Target;
    /// <summary>
    /// Liste anderer Gegner die mit diesem Gegner zusammen gespawned sind, dient dazu das falls ein Gegner aus gruppe agro hat andere mitzunehmen
    /// </summary>
    public GameObject[] Group = new GameObject[0];
    /// <summary>
    /// Distanz von der der Gegner angreifen kann
    /// </summary>
    public float AttackRange = 3f;
    public void Initilize()
    {
        GetComponent<SphereCollider>().radius = AgroRange;
        SpezilizedInitilize();
    }
    /// <summary>
    ///     Archer wollen z.b. nicht so naher am spieler sein, deshalb ist Zieldistanz hier in der vererbung ver�ndert, andere Gegner k�nnen vl �hnliche changes gebrauchen
    /// </summary>
    public virtual void SpezilizedInitilize()
    {

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
        if (other.gameObject.GetComponent<PlayerController>())
        {
            if (ReturnedToAgroPunkt)
            {
                Agropunkt = transform.position;
                ReturnedToAgroPunkt = false;
            }
            Spieler = other.gameObject;
        }
        //Pr�fen ob auch mit 2 Kollider Kugeln gehandhabt werden k�nnen
    }
    //Spieler ist aus agro range drau�en, wir ignorieren AgroRange > FollowRange
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Spieler = null;
        }
    }
    /// <summary>
    /// Entscheidet target f�r movement, m�ssen hier in RangedGegner und MeleeGegner anders bewegen, Patrolling ist gleich
    /// </summary>
    public virtual void FixedUpdate()
    {
        //movement ist hier
    }
    public void MoveTo(Vector3 Direction)
    {
        //Debug.LogWarning("MoveTo");
        Vector3 RealZiel = Direction;
        NavMeshPath path = new NavMeshPath();
        Vector3 pathcalculatedat = transform.position;
        NavMesh.CalculatePath(transform.position, RealZiel, NavMesh.AllAreas, path);
        Vector3 Ziel;
        if (path.corners.Length > 0)
        {
            Ziel = path.corners[0];
            int num = 0;
            while (path.corners.Length > num && Vector3.Distance(Ziel, transform.position) < ZielDistance)
            {
                Ziel = path.corners[num];
                num++;
            }
        }
        else
        {
            Debug.LogError("No path found");
            Ziel = transform.position;
        }
        transform.LookAt(new Vector3(Ziel.x, transform.position.y, Ziel.z));
        if (Vector3.Distance(Ziel, transform.position) > ZielDistance)
            transform.Translate(Vector3.forward * MovementSpeed);
    }
}
