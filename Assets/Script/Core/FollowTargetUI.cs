using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Ohajiki.Core
{
    public class FollowTargetUI : MonoBehaviour    //  オブジェクトを追従するUI
    {
        public Transform target;    //  ターゲット

        private RectTransform rectTransform;
        private Camera mainCamera;

        [SerializeField] private float indicateSpeed_First;    //  表示スピード
        [SerializeField] private float indicateSpeed_Second;    //  表示スピード
        [SerializeField] private float indicateSpeed_HalfSecond;    //  表示スピード
        [SerializeField] private float indicateSpeed_Third;    //  表示スピード
        [SerializeField] private float indicateSpeed_Forth;    //  表示スピード
        [SerializeField] private float indicateSpeed_Fifth;    //  表示スピード
        [SerializeField] private float indicateSpeed_Sixth;    //  表示スピード
        private TextMeshProUGUI textElement;
        [SerializeField] string fullText_First;
        [SerializeField] string fullText_Second;
        [SerializeField] string fullText_HalfSecond;
        [SerializeField] string fullText_Third;
        [SerializeField] string fullText_Forth;
        [SerializeField] string fullText_Fifth;
        [SerializeField] string fullText_Sixth;

        [SerializeField] OhajikiFlick flick;
        [SerializeField] EscortWarp warp;
        [SerializeField] EnemySpawnDirector spawnDirector;

        private string currentText = "";

        public bool isAppearEnemyTimimig = false;

        bool isClicked = false;
        public bool explaind = false;

        void Awake()
        {
            textElement = GetComponent<TextMeshProUGUI>();
            textElement.text = "";
        }

        void Start()
        {
            StartCoroutine(ShowText());
            rectTransform = GetComponent<RectTransform>();
            mainCamera = Camera.main;
            spawnDirector.canSpawn = false;
        }

        void LateUpdate()
        {
            if (target == null) return;

            rectTransform.localScale = Vector3.one;

            if (Pointer.current != null && Pointer.current.press.isPressed)
            {
                isClicked = true;
                Debug.Log("Clicked");
            }
        }

        IEnumerator ShowText()    //  テキストゆっくり表示
        {
            for (int i = 0; i <= fullText_First.Length; i++)
            {
                currentText = fullText_First.Substring(0, i);
                textElement.text = currentText;

                yield return new WaitForSeconds(indicateSpeed_First);
            }

            while (!flick.isDragging)
            {
                yield return null;
            }

            isAppearEnemyTimimig = true;

            yield return new WaitForSeconds(5f);

            currentText = "";
            Debug.Log("What");

            for (int i = 0; i <= fullText_Second.Length; i++)
            {
                currentText = fullText_Second.Substring(0, i);
                textElement.text = currentText;

                yield return new WaitForSeconds(indicateSpeed_Second);
            }

            yield return new WaitForSeconds(4f);

            currentText = "";

            for (int i = 0; i <= fullText_HalfSecond.Length; i++)
            {
                currentText = fullText_HalfSecond.Substring(0, i);
                textElement.text = currentText;

                yield return new WaitForSeconds(indicateSpeed_HalfSecond);
            }
            
            while (!warp.isWarped)
            {
                yield return null;
            }

            yield return new WaitForSeconds(3f);

            currentText = "";
            //ここからスポーンするように
            spawnDirector.canSpawn = true;

            for (int i = 0; i <= fullText_Third.Length; i++)
            {
                currentText = fullText_Third.Substring(0, i);
                textElement.text = currentText;

                yield return new WaitForSeconds(indicateSpeed_Third);
            }

            currentText = "";
            yield return new WaitForSeconds(4.7f);

            for (int i = 0; i <= fullText_Forth.Length; i++)
            {
                currentText = fullText_Forth.Substring(0, i);
                textElement.text = currentText;

                yield return new WaitForSeconds(indicateSpeed_Forth);
            }

            yield return new WaitForSeconds(1.5f);
            explaind = true;

            for (int i = 0; i <= fullText_Fifth.Length; i++)
            {
                currentText = fullText_Fifth.Substring(0, i);
                textElement.text = currentText;

                yield return new WaitForSeconds(indicateSpeed_Fifth);
            }

            currentText = "";

            yield return new WaitForSeconds(3f);

            ServiceLocator.Resolve<IScreenFadeFacade>().FadeOut(5f);

            for (int i = 0; i <= fullText_Sixth.Length; i++)
            {
                currentText = fullText_Sixth.Substring(0, i);
                textElement.text = currentText;

                yield return new WaitForSeconds(indicateSpeed_Sixth);
            }

            yield return new WaitForSeconds(2f);

            SceneManager.LoadScene("InGameMain");
        }
    }
}