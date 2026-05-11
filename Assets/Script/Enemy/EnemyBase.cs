using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEditor.PackageManager.Requests;

public class EnemyBase : MonoBehaviour
{
    protected Rigidbody rb;
    private EnemySpawnDirector spawnDirector;  //スポーンディレクターへの参照

    public enum movePattern { Idle, Walk, Knock };  //行動パターン
    public movePattern moveState = movePattern.Idle;  //現在の行動パターン

    [SerializeField] protected float speed = 1f;  //移動速度
    [SerializeField] protected int maxHp = 1;  //最大HP
    protected int currentHp;  //現在のHP
    [SerializeField] private float knockBackMultiplier = 1.0f;  //ノックバック倍率
    public GameObject target;  //追尾ターゲット

    private bool knockbyPlayer = false;  //プレイヤーによるノックバックを受けたかどうか
    private HashSet<EnemyBase> hitEnemies = new HashSet<EnemyBase>();  //ノックバックを受けた敵のリスト
    private bool knockRock = false;  //ノックバックのクールダウン中かどうか

    public int enemyIndex;  //敵の種類を識別するためのインデックス

    void Start()
    {
        currentHp = maxHp;  //現在のHPを最大HPで初期化
        rb = GetComponent<Rigidbody>();

        //ターゲット確認
        if (target == null)
        {
            target = GameObject.Find("Prince");

            if (target == null)
                Debug.LogError("Targetが設定されていません");
        }

        spawnDirector = GameObject.Find("EnemySpawnDirector").GetComponent<EnemySpawnDirector>();  //スポーンディレクターへの参照を取得

        moveState = movePattern.Walk;
    }

    void Update()
    {
        //行動パターンに応じた処理
        switch (moveState)
        {
            case movePattern.Idle:  //待機行動
                break;

            case movePattern.Knock:  //ノックバック行動
                //ノックバックのクールダウンが終わり、ノックバックの勢いが弱まったら歩行行動に移行
                if (rb.linearVelocity.magnitude < 0.01f && !knockRock)
                {
                    //Debug.Log("ノックバック終了");
                    moveState = movePattern.Walk;
                    hitEnemies.Clear();
                    //knockedByEnemy = false;
                }
                break;

            case movePattern.Walk:  //歩行行動
                //ターゲットに向かって移動
                MovePatternWalk();
                break;
        }
    }

    //歩行行動
    protected virtual void MovePatternWalk()
    {
        if (target != null)
        {
            transform.LookAt(target.transform);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            rb.linearVelocity = transform.forward * speed;
        }
    }

    private void MovePatternKnock()
    {
        currentHp--;  //HPを減らす

        if (currentHp <= 0)  //HPが0未満になったら死亡処理
        {
            StartCoroutine(DeadEnemy());
        }

        //ノックバック行動に移行
        moveState = movePattern.Knock;
        //ノックバックの勢いを減衰させる
        rb.linearVelocity *= knockBackMultiplier;
        //ノックバックのクールダウンを開始
        knockRock = true;
        StartCoroutine(KnockCoolDown());
    }

    private IEnumerator KnockCoolDown()
    {
        yield return new WaitForSeconds(0.5f);  //ノックバックのクールダウン時間
        knockRock = false;  //クールダウン終了
    }

    //hpが0未満になったときの死亡処理
    private IEnumerator DeadEnemy()
    {
        //ぶっ飛ぶ
        //knockBackMultiplier = 50f;
        yield return new WaitForSeconds(0.5f);

        //初期化
        Reset();

        //スポーンディレクターに敵をオブジェクトプールに返すように指示
        spawnDirector.ReturnEnemyToPool(this.gameObject, enemyIndex);
    }

    protected virtual void Reset()
    {
        moveState = movePattern.Idle;  //行動パターンを待機にする
        rb.linearVelocity = Vector3.zero;  //速度を0にする
        currentHp = maxHp;  //HPを最大HPに戻す
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !knockbyPlayer && moveState != movePattern.Knock)
        {
            //Debug.Log("くぉ～ぶつかる！！");
            knockbyPlayer = true;
            MovePatternKnock();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

            //ノックバック行動中の敵に衝突した場合、ノックバックを受ける
            if ((enemy.moveState == movePattern.Knock || moveState == movePattern.Knock) && !hitEnemies.Contains(enemy))
            {
                //Debug.Log("ノックバックを受ける");
                //ノックバックを受けた敵がリストにない場合、リストに追加
                hitEnemies.Add(enemy);
                MovePatternKnock();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

            //ノックバック行動中の敵に衝突した場合、ノックバックを受ける
            if ((enemy.moveState == movePattern.Knock || moveState == movePattern.Knock) && !hitEnemies.Contains(enemy))
            {
                //ノックバックを受けた敵がリストにない場合、リストに追加
                hitEnemies.Add(enemy);
                MovePatternKnock();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

            if (hitEnemies.Contains(enemy))
            {
                //ノックバックを受けた敵が衝突から離れた場合、リストから削除
                hitEnemies.Remove(enemy);
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("インド人を右に！！");
            //プレイヤーとの衝突が離れた場合、ノックバックを受けた状態をリセット
            knockbyPlayer = false;
        }
    }
}
