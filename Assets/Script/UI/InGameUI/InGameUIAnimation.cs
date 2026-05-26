/*using UnityEngine;
using System.Collections;

public class InGameUIAnimation : MonoBehaviour
{
    [SerializeField] RectTransform gameUILU;
    [SerializeField] RectTransform gameUIRU;

    Vector2 gameUILUInPos;
    Vector2 startInPos;

    Vector2 gameUILUOutPos;
    Vector2 startOutPos;

    void Start()
    {
        gameUILUInPos = gameUILU.anchoredPosition;
        startInPos = start.anchoredPosition;

        gameUILUOutPos = gameUILUInPos + new Vector2(-800, 1000);
        startOutPos = startInPos + new Vector2(-500, -1200);
    }

    public void MoveGameUIInPosition(float duration)
    {
        StartCoroutine(MGIP(duration));
    }

    public void MoveTitleOutPosition(float duration)
    {
        StartCoroutine(MTOP(duration));
    }

    IEnumerator MGIP(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = t * t;

            gameUILU.anchoredPosition = Vector2.Lerp(gameUILUOutPos, gameUILUInPos, t);
            start.anchoredPosition = Vector2.Lerp(startOutPos, startInPos, t);

            yield return null;
        }
    }

    IEnumerator MTOP(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = t * t;

            title.anchoredPosition = Vector2.Lerp(titleInPos, titleOutPos, t);
            start.anchoredPosition = Vector2.Lerp(startInPos, startOutPos, t);

            yield return null;
        }
    }

    public void SetTitleInPosition()
    {
        title.anchoredPosition = titleInPos;
        start.anchoredPosition = startInPos;
    }

    public void SetTitleOutPosition()
    {
        title.anchoredPosition = titleOutPos;
        start.anchoredPosition = startOutPos;
    }
}
*/