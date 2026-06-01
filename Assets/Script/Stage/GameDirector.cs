using System;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] private EnemySpawnDirector enemySpawnDirector;  //スポーンディレクターへの参照
    [SerializeField] private ScoreDirector scoreDirector;  //スコアディレクターへの参照
    [SerializeField] private TargetToProtect targetToProtect;  //守るべきターゲットへの参照
    [SerializeField] private ResultManager resultManager;  //リザルトマネージャーへの参照
    [SerializeField] private GameStop gameStop;  //ゲームストップへの参照

    public bool gameFinish = false;  //ゲーム終了フラグ

    void Update()
    {
        //ゲーム終了条件の確認
        if (!gameFinish && targetToProtect.currentHp <= 0)
        {
            gameFinish = true;
        }
    }
}
