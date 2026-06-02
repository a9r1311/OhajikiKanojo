using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static RankingManager;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] nameTexts;
    [SerializeField] private TextMeshProUGUI[] scoreTexts;

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

    void Start()
    {
        //ゲーム開始時にランキングを取得
        StartCoroutine(GetRanking());
    }

    //スコア送信
    public void SendScore(string name, int score)
    {
        StartCoroutine(AddScore(name, score));
    }

    IEnumerator AddScore(string name, int score)
    {
        WWWForm form = new WWWForm();  //送信するデータ
        //送信するデータを追加
        form.AddField("name", name);
        form.AddField("score", score);

        //送信リクエスト作成
        UnityWebRequest www =
            UnityWebRequest.Post("http://localhost/OhajikiGame/addScore.php", form);

        //送信
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("スコア送信成功");

            //送信後ランキング更新
            StartCoroutine(GetRanking());
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    //ランキング取得
    IEnumerator GetRanking()
    {
        //送信リクエスト作成
        UnityWebRequest www =
            UnityWebRequest.Get("http://localhost/OhajikiGame/getRanking.php");

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
    }
}
