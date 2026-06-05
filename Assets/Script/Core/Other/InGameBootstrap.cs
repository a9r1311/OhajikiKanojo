using UnityEngine;

namespace Ohajiki.Core
{
    public class InGameBootstrap : MonoBehaviour     //  インゲームの最初の処理をするクラス
    {
        void Start()
        {
            Debug.Log("バグってんかおい");
            ServiceLocator.Resolve<IScreenFadeFacade>().FadeIn(3f);
        }
    }
}