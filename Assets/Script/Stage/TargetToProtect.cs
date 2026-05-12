using UnityEngine;

public class TargetToProtect : MonoBehaviour
{
    [SerializeField] private int hp = 3;  //ターゲットのHP

    //ターゲットが攻撃されたときの処理
    private void Damage()
    {
        Debug.Log("ターゲットが攻撃されました！");
        //HPを減らす
        if (--hp <= 0)
        {
            Debug.Log("お前が殺した");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //敵がターゲットに接触したときの処理
            Damage();
            //敵を破壊する
            Destroy(other.transform.parent.gameObject);
        }
    }
}
