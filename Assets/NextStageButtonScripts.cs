using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NextStageButtonScripts : MonoBehaviour
{
    public void NextStage()
    {
        string mapName = YH_SingleTon.DataManager.Instance.currentMapName;
        int hypenIdx = mapName.IndexOf('-');
        int mainStage = Int32.Parse(mapName.Substring(0, hypenIdx));
        int subStage = Int32.Parse(mapName.Substring(hypenIdx+1, mapName.Length-(hypenIdx+1)));
        StringBuilder builder = new StringBuilder();
        builder.Append(mainStage.ToString());
        builder.Append('-');
        builder.Append((subStage + 1).ToString());

        YH_SingleTon.DataManager.Instance.LoadMapData(builder.ToString());
       
    }
    public void NextStage(string mapName)
    {
        int hypenIdx = mapName.IndexOf('-');
        int mainStage = Int32.Parse(mapName.Substring(0, hypenIdx));
        int subStage = Int32.Parse(mapName.Substring(hypenIdx, mapName.Length - hypenIdx));

        YH_SingleTon.DataManager.Instance.LoadMapData(mapName);

    }
}
