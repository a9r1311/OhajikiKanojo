using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] ChangeGame change;
    public void OnCrickTitleButton()
    {
        change.GoTitle();
    }
}
