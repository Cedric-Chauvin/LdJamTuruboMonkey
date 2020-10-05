using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Text remainTimer;
    public Text laps;
    public GameObject scorePanel;
    public Text timePanel;
    public Text lapsPanel;
    public Text bestLapPanel;
    public void updateTimer(float time,float maxTime,float maxTimer)
    {
        timerText.text = time.ToString();
        timerFill.fillAmount = time/maxTime;
        string mremain = Mathf.Floor(time / 60).ToString("00");
        string sremain = (time % 60).ToString("00");
        remainTimer.text = string.Format("{0}:{1}", mremain, sremain);
        string mtotal = Mathf.Floor(maxTimer / 60).ToString("00");
        string stotal = (maxTimer % 60).ToString("00");
        totalTimer.text = string.Format("{0}:{1}", mtotal, stotal);
    }

    public void updateSpeed(float kph)
    {
        speedText.text = ((int)kph).ToString();
    }

    public void End()
    {
        string minutes = Mathf.Floor(RaceManager.Instance.totalTimer / 60).ToString("00");
        string seconds = (RaceManager.Instance.totalTimer % 60).ToString("00");
        if(RaceManager.Instance.bestLap != 999f)
        {
            string mLap = Mathf.Floor(RaceManager.Instance.bestLap / 60).ToString("00");
            string sLap = (RaceManager.Instance.bestLap % 60).ToString("00");
            bestLapPanel.text = string.Format("{0}:{1}", mLap, sLap);
        }
        timePanel.text = string.Format("{0}:{1}", minutes, seconds);
        lapsPanel.text = laps.text;
        scorePanel.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Restart();
    }
}
