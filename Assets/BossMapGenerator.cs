using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMapGenerator : MonoBehaviour
{
    public GameObject Player;
    public float MinSpawnDistancePlayer = 15;
    public float MinSpawnDistanceGegner = 3;
    public int GegnerAnzahl = 5;
    public Transform[] Gegner;
    public Transform[] Boss;
    public float ArenaSize = 50;
    private List<Vector2> spawnedGegner = new List<Vector2>();
    // Start is called before the first frame update
    public void SpawnLevel(int Level)
    {
        SpawnGegner(((Level-1)/5) * GegnerAnzahl);
    }
    public void SpawnGegner(int anzahl)
    {
        int FailTimeout = 0;
        while (anzahl > 0)
        {
            Vector2 pos = new Vector2(Random.Range(-ArenaSize / 2, ArenaSize / 2), Random.Range(-ArenaSize / 2, ArenaSize / 2));
            if (PositionAvalible(pos))
            {
                SpawnGegner(pos);
                anzahl--;
                FailTimeout = 0;
            }
            else
            {
                FailTimeout++;
            }
            if(FailTimeout > 100)
            {
                Debug.Log("FAILED TO OFTEN TO FIND A SPAWNPOINT");
                anzahl = 0;
            }
        }
    }
    private bool PositionAvalible(Vector2 pos)
    {
        if (Vector2.Distance(new Vector2(Player.transform.position.x, Player.transform.position.z), pos) < MinSpawnDistancePlayer) return false;
        foreach(Vector2 g in spawnedGegner)
        {
            if (Vector2.Distance(g, pos) < MinSpawnDistanceGegner) return false;
        }
        return true;
    }
    private void SpawnGegner(Vector2 pos)
    {
        spawnedGegner.Add(pos);
        GameObject obj = GameObject.Instantiate(Gegner[Random.Range(0, Gegner.Length)].gameObject);
        obj.transform.position = new Vector3(pos.x, obj.GetComponent<Gegner>().SpawnHight, pos.y);
        obj.GetComponent<Gegner>().AgroRange = 500;
        obj.GetComponent<Gegner>().FollowRange = 500;
        obj.GetComponent<Gegner>().Initilize();
    }
}
