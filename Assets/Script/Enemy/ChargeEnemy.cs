using UnityEngine;

public class ChargeEnemy : EnemyBase
{
    private float chargeTimer = 0f;  //チャージのタイマー
    private float chargeTimeLimit = 3f;  //チャージのタイムリミット
    private float chargeMoveSpeed = 0.2f;  //チャージ後の加速度
    private float currentSpeed = 0.1f;  //現在の速度

    protected override void MovePatternWalk()
    {
        if (target == null) return;

        transform.LookAt(target.transform);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        chargeTimer += Time.deltaTime;
        if (chargeTimer < chargeTimeLimit)  //チャージ中
        {
             　
        }
        else  //チャージ解放
        {
            //徐々に加速する
            currentSpeed = currentSpeed + chargeMoveSpeed;
            rb.linearVelocity = transform.forward * currentSpeed;
        }
    }
}
