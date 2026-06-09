using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン切り替えに必要

namespace Ohajiki.Core
{
    public class SkipTutorial : MonoBehaviour
    {
        async Task Start()
        {
            if (PlayerPrefs.HasKey("IsFirstTime"))
            {
                await ServiceLocator.Resolve<IScreenFadeFacade>().FadeOut(1f);
                SceneManager.LoadScene("InGameMain");
            }
            else
            {
                PlayerPrefs.SetInt("IsFirstTime", 1);
                PlayerPrefs.Save();

                Debug.Log("初めての起動です！");
            }
        }
    }
}