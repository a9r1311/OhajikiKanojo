using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] ChangeGame change;
    //タイトルからインゲーム画面へ
    public void OnCrickStratButton()
    {
        change.GoInGame();
    }

    //コンフィグ呼び出し
    public void OnCrickConfigButton()
    {
        
    }

    //ランキング呼び出し
    public void OnCrickRankingButton()
    {
        
    }
}
