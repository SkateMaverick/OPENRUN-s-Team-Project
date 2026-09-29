using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Unity.AI.Assistant.PlayModeTest
{
    [InitializeOnLoad]
    internal static class PlayModeTestRunner
    {
        private const string StateKey = "PlayModeTest.State";
        private const string ResultKey = "PlayModeTest.Result";
        private const string ScriptPathKey = "PlayModeTest.ScriptPath";
        private const string SentinelLog = "PLAY_MODE_TEST_COMPLETE";

        private static List<string> _allLogs = new List<string>();

        static PlayModeTestRunner()
        {
            string state = SessionState.GetString(StateKey, "Idle");

            switch (state)
            {
                case "WaitingForCompile":
                    EditorApplication.delayCall += () =>
                    {
                        SessionState.SetString(StateKey, "EnteringPlayMode");
                        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                        EditorApplication.isPlaying = true;
                    };
                    break;

                case "EnteringPlayMode":
                    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                    if (EditorApplication.isPlaying)
                    {
                        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                        SessionState.SetString(StateKey, "InPlayMode");
                        Application.logMessageReceived += HandleLog;
                        EditorApplication.update += RunTestUpdate;
                    }
                    break;

                case "InPlayMode":
                    if (EditorApplication.isPlaying)
                    {
                        Application.logMessageReceived += HandleLog;
                        EditorApplication.update += RunTestUpdate;
                    }
                    break;

                case "Done":
                    Debug.Log(SentinelLog);
                    EditorApplication.delayCall += SelfDestruct;
                    break;
            }
        }

        private static void HandleLog(string log, string stackTrace, LogType type)
        {
            _allLogs.Add("[" + type + "] " + log);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                SessionState.SetString(StateKey, "InPlayMode");
                Application.logMessageReceived += HandleLog;
                EditorApplication.update += RunTestUpdate;
            }
        }

        private static int _step = 0;
        private static int _frameCounter = 0;
        private static float _timer = 0f;
        private static bool _dialogueStarted = false;
        private static TestResult _res = new TestResult();

        private static void RunTestUpdate()
        {
            _frameCounter++;
            _timer += Time.unscaledDeltaTime;

            // Step 0: In Dungeon 4, pick up crystal
            if (_step == 0)
            {
                if (_frameCounter < 5) return;
                _step = 1;
                _frameCounter = 0;
                _timer = 0f;

                var crystal = GameObject.Find("Crystal");
                if (crystal != null)
                {
                    var cp = crystal.GetComponent<CrystalPickup>();
                    if (cp != null) cp.PickUp();
                }
                _res.pickedUpCrystal = CrystalPickup.IsCrystalAcquired;
                Debug.Log("[Test] Picked up crystal: " + _res.pickedUpCrystal);

                SceneManager.LoadScene("SingleMain");
                return;
            }

            // Step 1: Wait for SingleMain to load
            if (_step == 1)
            {
                if (SceneManager.GetActiveScene().name != "SingleMain") return;
                _frameCounter++;
                if (_frameCounter < 10) return;

                _step = 2;
                _frameCounter = 0;
                _timer = 0f;

                var intro = Object.FindFirstObjectByType<IntroWalkCutsceneController>();
                _res.isDungeonReturn = intro != null && intro.CheckIsDungeonReturn();
                Debug.Log("[Test] In SingleMain. isDungeonReturn: " + _res.isDungeonReturn);
                return;
            }

            // Step 2: In SingleMain, wait for dialogue to start, then complete it
            if (_step == 2)
            {
                if (!_dialogueStarted)
                {
                    if (DialogueCutsceneManager.Instance != null && DialogueCutsceneManager.Instance.IsDialogueActive)
                    {
                        _dialogueStarted = true;
                        _res.dialogueStarted = true;
                        Debug.Log("[Test] Dialogue started!");
                    }
                    return;
                }

                if (DialogueCutsceneManager.Instance != null && DialogueCutsceneManager.Instance.IsDialogueActive)
                {
                    DialogueCutsceneManager.Instance.AdvanceOrComplete();
                }
                else
                {
                    // Dialogue finished, black fade ending has started!
                    _step = 3;
                    _frameCounter = 0;
                    _timer = 0f;
                    _res.dialogueCompleted = true;
                    Debug.Log("[Test] Dialogue completed! Waiting for black fade ending...");
                }
                return;
            }

            // Step 3: Verify black fade ending is active, then simulate return to menu
            if (_step == 3)
            {
                var titleCard = ChapterTitleCardUI.Instance ?? Object.FindFirstObjectByType<ChapterTitleCardUI>();
                if (titleCard != null)
                {
                    var cg = titleCard.GetComponent<CanvasGroup>();
                    var overlay = titleCard.transform.Find("BlackOverlay")?.GetComponent<UnityEngine.UI.Image>();
                    var endingText = titleCard.transform.Find("EndingText")?.GetComponent<TMPro.TextMeshProUGUI>();

                    // Once ending routine has started and faded in (timer > 2.0s)
                    if (_timer >= 2.0f)
                    {
                        _res.cgAlpha = cg != null ? cg.alpha : -1f;
                        _res.blackOverlayAlpha = overlay != null ? overlay.color.a : -1f;
                        _res.endingTextAlpha = endingText != null ? endingText.color.a : -1f;
                        _res.endingText = endingText != null ? endingText.text : "";

                        _res.endingScreenActive = (_res.cgAlpha >= 0.99f && _res.blackOverlayAlpha >= 0.5f);
                        Debug.Log(string.Format("[Test] Ending screen state: cgAlpha={0}, blackAlpha={1}, textAlpha={2}",
                            _res.cgAlpha, _res.blackOverlayAlpha, _res.endingTextAlpha));

                        _step = 4;
                        _frameCounter = 0;
                        _timer = 0f;

                        // Simulate left-click return to menu
                        Debug.Log("[Test] Simulating ReturnToMainMenu()");
                        titleCard.ReturnToMainMenu();
                        _res.calledReturnToMenu = true;
                    }
                }
                return;
            }

            // Step 4: Wait for transition to LoadingScene or Menu
            if (_step == 4)
            {
                string sceneName = SceneManager.GetActiveScene().name;
                _res.sceneAfterReturn = sceneName;

                if (sceneName == "LoadingScene" || sceneName == "Menu")
                {
                    _res.returnedToMenu = true;
                    _res.success = _res.pickedUpCrystal && _res.isDungeonReturn && _res.endingScreenActive && _res.returnedToMenu;
                    Debug.Log("[Test] SUCCESS! Transitioned to: " + sceneName);

                    FinishTest();
                }
                else if (_frameCounter > 200)
                {
                    _res.success = false;
                    _res.error = "Timeout waiting for LoadingScene/Menu, current: " + sceneName;
                    Debug.LogError("[Test] " + _res.error);

                    FinishTest();
                }
            }
        }

        private static void FinishTest()
        {
            Application.logMessageReceived -= HandleLog;
            EditorApplication.update -= RunTestUpdate;
            _res.allLogs = _allLogs.ToArray();
            SessionState.SetString(ResultKey, JsonUtility.ToJson(_res));
            SessionState.SetString(StateKey, "Done");
            EditorApplication.isPlaying = false;
        }

        private static void SelfDestruct()
        {
            string scriptPath = SessionState.GetString(ScriptPathKey, "");
            if (!string.IsNullOrEmpty(scriptPath) && AssetDatabase.AssetPathExists(scriptPath))
            {
                AssetDatabase.DeleteAsset(scriptPath);
            }
            SessionState.EraseString(StateKey);
            SessionState.EraseString(ScriptPathKey);
        }

        [System.Serializable]
        public class TestResult
        {
            public bool success;
            public string error = "";
            public bool pickedUpCrystal;
            public bool isDungeonReturn;
            public bool dialogueStarted;
            public bool dialogueCompleted;
            public float cgAlpha;
            public float blackOverlayAlpha;
            public float endingTextAlpha;
            public string endingText = "";
            public bool endingScreenActive;
            public bool calledReturnToMenu;
            public string sceneAfterReturn = "";
            public bool returnedToMenu;
            public string[] allLogs;
        }
    }
}
