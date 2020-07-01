using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH_SingleTon;

namespace YH_SingleTon
{

    public class MainUIController : Singleton<MainUIController>
    {
        public GameObject mainPanel;
        public GameObject settingPanel;
        public GameObject scorePanel;

        public void Init()
        {
            GameObject canvas = GameObject.Find("Canvas");
            mainPanel = canvas.transform.Find("MainPanel").gameObject;
            settingPanel = canvas.transform.Find("SettingPanel").gameObject;
            scorePanel = canvas.transform.Find("ScorePanel").gameObject;
        }
        public void SetMainPanelEnable(bool b)
        {
            mainPanel.SetActive(b);
        }
        public void SetSettingPanelEnable(bool b)
        {
            settingPanel.SetActive(b);
        }
        public void SetScorePanelEnable(bool b)
        {
            scorePanel.SetActive(b);
        }
        
    }

}
