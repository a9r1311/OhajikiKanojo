using UnityEngine;
using UnityEngine.SceneManagement;

public class Princess_Move : MonoBehaviour
{
    [Header("目的地の座標")]
    [SerializeField] public float coordinate = 10f;

    [Header("移動速度")]
    [SerializeField] public float speed = 5f;

    [Header("遷移するシーン名")]
    [SerializeField] string nextSceneName = "GameClear";

    [SerializeField] private ScoreDirector scoreDirector;  // スコアデータの参照
    [SerializeField] private GameStop gameStop;  //ゲームストップへの参照

    // シーン遷移を1回だけ行うため
    bool hasChangedScene = false;

    void Update()
    {
        //ゲームストップ中はスポーンさせない
        if (gameStop.isGameStop)
            return;

        Vector3 targetPosition = new Vector3(0f, 1f, coordinate);

        // 目的地へ移動
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // 目的地に到着したらシーン遷移
        if (!hasChangedScene &&
            Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            //スコアを保存
            ScoreData.FinalScore = scoreDirector.GetScore();

            hasChangedScene = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}