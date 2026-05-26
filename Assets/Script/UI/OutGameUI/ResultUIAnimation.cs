using UnityEngine;
using System.Collections;

public class ResultUIAnimation : MonoBehaviour
{
    [SerializeField] RectTransform result;

    Vector2 resultInPos;
    Vector2 resultOutPos;

    void Start()
    {
        resultInPos = result.anchoredPosition;
        resultOutPos = resultInPos + new Vector2(0, 1000);
    }

    public void MoveResultInPosition(float duration)
    {
        StartCoroutine(MRIP(duration));
    }

    public void MoveResultOutPosition(float duration)
    {
        StartCoroutine(MROP(duration));
    }

    IEnumerator MRIP(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = t * t;

            result.anchoredPosition = Vector2.Lerp(resultOutPos, resultInPos, t);

            yield return null;
        }
    }

    IEnumerator MROP(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = t * t;

            result.anchoredPosition = Vector2.Lerp(resultInPos, resultOutPos, t);

            yield return null;
        }
    }

    public void SetResultInPosition()
    {
        result.anchoredPosition = resultInPos;
    }

    public void SetResultOutPosition()
    {
        result.anchoredPosition = resultOutPos;
    }
}
