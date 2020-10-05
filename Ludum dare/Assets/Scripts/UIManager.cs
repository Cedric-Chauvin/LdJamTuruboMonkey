using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton
    private static UIManager instance;
    public static UIManager Instance
    {
        get => instance;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    public Image timerFill;
    public Text timerText;
    public Text speedText;
    public Text totalTimer;
    public Text laps;

    public void updateTimer(float time,float maxTime,float maxTimer)
    {
        timerText.text = time.ToString();
        timerFill.fillAmount = time/maxTime;
        string minutes = Mathf.Floor(maxTimer / 60).ToString("00");
        string seconds = (maxTimer % 60).ToString("00");
        totalTimer.text = string.Format("{0}:{1}", minutes, seconds);
    }

    public void updateSpeed(float kph)
    {
        speedText.text = ((int)kph).ToString();
    }
}
