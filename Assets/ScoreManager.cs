using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YH_SingleTon;

namespace YH_SingleTon
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        public delegate int OnAddScoreValue();
        Text textComponent;

        int score = 0;

        // Start is called before the first frame update
        void Start()
        {
            textComponent = GetComponent<Text>();
        }

        public void AddScore(int val)
        {
            score += val;
            textComponent.text = score.ToString();
        }
    }
}
