using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonInteration : MonoBehaviour
{
    public GameObject volumeSlider;
    float sliderVolume;
    public void Start()
    {
        sliderVolume = YH_Data.DataManager.Instance.playerCommonData.masterVolume;
    }
    public void DisableThis()
    {
        gameObject.SetActive(false);
    }
    public void SavePlayerCommonData()
    {
        //YH_Data.DataManager.Instance.playerCommonData.masterVolume = 
    }
    public void SetVolum()
    {
        YH_Data.DataManager.Instance.playerCommonData.masterVolume = volumeSlider.GetComponent<Slider>().value;
    }
    public void EnableThis()
    {
        gameObject.SetActive(true);
    }
}
