using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    [SerializeField]
    private Button startGameBtn;
    [SerializeField]
    private Button quitBtn;

    private void Awake()
    {
        startGameBtn.onClick.AddListener(SceneLoader.LoadMainScene);
        quitBtn.onClick.AddListener(SceneLoader.QuitGame);
    }
}
