using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public IntReference score;

    [SerializeField]
    private TextMeshProUGUI scoreDisplay;

    [SerializeField]
    private float targetSizeMultiplier;
    [SerializeField]
    private float growSpeed;
    private Vector3 targetSize;
    private Vector3 originalSize;

    private void Awake()
    {
        originalSize = scoreDisplay.transform.localScale;
        targetSize = scoreDisplay.transform.localScale * targetSizeMultiplier;
    }

    public void Init()
    {
        scoreDisplay.text = score.Value + "";
    }

    public void SetScore(int score)
    {
        this.score.Variable.SetValue(score);
        scoreDisplay.text = score.ToString();
        StopCoroutine("SetScoreCo");
        StartCoroutine("SetScoreCo");
    }

    IEnumerator SetScoreCo()
    {
        while (Vector3.Distance(scoreDisplay.transform.localScale, targetSize) > 0.1f)
        {
            scoreDisplay.transform.localScale = Vector3.Lerp(scoreDisplay.transform.localScale, targetSize, growSpeed * Time.deltaTime);
            yield return null;
        }

        transform.localScale = targetSize;

        while (Vector3.Distance(scoreDisplay.transform.localScale, originalSize) > 0.1f)
        {
            scoreDisplay.transform.localScale = Vector3.Lerp(scoreDisplay.transform.localScale, originalSize, growSpeed * Time.deltaTime);
            yield return null;
        }

        scoreDisplay.transform.localScale = originalSize;
    }

    public void ResetScore()
    {
        score.Variable.SetValue(0);
    }
}
