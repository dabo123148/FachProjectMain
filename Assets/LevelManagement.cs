using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagement : MonoBehaviour
{
    public int CurrentLevel = 0;
    public int HP = 10;
    public int GegnerRemaining = 0;
    public int GegnerStartAmount = 0;
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void StartGame()
    {
        int level = 1;
        try
        {
            level = int.Parse(GameObject.Find("Level").GetComponent<UnityEngine.UI.InputField>().text);
        }catch(Exception ex)
        {

        }
        if(level < 1 || level >= 100)
        {
            level = 1;
        }
        CurrentLevel = level-1;
        GegnerRemaining = 0;
        GegnerStartAmount = 10;
        int HPinput = 10;
        try
        {
            HPinput = int.Parse(GameObject.Find("HP").GetComponent<UnityEngine.UI.InputField>().text);
        }
        catch (Exception ex)
        {

        }
        if (HPinput < 1 || HPinput >= 1000)
        {
            HPinput = 10;
        }
        HP = HPinput;
        OnGegnerDeath();
    }
    public void OnLevelWasLoaded(int level)
    {
        //Died, so we are in gameover screen, we want to delete our old data
        if (CurrentLevel!=0 && FindObjectsOfType<LevelManagement>().Length > 1) Destroy(gameObject);
        CurrentLevel++;
        if (GameObject.FindObjectOfType<MapGenerator>())
        {
            GameObject.FindObjectOfType<MapGenerator>().SpawnLevel(CurrentLevel);
        }
        if (GameObject.FindObjectOfType<BossMapGenerator>())
        {
            GameObject.FindObjectOfType<BossMapGenerator>().SpawnLevel(CurrentLevel);
        }
        if (GameObject.FindObjectOfType<PlayerController>())
        {
            GameObject.FindObjectOfType<PlayerController>().HP = HP;
        }
        GegnerStartAmount = FindObjectsOfType<Gegner>().Length;
        GegnerRemaining = GegnerStartAmount;
    }
    public void OnGegnerDeath()
    {
        GegnerRemaining--;
        if (GegnerRemaining == 0 || !GameObject.FindObjectOfType<BossMapGenerator>())
        {
            if (GegnerRemaining < GegnerToKill())
            {
                if (CurrentLevel != 0 && CurrentLevel % 5 == 0)
                {
                    //boss
                    UnityEngine.SceneManagement.SceneManager.LoadScene(3);
                }
                else
                {
                    //NextNormalLevel
                    UnityEngine.SceneManagement.SceneManager.LoadScene(2);
                }
            }
        }
    }
    public int GegnerToKill()
    {
        return Mathf.FloorToInt(GegnerStartAmount / 2);
    }
    public void OnPlayerDamage()
    {
        HP--;
    }
}
