using UnityEngine;

public class TargetToProtect : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;  //ターゲットのHP
    public int currentHp;  //現在のHP

    void Start()
    {
        currentHp = maxHp;  //初期HPを設定
    }

    //ターゲットが攻撃されたときの処理
    private void Damage(int damage)
    {
        //Debug.Log(damage + "ダメージ");
        Debug.Log("HP: " + (currentHp - damage));
        //HPを減らす
        if ((currentHp -= damage) <= 0)
        {
            //Debug.Log("お前が殺した");
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
