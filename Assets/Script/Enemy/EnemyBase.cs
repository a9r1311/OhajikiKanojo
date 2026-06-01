using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class EnemyBase : MonoBehaviour
{
    protected Rigidbody rb;
    public EnemySpawnDirector spawnDirector;  //スポーンディレクターへの参照
    public ScoreDirector scoreDirector;  //スコアディレクターへの参照
    public GameStop gameStop;  //ゲームストップへの参照
    private AudioSource audioSource;
    protected GameObject model;  //敵のモデル（見た目）への参照
    protected GameObject colliderObject;  //敵のコライダーへの参照

    public enum movePattern { Idle, Walk, Knock };  //行動パターン
    public movePattern moveState = movePattern.Idle;  //現在の行動パターン

    public int id = 0;  //敵のID（種類を識別するためのもの）
    [SerializeField] protected float speed = 1f;  //移動速度
    [SerializeField] protected int maxHp = 1;  //最大HP
    protected int currentHp;  //現在のHP
    public int power = 30;  //ノックバックの力
    [SerializeField] private float knockBackMultiplier = 1.0f;  //ノックバック倍率
    public GameObject target;  //追尾ターゲット

    protected bool knockbyPlayer = false;  //プレイヤーによるノックバックを受けたかどうか
    protected HashSet<EnemyBase> hitEnemies = new HashSet<EnemyBase>();  //ノックバックを受けた敵のリスト
    protected bool knockRock = false;  //ノックバックのクールダウン中かどうか

    protected int knockScore = 0;  //この敵のスコア

    void Start()
    {
        currentHp = maxHp;  //現在のHPを最大HPで初期化
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        model = transform.GetChild(0).gameObject;  //敵のモデルへの参照を取得
        colliderObject = transform.GetChild(1).gameObject;  //敵のコライダーへの参照を取得

        //ターゲット確認
        if (target == null)
        {
            target = GameObject.Find("Prince");

            if (target == null)
                Debug.LogError("Targetが設定されていません");
        }

        if (spawnDirector == null)
        {
            spawnDirector = GameObject.Find("StageDirector").GetComponent<EnemySpawnDirector>();  //スポーンディレクターへの参照を取得
        }
        if (scoreDirector == null)
        {
            scoreDirector = GameObject.FindWithTag("Player").transform.GetChild(0).GetComponent<ScoreDirector>();  //スコアディレクターへの参照を取得
        }
        if (gameStop == null)
        {
            gameStop = GameObject.Find("Manager").GetComponent<GameStop>();  //ゲームストップへの参照を取得
        }

        //初期行動パターンを歩行にする
        moveState = movePattern.Walk;
    }

    void Update()
    {
        //ゲーム終了時の処理
        if (gameStop.isGameStop)
        {
            return;
        }

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
                else
                {
                    //ノックバックの勢いを減衰させる
                    rb.linearVelocity *= knockBackMultiplier;
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

        //動物の鳴き声を再生
        audioSource.Play();

        yield return new WaitForSeconds(0.5f);

        model.SetActive(false);  //敵のモデルを非アクティブにする
        colliderObject.SetActive(false);  //敵のコライダーを非アクティブにする

        //スポーンディレクターに敵をオブジェクトプールに返す
        spawnDirector.ReturnEnemyToPool(this.gameObject, id);

        yield return new WaitForSeconds(1.0f);

        //初期化
        ResetState();
    }

    public void EnemyRetryGame()
    {
        //初期化
        ResetState();

        //スポーンディレクターに敵をオブジェクトプールに返す
        spawnDirector.ReturnEnemyToPool(this.gameObject, id);
    }

    public virtual void ResetState()
    {
        model.SetActive(true);  //敵のモデルをアクティブにする
        colliderObject.SetActive(true);  //敵のコライダーをアクティブにする
        gameObject.SetActive(false);  //こののゲームオブジェクトを非アクティブにする
        moveState = movePattern.Idle;  //行動パターンを待機にする
        rb.linearVelocity = Vector3.zero;  //速度を0にする
        currentHp = maxHp;  //HPを最大HPに戻す
        knockScore = 0;  //スコアをリセット
        knockbyPlayer = false;  //プレイヤーによるノックバックを受けた状態をリセット
        knockRock = false;  //ノックバックのクールダウン状態をリセット
        hitEnemies.Clear();  //ノックバックを受けた敵のリストをクリア
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !knockbyPlayer && moveState != movePattern.Knock)
        {
            //プレイヤーがアタック中の場合、敵はノックバックを受ける
            //if (!collision.gameObject.GetComponent<OhajikiFlick>().canFlick)
            //{
                //Debug.Log("くぉ～ぶつかる！！");
                knockbyPlayer = true;
                MovePatternKnock();
            //スコアを加算
            knockScore = collision.gameObject.GetComponent<ScoreDirector>().AddScore();
            //}
            //else  //プレイヤーがアタック中ではない場合、プレイヤーはノックバックを受ける
            //{
            //    Debug.Log("インド人を右に！！");
            //    Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            //    Debug.Log(playerRb.linearVelocity);
            //    rb.AddForce(collision.gameObject.GetComponent<Rigidbody>().linearVelocity.normalized * power * 0.5f, ForceMode.Impulse);
            //}
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
                //連鎖スコアを加算
                if (knockScore == 0)
                    knockScore = scoreDirector.ChainScore(collision.gameObject.GetComponent<EnemyBase>().knockScore);

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
                //連鎖スコアを加算
                if (knockScore == 0)
                    knockScore = scoreDirector.ChainScore(collision.gameObject.GetComponent<EnemyBase>().knockScore);

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
