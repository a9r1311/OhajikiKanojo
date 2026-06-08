using UnityEngine;

public class StarSideStepEnemy : EnemyBase
{
    private float sideStepTimer = 0f;  //サイドステップのタイマー
    private float sideStepInterval = 0.08f;  //サイドステップの間隔
    private float normalInterval = 1.2f;  //通常移動の間隔
    private float stepSpeed = 20f;  //サイドステップの速度
    [SerializeField] private bool rightSideStep = true;  //サイドステップの方向(右:true, 左:false)
    
    float sideStepDirection;

    void Awake()
    {
        if (!rightSideStep)
            id = 1;  //右サイドステップのID
        else
        {
            id = 2;  //左サイドステップのID

            //左サイドステップのパラメータを調整
            normalInterval = 0.3f;
            sideStepInterval = 0.15f;
            stepSpeed = 9f;
        }
    }

    protected override void MovePatternWalk()
    {
        if (target == null) return;

        transform.LookAt(target.transform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        sideStepTimer += Time.deltaTime;
        if (sideStepTimer <= normalInterval)  //通常移動
        {
            rb.linearVelocity = transform.forward * speed;
        }
        else if (sideStepTimer <= sideStepInterval + normalInterval)  //サイドステップ
        {
            //サイドステップの方向をランダムに決定
            if (rightSideStep)
            {
                sideStepDirection = 1f;
                rb.linearVelocity = transform.right * sideStepDirection * stepSpeed;
                rightSideStep = false;
            }
            else
            {
                sideStepDirection = -1f;
                rb.linearVelocity = transform.right * sideStepDirection * stepSpeed;
                rightSideStep = true;
            }
        }
        else
        {
            //タイマーをリセットして通常移動に戻る
            sideStepTimer = 0f;
        }
    }

    //初期化
    public override void ResetState()
    {
        model.transform.rotation = new Quaternion();  //モデルの回転をリセット
        isDead = false;  //死亡状態をリセット
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
        sideStepTimer = 0f;  //チャージタイマーをリセット
    }
}