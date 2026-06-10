using UnityEngine;
using System.Collections;

public class GameOverUIAnimation : MonoBehaviour
{
    [SerializeField] RectTransform gameover;

    Vector2 gameoverInPos;
    Vector2 gameoverOutPos;

    void Start()
    {
        gameoverInPos = gameover.anchoredPosition;
        gameoverOutPos = gameoverInPos + new Vector2(0, 1000);
    }

    public void MoveGameOverInPosition(float duration)
    {
        StartCoroutine(MGOIP(duration));
    }

    public void MoveResultOutPosition(float duration)
    {
        StartCoroutine(MGOOP(duration));
    }

    IEnumerator MGOIP(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = t * t;

            gameover.anchoredPosition = Vector2.Lerp(gameoverOutPos, gameoverInPos, t);

            yield return null;
        }
    }

    IEnumerator MGOOP(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = t * t;

            gameover.anchoredPosition = Vector2.Lerp(gameoverInPos, gameoverOutPos, t);

            yield return null;
        }
    }

    public void SetGameOverInPosition()
    {
        gameover.anchoredPosition = gameoverInPos;
    }

    public void SetGameOverOutPosition()
    {
        gameover.anchoredPosition = gameoverOutPos;
    }
}
