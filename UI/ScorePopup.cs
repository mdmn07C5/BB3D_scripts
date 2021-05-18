using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    static ScorePopup prefab;

    public int point;

    private Transform canvas; //Canvas that this ui should be under
    private GameObject totalScoreText; //Total score of the game (need this to reference the target position the score popup should lerp to)

    private TextMeshProUGUI tmpUGUI;
    private ScoreDisplay scoreDisplay;

    private RectTransform rectTransform;

    [SerializeField]
    private float moveSpeed = 1f; // how fast score popup should lerp to total score text
    [SerializeField]
    private float scaleUpSpeed = 0.3f; //how fast the score popup text should scale up
    [Range(0f, 1f)]
    [SerializeField]
    private float fadeSpeed = 0.025f; // how fast text should fade (for changing text alpha value)

    [Space(10)]
    [SerializeField] private Vector2 scoreTextStartPos = new Vector2(0f, -365.1401f);  // the start pos score popup should be spawned
    [SerializeField] private Vector3 textStartingSize = new Vector3(1.693961f, 1.693961f, 1.693961f); // the start size of this text
    [SerializeField] private Vector3 textSizeTarget = new Vector3(2.139596f, 2.139596f, 2.139596f); // the target size of this text

    static bool areVariablesAssigned;
    static Vector3 Orignalposition;
    static Vector2 OriginalDimensions;
    static Vector3 OriginalScale;


    private void Awake()
	{
        AssignStaticVariables();

        totalScoreText = GameObject.Find("ScoreDisplay");
        canvas = GameObject.Find("ScoreUI").transform;

        tmpUGUI = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        scoreDisplay = totalScoreText.transform.parent.GetComponent<ScoreDisplay>();
    }

    private void OnEnable()
    {
        Reset();
    }

    void AssignStaticVariables() {
        if (areVariablesAssigned == false) {
            RectTransform temp = GetComponent<RectTransform>();
            Orignalposition = new Vector3(temp.position.x, temp.position.y, temp.position.z);
            OriginalDimensions = new Vector2(temp.rect.width, temp.rect.height);
            OriginalScale = new Vector3(temp.localScale.x, temp.localScale.y, temp.localScale.z);
        }
    }

    private void Reset()
    {
        rectTransform.localScale = OriginalScale;
        rectTransform.position = Orignalposition;
        Rect rect = rectTransform.rect;
        rect.width = OriginalDimensions.x;
        rect.height = OriginalDimensions.x;
    }

    public void Init(int scoreToAdd)
    {
        point = scoreToAdd;

        tmpUGUI.text = "+" + point.ToString();
        StartCoroutine(MoveToTotalScoreCo());

    }

    // scaling the size of this text
    IEnumerator ScaleTextCo() {
        transform.localScale = textStartingSize;

        while (tmpUGUI.IsActive()) {
            transform.localScale = Vector3.Lerp(transform.localScale, textSizeTarget, scaleUpSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator FadeTextCo() {
        while (tmpUGUI.alpha > 0.1f) {
            Color temp = tmpUGUI.color;
            temp.a -= fadeSpeed;
            tmpUGUI.color = temp;
            yield return null;
        }
        Color temp1 = tmpUGUI.color;
        temp1.a = 0;
        tmpUGUI.color = temp1;
    }

    // move this text to total score text position
	IEnumerator MoveToTotalScoreCo()
    {
        transform.SetParent(canvas); //find canvas
        //initialize position and scale
        rectTransform.anchoredPosition = scoreTextStartPos;
        rectTransform.localScale = textStartingSize;
        Color temp = tmpUGUI.color;
        temp.a = 1;
        tmpUGUI.color = temp;

        // start scaling animation
        //StartCoroutine(ScaleTextCo());

        UpdateTotalScore();

        //start moving to total score text position
        while (Vector3.Distance(transform.position, totalScoreText.transform.position) > .5f)
        {
            //Debug.Log("<color=red> POSITION </color>" + Vector3.Distance(transform.position, totalScoreText.transform.position));
            transform.position = Vector3.Lerp(transform.position, totalScoreText.transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return StartCoroutine(FadeTextCo());

        try //if this gameobject is spawned from the object pooler, put it back into pool
        {
            ObjectPooler.instance.PutBackIntoPool("ScorePopupText", gameObject);
        }
        catch (System.Exception e) {
            print("put");
            Debug.Log(e.ToString());
        }
    }

    void UpdateTotalScore() {
        int _point = int.Parse(tmpUGUI.text);
        int totalScore = scoreDisplay.score;
        int newTotalScore = _point + totalScore;
        scoreDisplay.SetScore(newTotalScore);
    }


}
