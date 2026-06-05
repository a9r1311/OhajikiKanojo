using UnityEngine;

namespace Ohajiki.Core
{
    public class TutorialBootstrap : MonoBehaviour    //  チュートリアル
    {
        void Start()
        {
            ServiceLocator.Resolve<IScreenFadeFacade>().FadeOut(3f);
        }
    }
}