using UnityEngine;

public class UIPlatformSwitcher : MonoBehaviour
{
    public GameObject mobileUI;
    public GameObject desktopUI;

    public bool forceMobile = false; // 테스트용

    void Start()
    {
#if UNITY_EDITOR
        if (forceMobile)
        {
            mobileUI.SetActive(true);
            desktopUI.SetActive(false);
            return;
        }
#endif

        if (Application.isMobilePlatform)
        {
            mobileUI.SetActive(true);
            desktopUI.SetActive(false);
        }
        else
        {
            mobileUI.SetActive(false);
            desktopUI.SetActive(true);
        }
    }
}