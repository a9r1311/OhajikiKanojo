using UnityEngine;

namespace Andtech.StarPack
{

	public class ActivateDepthRendering : MonoBehaviour
	{

		private void Start()
		{
            UnityEngine.Camera.main.depthTextureMode = DepthTextureMode.Depth;
        }
	}
}
