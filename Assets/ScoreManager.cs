using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YH_SingleTon;

namespace YH_SingleTon
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        Text textComponent;

        int score = 0;

        public int Score { get { return score; } }
        // Start is called before the first frame update
        public void Init()
        {
            textComponent = GameObject.Find("ScoreText").GetComponent<Text>();
            score = 0;
            textComponent.text = "0";
        }

        public void AddScore(int val)
        {
            if(textComponent != null)
            {

            score += val;
            textComponent.text = score.ToString();
            }
        }
    }
}
