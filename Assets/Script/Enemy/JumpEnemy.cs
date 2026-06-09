using UnityEngine;

public class JumpEnemy : EnemyBase
{
    private float jumpTimer = 0f;  //ジャンプのタイマー
    private float jumpInterval = 0.19f;  //ジャンプの間隔
    private float normalInterval = 1.5f;  //通常移動の間隔
    private float jumpSpeed = 10f;  //ジャンプの速度
    private bool hasJumped = false;  //ジャンプしたかどうかのフラグ

    void Awake()
    {

    }

    protected override void MovePatternWalk()
    {
        if (target == null) return;

        transform.LookAt(target.transform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        jumpTimer += Time.deltaTime;
        if (jumpTimer <= normalInterval)  //通常移動
        {
            rb.linearVelocity = transform.forward * speed;
        }
        else if (jumpTimer <= jumpInterval + normalInterval)  //ジャンプ
        {
            //ジャンプの処理
            if (!hasJumped)
            {
                Vector3 jumpDirection = (transform.forward + Vector3.up).normalized;

                rb.AddForce(jumpDirection * jumpSpeed, ForceMode.VelocityChange);

                hasJumped = true;
            }
        }
        else
        {
            //タイマーをリセットして通常移動に戻る
            jumpTimer = 0f;
            hasJumped = false;
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
        jumpTimer = 0f;  //チャージタイマーをリセット
    }
}
