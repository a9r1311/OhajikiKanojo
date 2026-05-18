using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RankingManager : MonoBehaviour
{
    public TextMeshProUGUI rankingText;  //ランキング表示用テキスト

    void Start()
    {
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
            rankingText.text = www.downloadHandler.text;
        }
        else
        {
            Debug.LogError(www.error);
        }
    }
}
