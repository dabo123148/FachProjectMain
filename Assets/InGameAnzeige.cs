using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameAnzeige : MonoBehaviour
{
    public GameObject Spieler;
    public LevelManagement Management;
    public bool BossLevel = false;
    // Start is called before the first frame update
    void Start()
    {
        Management = GameObject.FindObjectOfType<LevelManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        string Text = "";
        if (BossLevel == false) {
            Text = "Kill " + (Management.GegnerRemaining - Management.GegnerToKill()+1) + " enemies to get to the next level";
            if (Management.GegnerRemaining < 100)
            {
                float MinDistance = 1000;
                foreach(Gegner g in GameObject.FindObjectsOfType<Gegner>())
                {
                    MinDistance = (float)Mathf.Round(Mathf.Min(MinDistance, Vector3.Distance(g.transform.position, Spieler.transform.position)));
                }
                Text += "\nNächster Gegner in " + MinDistance + " Metern";
            }
            Text += "\nLevel: " + Management.CurrentLevel + "\nHP: " + Management.HP;
        }
        else
        {
            
            Text = "Kill all enemies to reach the next level";
            Text += "\nLevel: " + Management.CurrentLevel + "\nHP: " + Management.HP;
        }
        GetComponent<UnityEngine.UI.Text>().text = Text;
    }
}
