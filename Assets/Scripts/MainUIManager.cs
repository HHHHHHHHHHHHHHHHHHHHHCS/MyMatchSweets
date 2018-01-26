using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [SerializeField]
    private Text timeText;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Button backBtn;

    public MainUIManager Init()
    {
        timeText.text = "";
        scoreText.text = "0";
        //backBtn.onClick.AddListener(null);
        return this;
    }

    public void RefreshTime(float time)
    {
        timeText.text = time.ToString("0");
    }

    public void RefreshScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void TimeOver()
    {
        timeText.GetComponent<Animation>().Stop();
    }
}
