using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyContorllerBase : MonoBehaviour
{
    public int ID { get; set; }
    public int knockScore;
    public bool isknockBack; 

    public movePattern moveState;
    protected HashSet<EnemyBase> hitEnemies = new HashSet<EnemyBase>();    //ノックバックを受けた敵のリスト
}
