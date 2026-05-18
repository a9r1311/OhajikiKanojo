using UnityEngine;

public class SideStepEnemy : EnemyBase
{
    private float sideStepTimer = 0f;  //サイドステップのタイマー
    private float sideStepInterval = 0.2f;  //サイドステップの間隔
    private float normalInterval = 1.1f;  //通常移動の間隔
    private float stepSpeed = 17f;  //サイドステップの速度
    [SerializeField] private bool rightSideStep = true;  //サイドステップの方向(右:true, 左:false)

    void Awake()
    {
        if(rightSideStep)
            id = 1;  //右サイドステップのID
        else
            id = 2;  //左サイドステップのID
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
            float sideStepDirection = rightSideStep ? 1f : -1f;
            rb.linearVelocity = transform.right * sideStepDirection * stepSpeed;
        }
        else
        {
            //タイマーをリセットして通常移動に戻る
            sideStepTimer = 0f;
        }
    }

    //初期化
    protected override void Reset()
    {
        moveState = movePattern.Idle;  //行動パターンを待機にする
        rb.linearVelocity = Vector3.zero;  //速度を0にする
        currentHp = maxHp;  //HPを最大HPに戻す
        sideStepTimer = 0f;  //チャージタイマーをリセット
    }
}
