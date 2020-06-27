using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupTextEffect : MonoBehaviour
{
    private Text text;
    public float fontSize;
    static WaitForSeconds wfs = new WaitForSeconds(0.01f);
    static WaitForSeconds wf1sec = new WaitForSeconds(1);

    private void Awake()
    {
        text = GetComponent<Text>();
    }
    public void CreateText(string str,Vector3 position)
    {
        text.text = str;
        gameObject.transform.position = position;
        int score;
        Int32.TryParse(str,out score);
        YH_SingleTon.ScoreManager.Instance.AddScore(score);
        StartCoroutine(PopupEffect());
    }
    private IEnumerator PopupEffect()
    {

        Vector3 originPosition = gameObject.transform.position;
        Vector3 destPosition = originPosition;
        destPosition.y = originPosition.y + 1.5f;
        Vector3 newPosition;
        for(int i  = 0; i < 10; ++i)
        {
            newPosition = Vector3.Lerp(originPosition, destPosition, 0.1f * i);
            text.fontSize = (int)Mathf.Lerp(0, fontSize, 0.1f * i);
            gameObject.transform.position = newPosition;
            yield return wfs;
        }

        yield return wf1sec;

        YH_SingleTon.YH_ObjectPool.Instance.GiveBackObj(gameObject);
    }
   
}
