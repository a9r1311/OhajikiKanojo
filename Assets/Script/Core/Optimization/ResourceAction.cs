namespace Ohajiki.Core
{
    public class ResourceAction : ResourceActionBase    //  リソースに関する関数を持つ
    {
        public ResourceAction()
        {

        }

        public override void UnloadUnused()    //  参照されていないアセットを開放
        {
            base.UnloadUnused();
        }
    }
}