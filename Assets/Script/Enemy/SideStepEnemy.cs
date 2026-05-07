using UnityEngine;

public class SideStepEnemy : EnemyBase
{
    private float sideStepTimer = 0f;  //サイドステップのタイマー
    private float sideStepInterval = 0.15f;  //サイドステップの間隔
    private float normalInterval = 1.7f;  //通常移動の間隔
    private float stepSpeed = 10f;  //サイドステップの速度
    [SerializeField] private bool rightSideStep = true;  //サイドステップの方向(右:true, 左:false)

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
}
