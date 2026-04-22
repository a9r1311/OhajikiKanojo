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
    public GameObject target;  //追尾ターゲット

    private bool knockedByEnemy = false;

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

        currentMovePattern = movePattern.Walk;
    }

    void Update()
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
                    Debug.Log("ノックバック終了");
                    currentMovePattern = movePattern.Walk;
                    knockedByEnemy = false;
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
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);  //Y軸のみ回転
            rb.linearVelocity = transform.forward * speed;
        }
    }

    private void MovePatternKnock()
    {
        currentHp--;  //HPを減らす
        if (currentHp <= 0)  //HPが0未満になったら死亡処理
        {
            DeadEnemy();
        }

        //ノックバック行動に移行
        currentMovePattern = movePattern.Knock;
        //ノックバックの勢いを減衰させる
        rb.linearVelocity *= knockBackMultiplier;
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
            MovePatternKnock();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

            if (!knockedByEnemy && enemy.currentMovePattern == movePattern.Knock)
            {
                knockedByEnemy = true;
                MovePatternKnock();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

            if (!knockedByEnemy && enemy.currentMovePattern == movePattern.Knock)
            {
                knockedByEnemy = true;
                MovePatternKnock();
            }
        }
    }
}
