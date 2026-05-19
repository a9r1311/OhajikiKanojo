using UnityEngine;

public class ScoreDirector : MonoBehaviour
{
    [SerializeField] private int score = 0;

    private const float maxSpeed = 68f;
    private const int maxBonus = 400;

    float maxSpeedp = 0f;

    public int AddScore(float speed)
    {
        Debug.Log(speed);
        if (speed > maxSpeedp)
        {
            maxSpeedp = speed;
            Debug.Log("Max Speed: " + maxSpeedp);
        }

        //ボーナス計算(０～４００点)
        int bonus = Mathf.RoundToInt(speed / maxSpeed * maxBonus);
        bonus = Mathf.Clamp(bonus, 0, maxBonus);

        //スコア加算
        score += 100 + bonus;

        return 100 + bonus;
    }

    public int ChainScore(int enemyScore) 
    {
        //スコア加算(二倍)
        score += enemyScore * 2;

        return enemyScore * 2;
    }
}
