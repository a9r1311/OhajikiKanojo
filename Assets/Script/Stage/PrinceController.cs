using UnityEngine;

public class PrinceController : MonoBehaviour
{
    [SerializeField] private ScoreDirector scoreDirector;  //スコアデータの参照
    [SerializeField] private GameStop gameStop;  //ゲームストップへの参照
    [SerializeField] private ChangeGame changeGame;  //ゲーム遷移への参照
    [SerializeField] private ResultManager resultManager;  //リザルト管理への参照

    [Header("目的地の座標")]
    [SerializeField] public float coordinate = 10f;

    [Header("移動速度")]
    [SerializeField] public float speed = 5f;

    [Header("ターゲットのHP")]
    [SerializeField] private int maxHp = 100;  //ターゲットのHP
    public int currentHp;  //現在のHP

    //リザルト処理を1回だけ行うため
    bool hasChangedScene = false;

    //リトライ用
    Vector3 startPosition;
    Quaternion startRotation;

    void Start()
    {
        currentHp = maxHp;  //初期HPを設定

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        //ゲームストップ中はスポーンさせない
        if (gameStop.isGameStop)
            return;

        Vector3 targetPosition = new Vector3(0f, 1f, coordinate);

        //目的地へ移動
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        //目的地に到着したらリザルト
        if (!hasChangedScene &&
            Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            hasChangedScene = true;

            //HP0時と同じ処理
            resultManager.ResultView(currentHp, true);
            gameStop.StopGame();
            changeGame.GoResult();
        }
    }

    //ターゲットが攻撃されたときの処理
    private void Damage(int damage)
    {
        Debug.Log("HP: " + (currentHp - damage));

        //HPを減らす
        if ((currentHp -= damage) <= 0)
        {
            //リザルト表示
            resultManager.ResultView(currentHp, false);
            gameStop.StopGame();  //ゲームストップ
            changeGame.GoGameOver();  //リザルト移行
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemyBase = other.transform.parent.gameObject.GetComponent<EnemyBase>();

            //敵が死んでいるかどうかを確認
            if (!enemyBase.isDead)
            {
                //敵がターゲットに接触したときの処理
                Damage(enemyBase.power);

                //敵の状態をリセット
                enemyBase.ResetState();

                //敵をオブジェクトプールに返す
                enemyBase.spawnDirector.ReturnEnemyToPool(other.transform.parent.gameObject, enemyBase.id);
                enemyBase.spawnDirector.RemoveActiveEnemy(other.transform.parent.gameObject);
            }
        }
    }

    //リトライ時初期化
    public void Retry()
    {
        currentHp = maxHp;
        hasChangedScene = false;

        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}