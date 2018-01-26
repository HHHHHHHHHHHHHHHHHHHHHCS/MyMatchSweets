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
    [SerializeField]
    private GameObject timeOverBg;
    [SerializeField]
    private Text endScoreText;
    [SerializeField]
    private Button endBackBtn;
    [SerializeField]
    private Button restartBtn;

    public MainUIManager Init()
    {
        timeText.text = "";
        scoreText.text = "0";
        backBtn.onClick.AddListener(SceneLoader.LoadStartScene);
        endBackBtn.onClick.AddListener(SceneLoader.LoadStartScene);
        restartBtn.onClick.AddListener(SceneLoader.LoadMainScene);
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

    public void TimeOver(int score)
    {
        timeText.GetComponent<Animation>().Stop();
        timeOverBg.SetActive(true);
        endScoreText.text = score.ToString();
    }
}
