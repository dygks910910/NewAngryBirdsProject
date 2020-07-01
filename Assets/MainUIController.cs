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
            mainPanel = gameObject.transform.Find("MainPanel").gameObject;
            settingPanel = gameObject.transform.Find("SettingPanel").gameObject;
            scorePanel = gameObject.transform.Find("ScorePanel").gameObject;
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
