using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScaling : MonoBehaviour
{
    public float width = 0;
    public float height = 0;
    public GameObject StartGame;
    public GameObject SetLevel;
    public GameObject SetLevelMessage;
    public GameObject SetHP;
    public GameObject SetHPMessage;
    public GameObject GameOverText;
    public GameObject HPLevelMessage;
    void Update()
    {
        if (width != Screen.width || height != Screen.height)
        {
            Rescale();
        }
    }
    void Rescale()
    {
        width = Screen.width;
        height = Screen.height;
        float size = height / 20;
        float currentheight = height - size/2;
        if (HPLevelMessage != null)
        {
            int Times = HPLevelMessage.GetComponent<UnityEngine.UI.Text>().text.Split('\n').Length;
            //We are in a boss or normal level scene
            HPLevelMessage.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, currentheight- size * (Times/2), 0);
            HPLevelMessage.GetComponent<RectTransform>().sizeDelta = new Vector3(Screen.width, size* Times, 0);
            currentheight -= size*Times;
        }
        else
        {
            currentheight -= size*2;
            GameOverText.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, currentheight, 0);
            GameOverText.GetComponent<RectTransform>().sizeDelta = new Vector3(Screen.width, size*2, 0);
            currentheight -= size * 2;
            SetHPMessage.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2- Screen.width / 8, currentheight, 0);
            SetHPMessage.GetComponent<RectTransform>().sizeDelta = new Vector3(Screen.width / 8, size, 0);
            SetHP.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2 + Screen.width / 8, currentheight, 0);
            SetHP.GetComponent<RectTransform>().sizeDelta = new Vector3(Screen.width / 8, size, 0);
            currentheight -= size * 2;
            SetLevelMessage.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2 - Screen.width / 8, currentheight, 0);
            SetLevelMessage.GetComponent<RectTransform>().sizeDelta = new Vector3(Screen.width / 8, size, 0);
            SetLevel.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2 + Screen.width / 8, currentheight, 0);
            SetLevel.GetComponent<RectTransform>().sizeDelta = new Vector3(Screen.width / 8, size, 0);
            currentheight -= size * 2;
            StartGame.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, currentheight, 0);
            StartGame.GetComponent<RectTransform>().sizeDelta = new Vector3(Screen.width/2, size, 0);
            currentheight -= size * 2;
            //We are in the main menu

        }
    }
}
