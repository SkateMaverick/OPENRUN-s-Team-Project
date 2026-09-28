using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nicrom.NHP
{
    public class Quit : MonoBehaviour
    {

        public bool quitInEditor = false;
        private float count = 10f;

        // Update is called once per frame
        void Update()
        {
            count += Time.deltaTime;

            if (Input.GetKeyDown("escape"))
            {
                QuitGame();

                count = 0;
            }
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            if (count < 2.5f && quitInEditor)
            {
                EditorApplication.isPlaying = false;
            }
#else
                Application.Quit();
#endif
        }
    }
}
