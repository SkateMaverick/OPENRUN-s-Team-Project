using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void LoadLobby()
    {
        Debug.Log("LoadLobby 호출됨");
        SceneManager.LoadScene("Lobby");
    }

    public void LoadMain()
    {
        SceneManager.LoadScene("Main");
    }

    public void LoadDungeon()
    {
        SceneManager.LoadScene("Dungeon");
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료 실행");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}