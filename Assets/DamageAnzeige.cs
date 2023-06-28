using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAnzeige : MonoBehaviour
{
    public string Text = "";
    public int Lines = 5;
    public void AddDamageMessage(int HPleft)
    {
        Text += System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second + ":";
        if (HPleft == 0)
        {
            Text += "Killed enemy\n";
        }
        else { 

            Text += "Hit enemy has " + HPleft + " HP left\n";
        }
        if (Text.Split('\n').Length > Lines)
        {
            string newText = "";
            string[] textsplit = Text.Split('\n');
            for (int A = 1; A < textsplit.Length-1; A++)
            {
                newText += textsplit[A] + "\n";
            }
            Text = newText;
        }
        GetComponent<UnityEngine.UI.Text>().text = Text;
    }
}
