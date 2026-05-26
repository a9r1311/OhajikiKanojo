using UnityEngine;
using System.Collections;

public class InGameUIAnimation : MonoBehaviour
{
    [SerializeField] RectTransform gameUILU;
    [SerializeField] RectTransform gameUIRU;

    Vector2 gameUILUInPos;
    Vector2 gameUIRUInPos;

    Vector2 gameUILUOutPos;
    Vector2 gameUIRUOutPos;

    void Start()
    {
        gameUILUInPos = gameUILU.anchoredPosition;
        gameUIRUInPos = gameUIRU.anchoredPosition;

        gameUILUOutPos = gameUILUInPos + new Vector2(-1000, 0);
        gameUIRUOutPos = gameUIRUInPos + new Vector2(1000, 0);
    }

    public void MoveInGameInPosition(float duration)
    {
        StartCoroutine(MIGIP(duration));
    }

    public void MoveInGameOutPosition(float duration)
    {
        StartCoroutine(MIGOP(duration));
    }

    IEnumerator MIGIP(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = t * t;

            gameUILU.anchoredPosition = Vector2.Lerp(gameUILUOutPos, gameUILUInPos, t);
            gameUIRU.anchoredPosition = Vector2.Lerp(gameUIRUOutPos, gameUIRUInPos, t);

            yield return null;
        }
    }

    IEnumerator MIGOP(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = t * t;

            gameUILU.anchoredPosition = Vector2.Lerp(gameUILUInPos, gameUILUOutPos, t);
            gameUIRU.anchoredPosition = Vector2.Lerp(gameUIRUInPos, gameUIRUOutPos, t);

            yield return null;
        }
    }

    public void SetInGameInPosition()
    {
        gameUILU.anchoredPosition = gameUILUInPos;
        gameUIRU.anchoredPosition = gameUIRUInPos;
    }

    public void SetInGameOutPosition()
    {
        gameUILU.anchoredPosition = gameUILUOutPos;
        gameUIRU.anchoredPosition = gameUIRUOutPos;
    }
}