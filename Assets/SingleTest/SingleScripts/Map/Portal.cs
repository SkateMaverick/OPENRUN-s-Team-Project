using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
public class Portal : MonoBehaviour
{
    // 포탈이 닿았을 때 이동할 씬
    [SerializeField] private string targetSceneName;
    
    private void OnTriggerEnter(Collider other)
    {
        // 닿은 대상이 IControllable을 가지고 있다면(플레이어가 조작중인 대상이라면)
        if (other.TryGetComponent<IControllable>(out IControllable controllable))
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneLoader.NextSceneName = targetSceneName;
                
                SceneManager.LoadScene("LoadingScene");
            }
            else
            {
                Debug.LogError("씬 목적지가 적혀있지 않습니다. Portal 스크립트를 가진 게임오브젝트의 인스펙터 창에서 targetSceneName에 씬 이름을 넣어주세요.");
            }
        }
    }
}
