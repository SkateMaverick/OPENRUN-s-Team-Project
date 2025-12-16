using System;
using Photon.Pun;
using UnityEngine;
using Enums;
using Player.InputActions;
using Player.Script.CameraScript;

public class NetworkManager : MonoBehaviour
{
    // Resources 파일에 담겨있는 동그란 캐릭터 프리팹 (포톤으로 프리팹을 인스턴스할 때는 Resources 파일에 담긴 프리팹만 가능합니다.)
    [SerializeField] private GameObject sphereGolem;
    // Resources 파일에 담겨있는 동그란 캐릭터 프리팹
    [SerializeField] private GameObject boxGolem;
    // 원형 캐릭터 기본 스폰 위치
    [SerializeField] private Transform sphereDefaultPosition;
    // 사각형 캐릭터 기본 스폰 위치
    [SerializeField] private Transform boxDefaultPosition;

    // 강제로 연결
    public NetworkMainCamera cam;

    private void Awake()
    {
        SpawnCharacter();
    }

    private void Start()
    {
        
    }

    private void SpawnCharacter()
    {
        // 로컬 플레이어의 정보에서 ChoiceCharacter를 가져옴 
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("ChoiceCharacter", out object characterType))
        {
            // 조종하던 캐릭터가 담김
            GameObject myCharacter;
            // 캐릭터가 스폰될 위치
            Vector3 spawnPosition;

            //  out object characterType에 담겨있던 것에 따라 myCharacter에 0이면 SphereGolem, 1이면 BoxGolem을 담음
            if ((CharacterType)characterType == CharacterType.SphereGolem)
            {
                myCharacter = sphereGolem;
            }
            else
            {
                myCharacter = boxGolem;
            }
            
            // 로컬 플레이어 정보에서 저장된 위치가 있다면 spawnPosition에 그 값을, 없다면 기본 위치
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SavedPosition", out object savedPosition))
            {
                spawnPosition = (Vector3)savedPosition;
            }
            else
            {
                if ((CharacterType)characterType == CharacterType.SphereGolem)
                {
                    spawnPosition = sphereDefaultPosition.position;
                }
                else
                {
                    spawnPosition = boxDefaultPosition.position;
                }
            }
            
            // test 변수는 필요없음 강제로 연결용
            GameObject test = PhotonNetwork.Instantiate(myCharacter.name, spawnPosition, Quaternion.identity);
            
            // 강제로 연결
            cam.defaultTarget = test.transform;
        }
    }
}
