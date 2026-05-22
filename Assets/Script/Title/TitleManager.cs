using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    //タイトルシーンからホームシーンへ
    public void OnCrickStratButton()
    {
        SceneLoader.Load("InGameMain_alpha");
    }

    //タイトルシーンからコンフィグへ
    public void OnCrickConfigButton()
    {
        SceneLoader.Load("ConfigScene");
    }

    //タイトルシーンからランキングへ
    public void OnCrickRankingButton()
    {
        SceneLoader.Load("RankingScene");
    }
}
