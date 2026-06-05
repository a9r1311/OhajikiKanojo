using UnityEngine;

public class TargetToProtect : MonoBehaviour
{
    [SerializeField] private GameStop gameStop;  //ゲームストップへの参照
    [SerializeField] private ChangeGame changeGame;  //ゲーム遷移への参照
    [SerializeField] private int maxHp = 100;  //ターゲットのHP
    public int currentHp;  //現在のHP

    void Start()
    {
        currentHp = maxHp;  //初期HPを設定
    }

    //ターゲットが攻撃されたときの処理
    private void Damage(int damage)
    {
        Debug.Log("HP: " + (currentHp - damage));

        //HPを減らす
        if ((currentHp -= damage) <= 0)
        {
            gameStop.StopGame();  //ゲームストップ
            changeGame.GoResult();  //リザルト移行
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
}
