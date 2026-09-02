using System.Collections.Generic;
using UnityEngine;

// 디바이스가 모바일일때만 활성화될 UI들을 활성화
public class MobileUI : MonoBehaviour
{
    // 디바이스가 모바일일때만 활성화될 UI들
    public List<RectTransform> mobileUIs = new List<RectTransform>();

    private void Awake()
    {
        // 현재 디바이스가 모바일이면
        if (CurrentDevice.IsMobile)
        {
            // mobileUIs에 들어있던 모든 UI들을 활성화
            foreach (var ui in mobileUIs)
            {
                ui.gameObject.SetActive(true);
            }
        }
    }
}