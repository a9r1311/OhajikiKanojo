using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("遷移するシーン名")]
    [SerializeField] string nextSceneName = "GameClear";

    [Header("ターゲットのHP")]
    [SerializeField] private int maxHp = 100;  //ターゲットのHP
    public int currentHp;  //現在のHP

    //シーン遷移を1回だけ行うため
    bool hasChangedScene = false;

    void Start()
    {
        currentHp = maxHp;  //初期HPを設定
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

        //目的地に到着したらシーン遷移
        if (!hasChangedScene &&
            Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            //スコアを保存
            ScoreData.FinalScore = scoreDirector.GetScore();

            hasChangedScene = true;
            SceneManager.LoadScene(nextSceneName);
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
            resultManager.ResultView();
            gameStop.StopGame();  //ゲームストップ
            changeGame.GoResult();  //リザルト移行
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemyBase = other.transform.parent.gameObject.GetComponent<EnemyBase>();

            //敵がターゲットに接触したときの処理
            Damage(enemyBase.power);
            //敵の状態をリセット
            enemyBase.ResetState();
            //敵をオブジェクトプールに返す
            enemyBase.spawnDirector.ReturnEnemyToPool(other.transform.parent.gameObject, enemyBase.id);
        }
    }
}
