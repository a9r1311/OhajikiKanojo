using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnDirector : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnEnemy = new List<GameObject>();  //スポーンする敵のリスト
    [SerializeField] private GameObject target;  //敵の追尾ターゲット

    private List<Queue<GameObject>> waitingEnemies;  //オブジェクトプール用の待機中の敵のキューリスト
    private int poolCount = 3;  //とりあえず3体ずつ用意しておく

    private float spawnInterval = 2.5f;  //スポーン間隔
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

        waitingEnemies = new List<Queue<GameObject>>();  //初期化
        Vector3 waitingPos = new Vector3(100f, 0f, 100f);  //初期待機位置

        for (int i = 0; i < spawnEnemy.Count; i++) 
        {
            //敵の種類ごとにオブジェクトプール用のキューを作成
            waitingEnemies.Add(new Queue<GameObject>());
            for (int j = 0; j < poolCount; j++)
            {
                //敵をスポーン
                GameObject enemy = Instantiate(spawnEnemy[i], waitingPos, Quaternion.identity);
                //スポーンした敵のターゲットを設定
                enemy.GetComponent<EnemyBase>().target = target;
                //敵を非アクティブにして待機キューに追加
                waitingEnemies[enemy.GetComponent<EnemyBase>().id].Enqueue(enemy);
                enemy.SetActive(false);
            }
        }
    }

    void Update()
    {
        //スポーンタイマー
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            if(spawnInterval > 1.0f)
                spawnInterval -= 0.02f;  //スポーン間隔を徐々に短くする
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
            Vector3 spawnPos = GetSpawnPosition(index);
            //敵をスポーン
            if (waitingEnemies[index].Count <= 0)  //オブジェクトプールが足りない場合は新たにスポーン
            {
                Debug.LogWarning("オブジェクトプールが足りません！");
                GameObject enemy = Instantiate(spawnEnemy[index], spawnPos, Quaternion.identity);
                //スポーンした敵のターゲットを設定
                enemy.GetComponent<EnemyBase>().target = target;
            }
            else  //オブジェクトプールから敵を出してスポーン
            {
                GameObject enemy = waitingEnemies[index].Dequeue();
                enemy.transform.position = spawnPos;
                enemy.SetActive(true);
                enemy.GetComponent<EnemyBase>().moveState = EnemyBase.movePattern.Walk;  //行動パターンを歩きにする
            }
        }
    }

    //スポーン位置をランダムに決定
    private Vector3 GetSpawnPosition(int id)
    {
        float targetRadius = 6f;  //ターゲットを中心とした半径
        float width = 20f;        //スポーン位置の横幅
        float height = 13f;       //スポーン位置の奥行き
        Vector3 targetPos = target.transform.position;  //ターゲットの位置
        Vector3 spawnPos = Vector3.zero;

        do
        {
            float x = Random.Range(targetPos.x - width / 2f, targetPos.x + width / 2f);
            float z = Random.Range(targetPos.z - height / 2f, targetPos.z + height / 2f) + 3f;

            spawnPos = new Vector3(x, 1f, z);

        } while (Vector3.Distance(spawnPos, targetPos) < targetRadius
                 || (id == 3 && spawnPos.z < targetPos.z));  //ターゲットから一定距離以上の位置かつ、id3はターゲットより前方なら通す

        return spawnPos;
    }

    //敵をオブジェクトプールに戻す
    public void ReturnEnemyToPool(GameObject enemy, int index)
    {
        enemy.SetActive(false);
        waitingEnemies[index].Enqueue(enemy);
    }
}
