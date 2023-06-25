using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gegner : MonoBehaviour
{
    /// <summary>
    /// Leben
    /// </summary>
    public int HP = 5;
    /// <summary>
    /// Bewegungstempo
    /// </summary>
    public float MovementSpeed = 0.1f;
    /// <summary>
    /// Range ab der wir den Spieler angreifen
    /// </summary>
    public float AgroRange = 30;
    /// <summary>
    /// Sobald der Spieler mehr als Follow range von agropunkt weg ist, returnen wir zu agropunkt
    /// </summary>
    public float FollowRange = 30;
    /// <summary>
    /// Die location an der der Gegner war als der Spieler in AgroRange gegangen ist, Gegner bewegt sich hierhin zurück sobald spieler aus followrange ist
    /// </summary>
    public Vector3 Agropunkt;
    /// <summary>
    /// Wen wir jedes mal den Agro punkt überschreiben wenn der Spieler in Range kommt, werden wir von unserem ursprungsort entfernt, wir wollen den also nicht immer überschreiben
    /// </summary>
    public bool ReturnedToAgroPunkt = true;
    /// <summary>
    /// Punkte zwischen denen Gegner hin und her läuft(falls wir so etwas machen wollen), startet immer wieder auf Index0
    /// </summary>
    public Vector3[] PatrolPoints = new Vector3[0];
    /// <summary>
    /// Falls wir Patroling drinnen haben, so gibt dies an, an welchem Punkt wir uns gerade befinden
    /// </summary>
    private int CurrentPatrolposition = 0;
    /// <summary>
    /// Indicates if idletime is over
    /// </summary>
    public bool ReadyForPatrol = true;
    /// <summary>
    /// Falls wir Patroling drinnen haben, dann läuft der Gegner zu einem Punkt und wartet dort IdleTime bevor er zum nächsten Punkt läuft
    /// </summary>
    public float IdleTime = 5;
    /// <summary>
    /// TimeBetwennAttacks
    /// </summary>
    public float AttackIdleTime = 2;
    /// <summary>
    /// Idicates if attack is off cooldown
    /// </summary>
    public bool ReadyToAttack = true;
    /// <summary>
    /// Wir müssen wissen wo der spieler ist um uns zu ihm bewegen zu können
    /// </summary>
    public GameObject Spieler;
    /// <summary>
    /// Wir bewegen uns nicht näher als Zieldistance an target, da wir sonst in z.b. Spieler laufen können
    /// </summary>
    public float ZielDistance = 1.5f;
    /// <summary>
    /// Die Koordinaten an die der Gegner im Moment läuft, läuft nicht wenn in Zieldistance vom Target, muss beim spawnen auf gespawnte Coordinaten gesetzt werden, sonst versuchter er nach 0/0 zu laufen
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
    /// <summary>
    /// Speed of speels and arrows and attack animations
    /// </summary>
    public float AttackSpeed = 1;
    /// <summary>
    /// Nich alle Gegner grafiken sind gleich, manche haben eine höhen verschiebung
    /// </summary>
    public float SpawnHight = 1.51f;
    public void Initilize()
    {
        Target = transform.position;
        GetComponent<SphereCollider>().radius = AgroRange/transform.localScale.y;
        SpezilizedInitilize();
    }
    /// <summary>
    ///     Archer wollen z.b. nicht so naher am spieler sein, deshalb ist Zieldistanz hier in der vererbung verändert, andere Gegner können vl ähnliche changes gebrauchen
    /// </summary>
    public virtual void SpezilizedInitilize()
    {

    }
    public bool AttackPossible()
    {
        if (gameObject.GetComponent<Gegner>().AttackReady() && Spieler!=null && Vector3.Distance(transform.position,Spieler.transform.position)<AttackRange) return true;
        return false;
    }
    public void Attack()
    {
        if (ReadyToAttack)
        {
            GetComponent<Animator>().SetFloat("AttackSpeedMultipler", AttackSpeed);
            Target = transform.position;
            StartCoroutine(AttackIdle());
            SpezilizedAttack();
        }
    }
    public virtual void SpezilizedAttack()
    {

    }
    public void SetAgroPunkt()
    {
        Agropunkt = transform.position;
    }
    public void ReturnToAgroPunkt()
    {
        Target = Agropunkt;
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
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Spieler = other.gameObject;
        }
        if (other.gameObject.GetComponent<Bullet>() && other.gameObject.GetComponent<Bullet>().PlayerBullet && Vector2.Distance(new Vector3(other.gameObject.transform.position.x,other.gameObject.transform.position.z),new Vector2(transform.position.x,transform.position.z))<=1.5f)
        {
            HP--;
            Destroy(other.gameObject);
            if (HP == 0)
            {
                if (GameObject.FindObjectOfType<LevelManagement>()) GameObject.FindObjectOfType<LevelManagement>().OnGegnerDeath();
                Destroy(gameObject);
            }
        }
    }
    //Spieler ist aus agro range draußen, wir ignorieren AgroRange > FollowRange
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Spieler = null;
        }
    }
    public void FixedUpdate()
    {
        //Wir haben ein Ziel, nun wollen wir uns auf das Ziel zu bewegen
        if (Vector3.Distance(Target, transform.position) > ZielDistance)
        {
            MoveTo(Target);
        }
        else
        {
            if (GetComponent<Animator>().GetInteger("State") != 2)
            {
                GetComponent<Animator>().SetInteger("State", 0);
                GetComponent<Animator>().Play("Idle");
            }
        }
    }
    public void MarkAgroPointAsToVisit()
    {
        ReturnedToAgroPunkt = false;
    } 
    public bool PlayerInAgroRange()
    {
        if (Spieler == null)
            return false;
        return true;
    }
    public bool PlayerInFollowRange()
    {
        if (Spieler == null || Vector3.Distance(Agropunkt, Spieler.transform.position) > FollowRange)
            return false;
        return true;
    }
    public bool AttackReady()
    {
        return ReadyToAttack;
    }
    public void Patrol()
    {
        if (PatrolPoints.Length > 0)
        {
            if (Vector3.Distance(transform.position, PatrolPoints[CurrentPatrolposition]) < ZielDistance)
            {
                StartCoroutine(PatrolIdle());
                CurrentPatrolposition++;
                if (CurrentPatrolposition > PatrolPoints.Length - 1) CurrentPatrolposition = 0;
            }
            if (ReadyForPatrol)
            {
                ReadyForPatrol = false;
                Target = PatrolPoints[CurrentPatrolposition];
            }
        }
    }
    public void MoveTo(Vector3 Direction)
    {
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
            //Debug.Log("Wir haben einen path " + transform.position);
            //gameObject.name = "Gegner mit Path";
        }
        else
        {
            gameObject.name = "No Path Gegner";
            Ziel = transform.position;
        }
        transform.LookAt(new Vector3(Ziel.x, transform.position.y, Ziel.z));
        if (Vector3.Distance(Ziel, transform.position) > ZielDistance)
            transform.Translate(Vector3.forward * MovementSpeed);
        if (GetComponent<Animator>().GetInteger("State")==0)
        {
            GetComponent<Animator>().SetInteger("State",1);
            GetComponent<Animator>().Play("Walk");
        }
    }
    public IEnumerator PatrolIdle()
    {
        GetComponent<Animator>().SetInteger("State", 0);
        yield return new WaitForSeconds(IdleTime);
        ReadyForPatrol = true;
    }
    public IEnumerator AttackIdle()
    {
        Target = transform.position;
        ReadyToAttack = false;
        GetComponent<Animator>().Play("Shoot");
        GetComponent<Animator>().SetInteger("State", 2);
        yield return new WaitForSeconds(AttackIdleTime);
        ReadyToAttack = true;
    }
    public void VoidAnimationFunctionEvent()
    {

    }

}
