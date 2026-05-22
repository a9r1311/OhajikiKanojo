using UnityEngine;

public class MiniMapPosition : MonoBehaviour
{
    public Transform target;
    public Transform goal;

    private float totalDistance;

    public RectTransform startRect;
    public RectTransform pointer;
    public RectTransform goalRect;

    void Start()
    {
        totalDistance = GetDistanceToGoal();
    }

    void LateUpdate()
    {
        OutputProgress(GetTargetProgress());
    }

    //ターゲットとゴールの距離を測る
    float GetDistanceToGoal()
    {
        return Vector3.Distance(target.position, goal.position);
    }

    //ターゲットとゴール間の距離を割合で求める
    float GetTargetProgress()
    {
        return Mathf.Clamp01(1 - (GetDistanceToGoal() / totalDistance));
    }

    //ミニマップみたいなもので表示する。
    void OutputProgress(float progress)
    {
        Vector2 pos = Vector2.Lerp(
            startRect.anchoredPosition,
            goalRect.anchoredPosition,
            progress
        );

        pointer.anchoredPosition = pos;
    }
}