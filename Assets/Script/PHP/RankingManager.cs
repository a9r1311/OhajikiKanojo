using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] nameTexts;
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField] private TextMeshProUGUI playerRankText;

    private bool isRankingUpdated = false;  //ランキングデータが更新されたかどうか

    //ランキングデータクラス
    [System.Serializable]
    public class RankingData
    {
        public int rank;
        public string name;
        public int score;
    }

    //ランキングリストクラス
    [System.Serializable]
    public class RankingList
    {
        public RankingData[] rankings;
    }

    [System.Serializable]
    public class RankResult
    {
        public int rank;
    }

    void Start()
    {
        //ゲーム開始時にランキングを取得
        StartCoroutine(GetRanking());
    }

    //スコア送信
    public void SendScore(string name, int score)
    {
        isRankingUpdated = false;  //ランキングデータが更新される前にプレイヤーの順位を取得できないようにするため

        StartCoroutine(AddScore(name, score));
    }

    //ランキング更新
    public void UpdateRanking()
    {
        if (isRankingUpdated)
        {
            isRankingUpdated = false;

            StartCoroutine(GetRanking());
        }
    }

    IEnumerator AddScore(string name, int score)
    {
        WWWForm form = new WWWForm();  //送信するデータ
        //送信するデータを追加
        form.AddField("name", name);
        form.AddField("score", score);

        //送信リクエスト作成
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/OhajikiGame/addScore.php", form);

        //送信
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("スコア送信成功");

            RankResult result = JsonUtility.FromJson<RankResult>(www.downloadHandler.text);

            //送信後ランキング更新
            StartCoroutine(GetRanking());

            //プレイヤーの順位を取得して表示
            //string playerRank = GetPlayerRank(score);
            playerRankText.text = result.rank + "位";
        }
        else
        {
            Debug.LogError(www.error);
            playerRankText.text = "?位";
        }
    }

    //ランキング取得
    IEnumerator GetRanking()
    {
        //送信リクエスト作成
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/OhajikiGame/getRanking.php");

        //送信
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            //ランキングデータをJSONからクラスに変換
            RankingList list = JsonUtility.FromJson<RankingList>(www.downloadHandler.text);

            //ランキングデータをテキストに
            for (int i = 0; i < nameTexts.Length; i++)
            {
                //ランキングデータが存在する場合は表示、存在しない場合は---を表示
                if (i < list.rankings.Length)
                {
                    RankingData data = list.rankings[i];

                    nameTexts[i].text = $"{data.name}";
                    scoreTexts[i].text = $"{data.score}";
                }
                else
                {
                    nameTexts[i].text = $"----------";
                    scoreTexts[i].text = $"---";
                }
            }
        }
        else
        {
            Debug.LogError(www.error);
        }

        //ランキングデータが更新されたことを通知
        isRankingUpdated = true;
        Debug.Log("ランキングデータが更新されました");
    }
}
