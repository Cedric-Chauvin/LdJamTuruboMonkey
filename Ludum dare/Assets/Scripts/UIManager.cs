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

    public void updateTimer(float time,float maxTime)
    {
        timerText.text = time.ToString();
        timerFill.fillAmount = time/maxTime;
    }

    public void updateSpeed(float kph)
    {
        speedText.text = ((int)kph).ToString();
    }
}
