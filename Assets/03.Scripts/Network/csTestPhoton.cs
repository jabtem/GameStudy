using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csTestPhoton : MonoBehaviour
{

    //App의 버전 정보 (번들 버전과 일치 시키자...)
    public string version = "Ver 0.1.0";

    //개발하는 동안 PUN 으로 개발하는 것이 처음이면 최대한 Unity 콘솔에 로그를 많이 찍어 어떤 
    //사항이 발생하는지 파악하는 것을 권장. 
    //예상하는 대로 동작하는 것에 대하여 확신이 서면 로그 레벨을 Informational 으로 변경 하자.
    public PhotonLogLevel LogLevel = PhotonLogLevel.Full;

    //플레어의 생성 위치 저장 레퍼런스
    public Transform playerPos;

    //App 인증 및 로비연결
    void Awake()
    {
        
        if (!PhotonNetwork.connected)
        {
            
            PhotonNetwork.ConnectUsingSettings(version);

            PhotonNetwork.logLevel = LogLevel;

            PhotonNetwork.playerName = "GUEST " + Random.Range(1, 9999);

        }


    }
    void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby !!!");


        // 로비 입장후 이미 생성된 룸(방) 중에서 무작위로 선택해 입장하는 (Random Match Making) 함수
        PhotonNetwork.JoinRandomRoom(); // (UI 버전에서는 주석 처리)


      

    }


    //포톤 클라우드는 Random Match Making 기능 제공
    //무작위 룸 접속(입장)에 실패한 경우 호출되는 콜백 함수
    void OnPhotonRandomJoinFailed()
    {
        //랜텀 매치 메이킹이 실패한 후 Console 뷰에 나타나는 메시지 설정
        Debug.Log("No Rooms !!!");

        bool isSucces = PhotonNetwork.CreateRoom("MyRoom");
       
        Debug.Log("[정보] 게임 방 생성 완료 : " + isSucces);
    }

    //룸(방) 만들기에 실패한 경우 호출되는 콜백 함수 
    /* PhotonNetwork.CreateRoom 함수로 룸 생성시 실패한 경우는 거의 대부분 룸의 이름의 중복에 있다.
     * 이런 경우 다음과 같이 OnPhotonCreateRoomFailed 콜백 함수가 호출되며, 이 함수에서 예외 처리 로직을 수행하게 하자.
     * 또한 다음과 같이 세부적인 오류 메시지를 받아올 수 있다. 사용자에게 전송 해주어 오류 정보를 알리자.
     * 
    */
    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        //오류 코드( ErrorCode Class )
        Debug.Log(codeAndMsg[0].ToString());
        //오류 메시지
        Debug.Log(codeAndMsg[1].ToString());
        // 여기선 디버그로 표현했지만 릴리즈 버전에선 사용자에게 메시지 전달
        Debug.Log("Create Room Failed = " + codeAndMsg[1]);
    }

    // 룸에 입장하면 호출되는 콜백 함수 
    // PhotonNetwork.CreateRoom 함수로 룸을 생성한 후 입장하거나, PhotonNetwork.JoinRandomRoom, PhotonNetwork.JoinRoom 함수를 통해 입장해도 호출 된다.
    void OnJoinedRoom()
    {
        Debug.Log("Enter Room");
        //여기까지 게임을 실행하면 로비 입장, 랜덤 매치 메이킹, 룸 생성, 룸 입장의 과정을 거치며 Console 뷰에 
        //Joined Lobby !!!, No Rooms !!!, Enter Room 메시지가 출력~ 즉 순서대로 룸 입장까지 완료된 로그 메시지를 확인하자~!

        //플레이어를 생성하는 함수 호출
        CreatePlayer();
    }

    //플레이어를 생성하는 함수
    void CreatePlayer()
    {
        //float pos = Random.Range(-100.0f, 100.0f);
        //포톤네트워크를 이용한 동적 네트워크 객체는 다음과 같이 Resources 폴더 안에 애셋의 이름을 인자로 전달 해야한다. 
        //PhotonNetwork.Instantiate( "MainPlayer", new Vector3(pos, 20.0f, pos), Quaternion.identity, 0 );
        PhotonNetwork.Instantiate("MainPlayer", playerPos.position, playerPos.rotation, 0);


        //PhotonNetwork.InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, byte group, object[] data);
        //이 함수도 PhotonNetwork.Instantiate와 마찬가지로 네트워크 상에 프리팹을 동시에 생성시키지만, Master Client 만 생성 및 삭제 가능.
        //생성된 프리팹 오브젝트의 PhotonView 컴포넌트의 Owner는 Scene이 된다.



    }

    //포톤 클라우드 서버로 접속하는 과정에 대한 로그 메시지 출력을 위한 콜백함수 
    //마지막 JoinedLobby 로그 메시지가 표시되면 정상적으로 포톤 클라우드에 접속하여 로비에 입장한 상태임.
    void OnGUI()
    {

        //화면 좌측 상단에 접속 과정에 대한 로그를 출력(포톤 클라우드 접속 상태 메시지 출력)
        // PhotonNetwork.ConnectUsingSettings 함수 호출시 속성 PhotonNetwork.connectionStateDetailed는
        //포톤 클라우드 서버에 접속하는 단계별 메시지를 반환함.
        //Joined Lobby 메시지시 포톤 클라우드 서버로 접속해 로비에 안전하게 입장했다는 뜻
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        //만약 포톤네트워크에 연결 되었다면...
        if (PhotonNetwork.connected)
        {
            GUI.Label(new Rect(0, 50, 200, 100), "Connected");

            //룸 리스트를 배열로 받아온다.
            RoomInfo[] roomList = PhotonNetwork.GetRoomList();

            if (roomList.Length > 0)
            {
                foreach (RoomInfo info in roomList)
                {
                    GUI.Label(new Rect(0, 80, 400, 100), "Room: " + info.Name
                        + " PlayerCount/MaxPlayer :" + info.PlayerCount + "/" + info.MaxPlayers //현재 플레이어/최대 플레이어
                        + " CustomProperties Count " + info.CustomProperties.Count // 설정한 CustomProperties 수 
                        + " Map ???: " + info.CustomProperties.ContainsKey("Map") //키로 설정한 Map이 있나
                        + " Map Count " + info.CustomProperties["Map"] // 설정한 키 값 
                        + " GameType ??? " + info.CustomProperties.ContainsKey("GameType") //키로 설정한 GameType이 있나
                        + " GameType " + info.CustomProperties["GameType"]);// 설정한 키 값 
                }
            }
            else
            {
                GUI.Label(new Rect(0, 80, 400, 100), "No Room List");
            }
        }
        //PhotonServerSettings 값 가져오기
        {
            GUI.Label(new Rect(0, 170, 400, 100), "AppID  :  " +
                PhotonNetwork.PhotonServerSettings.AppID);
            GUI.Label(new Rect(0, 200, 200, 100), "HostType  :  " +
                PhotonNetwork.PhotonServerSettings.HostType);
            GUI.Label(new Rect(0, 230, 200, 100), "ServerAddress  :  " +
                PhotonNetwork.PhotonServerSettings.ServerAddress);
            GUI.Label(new Rect(0, 260, 200, 100), "ServerPort  :  " +
                PhotonNetwork.PhotonServerSettings.ServerPort);
            //PhotonNetwork.PhotonServerSettings.UseCloud(); 

            //핑 테스트
            int pingTime = PhotonNetwork.GetPing();
            GUI.Label(new Rect(0, 310, 200, 100), "Ping: " + pingTime.ToString());
        }
    }
}
// 참고 https://doc-api.photonengine.com/ko-kr/pun/current/class_room_options.html


