using Photon.Pun; // 유니티용 포톤 컴포넌트들
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Enums;

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class LobbyManager : MonoBehaviourPunCallbacks {
    public TMP_Text connectionInfoText; // 네트워크 정보를 표시할 텍스트
    public Button joinSphereButton; // 동그란 캐릭터 선택 버튼
    public Button joinBoxButton; // 네모난 캐릭터 선택 버튼
    
    private string _gameVersion = "1"; // 게임 버전
    private CharacterType _choiceCharacter;

    // 게임 실행과 동시에 마스터 서버 접속 시도
    private void Start() {
        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = _gameVersion;
        // 설정한 정보로 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();

        // 룸 접속 버튼 비활성화
        joinSphereButton.interactable = false;
        joinBoxButton.interactable = false;
        // 접속 시도 중임을 텍스트로 표시
        connectionInfoText.text = "마스터 서버에 접속중...";
        
        // 방장이 씬을 로드하면 나머지도 따라가 로드하는 것을 허용
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster() {
        // 룸 접속 버튼 활성화
        joinSphereButton.interactable = true;
        joinBoxButton.interactable = true;
        
        // 접속 정보 표시
        connectionInfoText.text = "온라인 : 마스터 서버와 연결됨";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause) {
        // 룸 접속 버튼 비활성화
        joinSphereButton.interactable = false;
        joinBoxButton.interactable = false;
        // 접속 정보 표시
        connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";

        // 마스터 서버로의 재접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 캐릭터 선택 버튼을 누르면 룸 접속 시도(Connect 메서드) 전에 정보를 미리 저장
    // Button 컴포넌트와 직접 연결
    // choiceCharacter가 1이면 동그란 캐릭터, 2면 네모난 캐릭터
    public void OnClickMatch(int choiceCharacter)
    {
        // 선택한 캐릭터를 저장
        _choiceCharacter = (CharacterType)choiceCharacter;
        // 어느 캐릭터를 골랐는지가 저장된 해시테이블
        Hashtable props = new Hashtable()
        {
            {"ChoiceCharacter", (int)_choiceCharacter}
        };
        // 현재 로컬 플레이어의 정보에 저장
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        
        Connect();
    }

    // 룸 접속 시도
    public void Connect() {
        // 중복 접속 시도를 막기 위해 접속 버튼 잠시 비활성화
        joinSphereButton.interactable = false;
        joinBoxButton.interactable = false;

        // 마스터 서버에 접속 중이라면
        if (PhotonNetwork.IsConnected)
        {
            // 접속 정보 표시
            connectionInfoText.text = "룸에 접속...";
            
            // 선택한 캐릭터가 동그란 캐릭터면 1을, 네모난 캐릭터면 0을 가짐
            CharacterType targetCharacter = (_choiceCharacter == CharacterType.SphereGolem) ? CharacterType.BoxGolem : CharacterType.SphereGolem;

            // 방장이 어느 캐릭터인지 구분할 때 사용할 해시테이블
            Hashtable targetProps = new Hashtable() { { "HostChoiceCharacter", (int)targetCharacter } };
            
            // targetProps와 같은 정보의 랜덤 룸 접속
            PhotonNetwork.JoinRandomRoom(targetProps, 0);
        }
        else
        {
            // 마스터 서버에 접속 중이 아니라면 마스터 서버에 접속 시도
            connectionInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음\n접속 재시도 중...";
            // 마스터 서버로의 재접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message) {
        // 접속 상태 표시
        connectionInfoText.text = "빈 방이 없음, 새로운 방 생성...";
        
        // 룸에 설정할 옵션들을 가짐
        RoomOptions roomOptions = new RoomOptions();
        // 룸 최대 인원은 2명
        roomOptions.MaxPlayers = 2;
        // 룸의 정보로 쓰일 해시테이블
        roomOptions.CustomRoomProperties = new Hashtable() { { "HostChoiceCharacter", (int)_choiceCharacter } };
        // 외부에서 보일 룸의 정보
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "HostChoiceCharacter" };
        
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom() {
        // 접속 상태 표시
        connectionInfoText.text = "방 참가 성공";
    }

    // 다른 플레이어가 방에 들어옴
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        TryStartGame();
    }
    
    private void TryStartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // 접속 상태 표시
            connectionInfoText.text = "게임 시작 중...";

            // 현재 클라이언트가 방장이라면
            if (PhotonNetwork.IsMasterClient)
            {
                // Main 씬으로 이동
                PhotonNetwork.LoadLevel("Main");
            }
        }
    }
}