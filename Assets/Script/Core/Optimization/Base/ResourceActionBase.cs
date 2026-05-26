using UnityEngine;

namespace Ohajiki.Core
{
    public abstract class ResourceActionBase    //  リソース管理クラス
    {

        public ResourceActionBase()
        {

        }

        public virtual void UnloadUnused()    //  参照されていないアセットを開放
        {
            Resources.UnloadUnusedAssets();
        }
    }
}