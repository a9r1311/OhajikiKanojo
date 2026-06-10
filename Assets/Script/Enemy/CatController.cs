using System.Collections;
using UnityEngine;

public class CatController : EnemyContorllerBase
{
    EnemySpawnDirector spawnDirector;
    ScoreDirector scoreDirector;
    [SerializeField] AudioSource audioSource;

    GameObject target;
    Transform catTF;

    [SerializeField] float moveSpeed;
    [SerializeField] int catKnockScore;
    int catID = 4;

    bool isKnockingPlayer = false;

    void Start()
    {
        spawnDirector = GameObject.Find("StageDirector").GetComponent<EnemySpawnDirector>();
        scoreDirector = GameObject.Find("ScoreDirector").GetComponent<ScoreDirector>();
        
        base.ID = catID;
        base.knockScore = catKnockScore;
        base.moveState = movePattern.Idle;

        target = spawnDirector.target_forcat;
        catTF = this.gameObject.transform;
    }

    void Update()
    {
        if(spawnDirector == null)
        { return; }

        if(base.moveState == movePattern.Idle)
        { return; }


        catTF.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isKnockingPlayer && moveState != movePattern.Knock)
        {
            OhajikiFlick player = collision.gameObject.GetComponent<OhajikiFlick>();

            //プレイヤーがアタック中の場合、敵はノックバックを受ける
            if (player.isAttacking)
            {
                isKnockingPlayer = true;  //プレイヤーによるノックバックを受けた状態にする
                StartCoroutine(Cycle(catTF));
                StartCoroutine(DeadEnemy());
                base.isknockBack = true;

                if (scoreDirector != null)
                {
                    scoreDirector.chainCount++;  //連鎖カウントを増やす

                    //スコアを加算
                    if (knockScore <= 1)
                        knockScore = scoreDirector.AddScore();
                    else
                        knockScore = scoreDirector.ChainScore(scoreDirector.playerKnockScore, scoreDirector.chainCount);
                }
            }
        }

        else if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

            //ノックバック行動中の敵に衝突した場合、ノックバックを受ける
            if ((enemy.moveState == movePattern.Knock || moveState == movePattern.Knock) && !hitEnemies.Contains(enemy))
            {
                //ノックバックを受けた敵がリストにない場合、リストに追加
                base.hitEnemies.Add(enemy);

                //連鎖スコアを加算
                if (knockScore == 0 && scoreDirector != null)
                    knockScore = scoreDirector.ChainScore(collision.gameObject.GetComponent<EnemyContorllerBase>().knockScore, 2);

                StartCoroutine(Cycle(catTF));
                StartCoroutine(DeadEnemy());
            }
        }
    }
    private IEnumerator KnockCoolDown()
    {
        yield return new WaitForSeconds(0.5f);  //ノックバックのクールダウン時間
        base.isknockBack = false;  //クールダウン終了
    }

    private IEnumerator Cycle(Transform target)
    {
        float timer = 0f;  //回転時間
        float rotateY = Random.value < 0.5f ? -180f : 180f;  //回転方向をランダムに決定
        float rotateSpeed = Random.Range(5f, 20f);  //回転速度をランダムに決定

        //回転処理
        while (timer < 10.0f)
        {
            //回転方向に応じてY軸を回転させる
            target.Rotate(0f, rotateY * rotateSpeed * Time.deltaTime, 0f);

            timer += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator DeadEnemy()
    {
        //動物の鳴き声を再生
        audioSource.Play();

        yield return new WaitForSeconds(0.5f);

        this.gameObject.SetActive(false);  //敵のモデルを非アクティブにする

        //スポーンディレクターに敵をオブジェクトプールに返す
        if (spawnDirector != null)
        {
            spawnDirector.ReturnEnemyToPool(this.gameObject, base.ID);
            spawnDirector.RemoveActiveEnemy(this.gameObject);
        }

        yield return new WaitForSeconds(1.0f);

        //初期化
        //ResetState();
    }
}
