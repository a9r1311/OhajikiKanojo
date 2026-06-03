using UnityEngine;
using System.Collections;

public class PrinceController : MonoBehaviour
{
    // スコア管理
    [SerializeField] private ScoreDirector scoreDirector;

    // ゲーム停止管理
    [SerializeField] private GameStop gameStop;

    // ゲーム遷移管理
    [SerializeField] private ChangeGame changeGame;

    // リザルト表示管理
    [SerializeField] private ResultManager resultManager;

    // Cinemachineカメラ切り替え
    [SerializeField] private CameraDirection cameraDirection;

    [Header("目的地のZ座標")]
    [SerializeField] private float coordinate = 10f;

    [Header("移動速度")]
    [SerializeField] private float speed = 5f;

    [Header("ターゲット最大HP")]
    [SerializeField] private int maxHp = 100;

    // 現在HP
    public int currentHp;

    // ゴール処理を1回だけ実行するため
    bool hasChangedScene = false;

    void Start()
    {
        // HP初期化
        currentHp = maxHp;
    }

    void Update()
    {
        // ゲーム停止中は処理しない
        if (gameStop.isGameStop)
            return;

        // ゴール地点
        Vector3 targetPosition =
            new Vector3(0f, 1f, coordinate);

        // ゴールへ移動
        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

        // ゴール到達判定
        if (!hasChangedScene &&
            Vector3.Distance(
                transform.position,
                targetPosition) < 0.01f)
        {
            // スコア保存
            ScoreData.FinalScore =
                scoreDirector.GetScore();

            // 二重実行防止
            hasChangedScene = true;

            // リザルトカメラへ切り替え
            if (cameraDirection != null)
            {
                cameraDirection.GoResultCamera(1.5f);
            }

            // リザルト表示開始
            StartCoroutine(ResultCoroutine());
        }
    }

    // ゴール時のリザルト表示
    IEnumerator ResultCoroutine()
    {
        // カメラブレンド待機
        yield return new WaitForSeconds(1.5f);

        // ゲーム停止
        gameStop.StopGame();

        // リザルト表示
        resultManager.ResultView();
    }

    // ダメージ処理
    public void Damage(int damage)
    {
        // HP減少
        currentHp -= damage;

        // 0未満防止
        currentHp =
            Mathf.Max(currentHp, 0);

        Debug.Log(
            "HP : " + currentHp
        );

        // HPが0になったら敗北
        if (currentHp <= 0)
        {
            // リザルトカメラへ切り替え
            if (cameraDirection != null)
            {
                cameraDirection.GoResultCamera(1.5f);
            }

            // 敗北リザルト表示
            StartCoroutine(
                DeathResultCoroutine()
            );
        }
    }

    // HP0時のリザルト処理
    IEnumerator DeathResultCoroutine()
    {
        // カメラ移動待機
        yield return new WaitForSeconds(1.5f);

        // リザルト表示
        resultManager.ResultView();

        // ゲーム停止
        gameStop.StopGame();

        // リザルト状態へ移行
        changeGame.GoResult();
    }


    // 敵接触時
    private void OnTriggerEnter(
        Collider other
    )
    {
        // Enemyタグ以外は無視
        if (!other.CompareTag("Enemy"))
            return;

        // 親からEnemyBase取得
        EnemyBase enemyBase =
            other.GetComponentInParent<EnemyBase>();

        if (enemyBase == null)
            return;

        // ダメージ
        Damage(enemyBase.power);

        // 敵状態リセット
        enemyBase.ResetState();

        // オブジェクトプールへ返却
        enemyBase.spawnDirector
            .ReturnEnemyToPool(
                enemyBase.gameObject,
                enemyBase.id
            );
    }
}