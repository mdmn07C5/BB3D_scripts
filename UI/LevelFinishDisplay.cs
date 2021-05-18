using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LevelFinishDisplay : MonoBehaviour
{
    [Header("Confetti Reference")]
    [SerializeField]
    private GameObject confettiUI;

    [SerializeField]
    private GameObject confetti1;

    [SerializeField]
    private GameObject confetti2;

    [SerializeField]
    private Transform spawnPos;

    [Header("Win UI Reference")]
    [SerializeField]
    private GameObject winUI;

    [SerializeField]
    private TextMeshProUGUI levelDisplay;

    [SerializeField]
    private TextMeshProUGUI winScoreDisplay;

    [SerializeField]
    private TextMeshProUGUI winHighScoreDisplay;

    [Header("Lose UI Reference")]
    [SerializeField]
    public GameObject loseUI;

    [SerializeField]
    private TextMeshProUGUI loseScoreDisplay;

    [SerializeField]
    private TextMeshProUGUI loseHighScoreDisplay;


    [Header("Scriptable Objects")]
    [SerializeField]
    private IntVariable score;

    private int highScore;

    [SerializeField]
    private IntVariable currentLevel;

    [SerializeField]
    private GameEvent onNextLvlClicked;

    [SerializeField]
    private GameEvent onRetryClicked;

    public void ShowRetry()
    {
        StopCoroutine("CloseWinUI");
        winUI.SetActive(false);
        loseUI.SetActive(true);
        GetHighScore();
        SetHighScore();
        loseScoreDisplay.text = score.GetValue() + "";
        loseHighScoreDisplay.text = "Best: " + highScore;
    }

    public void ShowNext()
    {     
        winUI.SetActive(true);
        StartCoroutine("CloseWinUI");

        //GetHighScore();
        //SetHighScore();

        levelDisplay.text = "Level " + (currentLevel.GetValue() - 1) + "";
        //winScoreDisplay.text = score.GetValue() + "";
        //winHighScoreDisplay.text = "Best: " + highScore;

        for (int i = 0; i < spawnPos.childCount; i++)
        {
            GameObject conf1 = Instantiate(confetti1, confettiUI.transform);
            conf1.transform.position = spawnPos.GetChild(i).position;
            Destroy(conf1, 3f);

            GameObject conf2 = Instantiate(confetti2, confettiUI.transform);
            conf2.transform.position = spawnPos.GetChild(i).position;
            Destroy(conf2, 3f);
        }
    }

    IEnumerator CloseWinUI()
    {
        yield return new WaitForSeconds(1.5f);
        winUI.SetActive(false);
    }

    public void CloseUI()
    {
        winUI.SetActive(false);
        loseUI.SetActive(false);
    }

    public void NextLevelClick()
    {
        onNextLvlClicked.Raise();
    }

    public void RetryClick()
    {
        onRetryClicked.Raise();
    }

    private void GetHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void SetHighScore()
    {
        if (score.GetValue() > highScore)
        {
            highScore = score.GetValue();
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }
}
