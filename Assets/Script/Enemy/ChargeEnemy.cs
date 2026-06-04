using UnityEngine;
using System.Collections;

public class ChargeEnemy : EnemyBase
{
    private float chargeTimer = 0f;  //チャージのタイマー
    private float chargeTimeLimit = 4f;  //チャージのタイムリミット
    private float chargeMoveSpeed = 0.2f;  //チャージ後の加速度
    private float currentSpeed = 0.1f;  //現在の速度

    void Awake()
    {
        id = 3;  //チャージのID
    }

    protected override void MovePatternWalk()
    {
        if (target == null) return;

        //ターゲットの方向を向く
        transform.LookAt(target.transform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        if (chargeTimer < chargeTimeLimit)  //チャージ中
        {
            chargeTimer += Time.deltaTime;
            rb.linearVelocity = Vector3.zero;
        }
        else  //チャージ解放
        {
            //徐々に加速する
            currentSpeed = currentSpeed + chargeMoveSpeed;
            rb.linearVelocity = transform.forward * currentSpeed;
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
        chargeTimer = 0f;  //チャージタイマーをリセット
        currentSpeed = 0.1f;  //現在の速度を初期値に戻す
    }
}
