using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the transition animation for floating the platform up when level starts and down when level ends
/// </summary>

public class PlatformTransition : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private IntVariable currentStars;
    [SerializeField] private IntVariable maxStars;

    [Header("Game Events")]
    [SerializeField] private GameEvent onPlatformFloating;
    [SerializeField] private GameEvent onPlatformNotFloating;
    [SerializeField] private GameEvent onLevelStart;
    private PlatformManager platform;

    private int flag;

    private void Awake()
    {
        platform = GetComponent<PlatformManager>();
    }

    public void CallFloatLevelPlatforms()
    {
        StartCoroutine("FloatLevelPlatforms");
    }

    public IEnumerator FloatLevelPlatforms()
    {
        for (int i = 0; i < platform.normalPlatforms.Count; i++)
        {
            flag++;
            StartCoroutine(FloatCo(platform.normalPlatforms[i]));       
        }

        while (flag > 0)
        {
            yield return null;
        }
        onLevelStart.Raise();
    }

    public IEnumerator FloatDownCo(GameObject obj)
    {
        if (obj == null)
            yield break;

        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.2f));

        Vector3 targetPosition = new Vector3(obj.transform.position.x, -100f, obj.transform.position.z);
        while (Vector3.Distance(obj.transform.position, targetPosition) > 0.1f)
        {
            targetPosition = new Vector3(obj.transform.position.x, -100f, obj.transform.position.z);
            obj.transform.position = Vector3.Lerp(obj.transform.position, targetPosition, Time.deltaTime * speed/4);
            yield return null;
        }

        obj.transform.position = targetPosition;
        Destroy(obj);
    }

    IEnumerator FloatCo(GameObject obj)
    {
        if (obj == null)
            yield break;

        yield return new WaitForSeconds(Random.Range(0f, 0.2f));

        Vector3 targetPosition = new Vector3(obj.transform.position.x, .5f, obj.transform.position.z);
        while (Vector3.Distance(obj.transform.position, targetPosition) > 0.1f)
        {
            targetPosition = new Vector3(obj.transform.position.x, .5f, obj.transform.position.z);
            obj.transform.position = Vector3.Lerp(obj.transform.position, targetPosition, Time.deltaTime * speed);
            yield return null;
        }

        obj.transform.position = targetPosition;

        Vector3 finalPosition = new Vector3(obj.transform.position.x, 0, obj.transform.position.z);

        while (Vector3.Distance(obj.transform.position, finalPosition) > 0.015f)
        {
            finalPosition = new Vector3(obj.transform.position.x, 0, obj.transform.position.z);
            obj.transform.position = Vector3.Lerp(obj.transform.position, finalPosition, Time.deltaTime * speed);
            yield return null;
        }

        obj.transform.position = finalPosition;
        flag--;
    }

    public void TryFloatVictoryPlatforms()
    {
        if (currentStars.GetValue() < maxStars.GetValue())
            return;

        StartCoroutine("FloatVictoryPlatforms");
    }

    IEnumerator FloatVictoryPlatforms()
    {
        onPlatformFloating.Raise();
        for (int i = 0; i < platform.victoryPlatforms.Count; i++)
        {
            flag++;
            platform.victoryPlatforms[i].GetComponent<MeshRenderer>().enabled = true;
            StartCoroutine(FloatCo(platform.victoryPlatforms[i]));
            yield return null;
        }

        while (flag > 0)
        {
            yield return null;
        }

        onPlatformNotFloating.Raise();
    }
}
