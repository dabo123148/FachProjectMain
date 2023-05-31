using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void Load(int ID)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ID);
    }
}
