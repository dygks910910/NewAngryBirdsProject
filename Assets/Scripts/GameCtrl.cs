using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameCtrl : MonoBehaviour {
    public string ExitSceneName = "OneJangMenu";
    public List<GameObject> Birds;
    public List<GameObject> Pigs;

    public GameObject VictoryPanel;
    public GameObject FailPanel;

    void Update()
    {
       for(int i = 0; i  < Pigs.Count; ++i)
        {
            if(Pigs[i] == null)
            {
                print("없음");
                Pigs.Remove(Pigs[i]);
            }
            
        }
        for (int i = 0; i < Birds.Count; ++i)
        {
            if (Birds[i] == null)
            {
                Birds.Remove(Birds[i]);
            }

        }

        if (Pigs.Count == 0)
        {
            GameObject[] a = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < a.Length; ++i)
            {
                a[i].SetActive(false);
            }
            VictoryPanel.SetActive(true);
            

        }
        if (Pigs.Count > 0   && Birds.Count  == 0)
        {
            FailPanel.SetActive(true);
            GameObject[] a = GameObject.FindGameObjectsWithTag("Player");
            for(int i = 0; i < a.Length; ++i)
            {
                a[i].SetActive(false);
            }
        }
        if(Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(ExitSceneName);
        }
      
    }
    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
