using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private Rigidbody rb;

    public enum movePattern { Idle, Walk, Knock };  //行動パターン
    public movePattern currentMovePattern = movePattern.Idle;  //現在の行動パターン

    [SerializeField] private float speed = 1f;  //移動速度
    [SerializeField] private int maxHp = 1;  //最大HP
    private int currentHp;  //現在のHP
    [SerializeField] private float knockBackMultiplier = 1.0f;  //ノックバック倍率
    [SerializeField] private GameObject target;  //追尾ターゲット

    void Start()
    {
        currentHp = maxHp;  //現在のHPを最大HPで初期化
        rb = GetComponent<Rigidbody>();

        //ターゲット確認
        if (target == null)
            Debug.LogError("Targetが設定されていません");

        currentMovePattern = movePattern.Walk;
    }

    void FixedUpdate()
    {
        //行動パターンに応じた処理
        switch (currentMovePattern)
        {
            case movePattern.Idle:  //待機行動
                break;

            case movePattern.Knock:  //ノックバック行動
                //ノックバックの勢いが弱まったら歩行行動に移行
                if (rb.linearVelocity.magnitude < 0.01f)
                {
                    currentMovePattern = movePattern.Walk;
                    //Debug.Log("Walkに移行");
                }
                break;

            case movePattern.Walk:  //歩行行動
                //ターゲットに向かって移動
                MovePatternWalk();
                break;
        }
    }

    //歩行行動
    private void MovePatternWalk()
    {
        if (target != null)
        {
            transform.LookAt(target.transform);
            rb.linearVelocity = transform.forward * speed;
        }
    }

    //hpが0未満になったときの死亡処理
    private void DeadEnemy()
    {
        //仮の死亡処理
        Destroy(gameObject);
        
        return;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currentHp--;  //HPを減らす
            if (currentHp < 0)  //HPが0未満になったら死亡処理
            {
                DeadEnemy();
            }

            //ノックバック行動に移行
            currentMovePattern = movePattern.Knock;
            //Debug.Log("Knockに移行");
            //ノックバックの勢いを減衰させる
            rb.linearVelocity *= knockBackMultiplier;
        }
    }
}
