using HP.Omnicept.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RateHeart : MonoBehaviour
{
    public GameObject textHeartRate;
    public GameObject textMediumHeartRate;
    public GameObject alarm;
    public Fire fire;

    public float cooldownTime = 3;
    public int countWanted = 10;

    private uint heartRate;
    private float timeReloadHeartRate;
    private uint heartRateTotal = 0;
    private int count = 0;
    private long mediumHeartRate;

    private GliaBehaviour _gliaBehaviour = null;
    private GliaBehaviour gliaBehaviour
    {
        get
        {
            if (_gliaBehaviour == null)
            {
                _gliaBehaviour = FindObjectOfType<GliaBehaviour>();
            }

            return _gliaBehaviour;
        }
    }
    
    public void Start()
    {
        timeReloadHeartRate = cooldownTime + Time.time;
    }

    public void Update() {
        if (timeReloadHeartRate <= Time.time) {
            if (gliaBehaviour.GetLastHeartRate() != null)
            {
                heartRate = gliaBehaviour.GetLastHeartRate().Rate;
                textHeartRate.GetComponent<TextMeshProUGUI>().text = heartRate.ToString();
                Debug.Log(gliaBehaviour.GetLastHeartRate().ToString());
                
                if(count < countWanted)
                {
                    heartRateTotal += heartRate;
                    count++;
                } else
                {
                    mediumHeartRate = heartRateTotal / count;
                    textMediumHeartRate.GetComponent<TextMeshProUGUI>().text = "Moyenne : " + mediumHeartRate;
                }
            }
            else
            {
                textHeartRate.GetComponent<TextMeshProUGUI>().text = "0";
                textMediumHeartRate.GetComponent<TextMeshProUGUI>().text = "Moyenne : 0";
            }
            timeReloadHeartRate = cooldownTime + Time.time;
        }
        colorCheck();
    }

    public void colorCheck()
    {
        long yellowColor = mediumHeartRate + 15 * mediumHeartRate / 100;
        long redColor = mediumHeartRate + 30 * mediumHeartRate / 100;

        if(heartRate >= yellowColor && heartRate < redColor)
        {
            textHeartRate.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            alarm.GetComponent<AudioSource>().volume = 0.2f;
            fire.maxInstances = 8;
            fire.spreadPeriod = 20;

        } else if (heartRate >= redColor)
        {
            textHeartRate.GetComponent<TextMeshProUGUI>().color = Color.red;
            alarm.GetComponent<AudioSource>().volume = 0.1f;
            fire.maxInstances = 6;
            fire.spreadPeriod = 30;
        } else
        {
            textHeartRate.GetComponent<TextMeshProUGUI>().color = Color.white;
            fire.maxInstances = 10;
            fire.spreadPeriod = 10;
        }
    }
}
