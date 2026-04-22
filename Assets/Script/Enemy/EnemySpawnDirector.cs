using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnDirector : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnEnemy = new List<GameObject>();  //スポーンする敵のリスト
    [SerializeField] private GameObject target;  //敵の追尾ターゲット

    private float spawnInterval = 2f;  //スポーン間隔
    private float spawnTimer = 0f;  //スポーンタイマー


    void Start()
    {
        //ターゲット確認
        if (target == null)
        {
            target = GameObject.Find("Prince");

            if (target == null)
                Debug.LogError("Targetが設定されていません");
        }
    }

    void Update()
    {
        //スポーンタイマー
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            //敵をスポーン
            SpawnEnemy();
        }
    }

    //敵をスポーンさせる
    private void SpawnEnemy()
    {
        if (spawnEnemy.Count > 0)
        {
            //出現させる敵をランダムに選択
            int index = Random.Range(0, spawnEnemy.Count);
            //出現座標をランダムに決定
            Vector3 spawnPos = GetSpawnPosition();
            //敵をスポーン
            GameObject enemy = Instantiate(spawnEnemy[index], spawnPos, Quaternion.identity);
            //スポーンした敵のターゲットを設定
            enemy.GetComponent<EnemyBase>().target = target;
        }
    }

    //スポーン位置をランダムに決定
    private Vector3 GetSpawnPosition()
    {
        float targetRadius = 5f;  //ターゲットを中心とした半径
        float width = 13f;        //スポーン位置の横幅
        float height = 7f;        //スポーン位置の奥行き
        Vector3 spawnPos = Vector3.zero;

        do
        {
            float x = Random.Range(-width / 2f, width / 2f);
            float z = Random.Range(-height / 2f, height / 2f);

            spawnPos = new Vector3(x, 1f, z);

        } while (Vector3.Distance(spawnPos, target.transform.position) < targetRadius);  //ターゲットから一定距離以上の位置なら通す

        return spawnPos;
    }
}
