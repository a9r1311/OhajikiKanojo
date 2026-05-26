using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnDirector : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnEnemy = new List<GameObject>();  //スポーンする敵のリスト
    [SerializeField] private GameObject target;  //敵の追尾ターゲット
    private ScoreDirector scoreDirector;  //スコアディレクター
    private GameDirector gameDirector;  //ゲームディレクター

    private List<Queue<GameObject>> waitingEnemies;  //オブジェクトプール用の待機中の敵のキューリスト
    private int poolCount = 3;  //とりあえず3体ずつ用意しておく
    
    private float spawnInterval = 5f;  //スポーン間隔
    private float minSpawnInterval = 0.4f;  //スポーン間隔の最小値
    private float spawnIntervalDecreaseRate = 0.04f;  //スポーン間隔の減少率
    private float spawnTimer = 0f;  //スポーンタイマー

    [SerializeField] private bool isLongDistanceSpawn = false;  //遠距離スポーンフラグ
    [SerializeField] private int spawnTypeCount = 1;  //スポーンさせる敵の種類数
    private int increaseSpawnTypeInterval = 15;  //スポーンさせる敵の種類を増やす間隔
    private int spawnCounter = 0;  //スポーンカウンター

    void Start()
    {
        //ターゲット確認
        if (target == null)
        {
            target = GameObject.Find("Prince");

            if (target == null)
                Debug.LogError("Targetが設定されていません");
        }

        //スコアディレクター確認
        scoreDirector = gameObject.GetComponent<ScoreDirector>();
        //ゲームディレクター確認
        gameDirector = gameObject.GetComponent<GameDirector>();

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
                EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
                //スポーンした敵に色々設定
                enemyBase.target = target;
                enemyBase.spawnDirector = this;
                enemyBase.scoreDirector = scoreDirector;
                //敵を非アクティブにして待機キューに追加
                waitingEnemies[enemyBase.id].Enqueue(enemy);
                enemy.SetActive(false);
            }
        }
    }

    void Update()
    {
        //ゲームストップ中はスポーンさせない
        //if (gameStop.isGameStop)
        if (gameDirector.gameFinish)
            return;

        //スポーンタイマー
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;

            if(spawnInterval > minSpawnInterval)
                spawnInterval -= spawnIntervalDecreaseRate;  //スポーン間隔を徐々に短くする

            if(spawnCounter >= increaseSpawnTypeInterval && spawnTypeCount < spawnEnemy.Count)
            {
                spawnCounter = 0;
                spawnTypeCount++;  //スポーンさせる敵の種類を増やす
            }
            else
            {
                spawnCounter++;
            }

            //敵をスポーン
            SpawnEnemy(false);

            //遠距離スポーンの処理
            if (isLongDistanceSpawn)
            {
                SpawnEnemy(true);
            }
        }
    }

    //敵をスポーンさせる
    private void SpawnEnemy(bool longDistance)
    {
        if (spawnEnemy.Count > 0)
        {
            int index;
            //出現させる敵をランダムに選択
            if (!longDistance)
                index = Random.Range(0, spawnTypeCount);
            else
                //遠距離スポーンは特定の敵しか出さないようにする
                do
                    index = Random.Range(0, spawnTypeCount);
                while (index == spawnEnemy.Count - 1);  //遠距離スポーンは最後の敵を出さない
            

            //出現座標をランダムに決定
            Vector3 spawnPos = GetSpawnPosition(index, longDistance);

            //敵をスポーン
            if (waitingEnemies[index].Count <= 0)  //オブジェクトプールが足りない場合は新たにスポーン
            {
                //敵を生成
                GameObject enemy = Instantiate(spawnEnemy[index], spawnPos, Quaternion.identity);
                EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
                //スポーンした敵に色々設定
                enemyBase.target = target;
                enemyBase.spawnDirector = this;
                enemyBase.scoreDirector = scoreDirector;
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
    private Vector3 GetSpawnPosition(int id, bool longDistance)
    {
        float targetRadius = 9f;  //ターゲットを中心とした半径
        float width = 25f;        //スポーン位置の横幅
        float height = 18f;       //スポーン位置の奥行き
        Vector3 targetPos = target.transform.position;  //ターゲットの位置
        Vector3 spawnPos = Vector3.zero;

        if (longDistance)
        {
            targetRadius = 20f;  //ターゲットを中心とした半径
            width = 28f;         //スポーン位置の横幅
            height = 35f;        //スポーン位置の奥行き
        }

        do
        {
            float x = Random.Range(targetPos.x - width / 2f, targetPos.x + width / 2f);
            float z = Random.Range(targetPos.z - height / 2f, targetPos.z + height / 2f) + 6f;

            spawnPos = new Vector3(x, 1f, z);

        } while (Vector3.Distance(spawnPos, targetPos + new Vector3(0f, 0f, 2f)) < targetRadius
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
