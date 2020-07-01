using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YH_Data;

public class ShowScorePanel : MonoBehaviour
{
    public Image[] startImg;
    // Start is called before the first frame update
    public Text scoreText;
    public Text ClearStr;
    public Text highScore;
    public Text stageName;

    public Button nextStageButton;
    const string FAIL_STR = "LEVEL FAILED!";
    const string CLEAR_STR = "LEVEL CLAERD!";
    const int STAR_COUNT = 3;


    private void OnEnable()
    {
        int curScore = YH_SingleTon.ScoreManager.Instance.Score;
        int tagetScore = YH_SingleTon.DataManager.Instance.mapData.threeStarScore;
        string strStageName = YH_SingleTon.DataManager.Instance.currentMapName;
        stageName.text = strStageName;
        int j = 1;
        for(int i = 0; i < STAR_COUNT; ++i)
        {
            if (curScore > ((tagetScore * 0.3333f) * j++))
            {
                startImg[i].enabled = true;
                ClearStr.text = CLEAR_STR;
            }
            else
            {
                startImg[i].enabled = false;
                if(i == 0)
                {
                    ClearStr.text = FAIL_STR;
                }
            }
        }
        if (ClearStr.text == FAIL_STR)
        {
            nextStageButton.enabled = false;
        }
        else
            nextStageButton.enabled = true;

        //현재점수 > (목표 점수  * 0.333f) * i 별 세팅.
        scoreText.text = curScore.ToString();
        //하이 스코어 세팅.
        int highScoreVal = YH_SingleTon.DataManager.Instance.playerData.GetHighScore(strStageName);

        if (highScoreVal > curScore)
        {
            highScore.text = highScoreVal.ToString();
        }
        else
        {
            highScore.text = curScore.ToString();
            YH_SingleTon.DataManager.Instance.playerData.SetHighScore(strStageName, curScore);
        }
    }
    public void SetScore()
    {
        scoreText.text = YH_SingleTon.ScoreManager.Instance.Score.ToString();
    }

}
