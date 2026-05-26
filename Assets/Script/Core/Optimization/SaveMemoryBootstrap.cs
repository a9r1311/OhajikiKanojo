using UnityEngine;

namespace Ohajiki.Core
{
    public class SaveMemoryBootstrap : MonoBehaviour    //  動的にメモリを解法
    {
        ResourceActionBase resourceAction;    //  リソース系の関数を持つクラス
        
        void Awake()
        {
            resourceAction = new ResourceAction() as ResourceActionBase;
            
            ServiceLocator.Register<ResourceActionBase>(resourceAction);
        }
    }
}