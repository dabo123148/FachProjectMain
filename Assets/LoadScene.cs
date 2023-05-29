using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public void Load(int ID)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ID);
    }
}
