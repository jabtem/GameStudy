using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csTestPhoton : MonoBehaviour
{
    public string version = "Ver 0.1.0";

    public PhotonLogLevel LogLevel = PhotonLogLevel.ErrorsOnly;

    public Transform playerPos;

    public int myId;

    public List<GameObject> players;

    void Awake()
    {
        //players = new List<GameObject>();
       if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings(version);

            PhotonNetwork.logLevel = LogLevel;

            PhotonNetwork.playerName = "GUEST" + Random.Range(1, 9999);
            
        }
    }

    void Update()
    {

        foreach (GameObject p in players)
        {
            if(p.GetComponent<PhotonView>().viewID != myId)
            {
                p.tag = "Team";
            }
        }

    }
    //방만들기 실패시 호출되는 콜백
    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        Debug.Log(codeAndMsg[0].ToString());
        Debug.Log(codeAndMsg[1].ToString());
        Debug.Log("Create Room Failed = " + codeAndMsg[1]);
    }

    void OnJoinedRoom()
    {
        Debug.Log("Enter Room");

        CreatePlayer();
    }

    void CreatePlayer()
    {
        GameObject go = PhotonNetwork.Instantiate("MainPlayer", playerPos.position, playerPos.rotation, 0);
        myId = go.GetComponent<PhotonView>().viewID;
        players.Add(go);
    }

    void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby !!!");

        PhotonNetwork.JoinRandomRoom(); // (UI 버전에서는 주석 처리)

    }


    //포톤 클라우드는 Random Match Making 기능 제공
    //무작위 룸 접속(입장)에 실패한 경우 호출되는 콜백 함수
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("No Rooms !!!");

        bool isSucces = PhotonNetwork.CreateRoom("MyRoom");

        Debug.Log("[정보] 게임 방 생성 완료 : " + isSucces);
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());




        if(PhotonNetwork.connected)
        {
            GUI.Label(new Rect(0, 50, 200, 100), "Connected");
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
                GUI.Label(new Rect(0, 80, 400, 100), "NO Room List");
                GUI.Label(new Rect(0, 100, 400, 100), PhotonNetwork.playerName);
                GUI.Label(new Rect(0, 120, 400, 100), myId.ToString());
            }


        }
        GUI.Label(new Rect(0, 170, 400, 100), "AppID  :  " + PhotonNetwork.PhotonServerSettings.AppID);
        GUI.Label(new Rect(0, 200, 200, 100), "HostType  :  " + PhotonNetwork.PhotonServerSettings.HostType);
        GUI.Label(new Rect(0, 230, 200, 100), "ServerAddress  :  " + PhotonNetwork.PhotonServerSettings.ServerAddress);
        GUI.Label(new Rect(0, 260, 200, 100), "ServerPort  :  " + PhotonNetwork.PhotonServerSettings.ServerPort);
        //PhotonNetwork.PhotonServerSettings.UseCloud(); 

        //핑 테스트
        int pingTime = PhotonNetwork.GetPing();
        GUI.Label(new Rect(0, 310, 200, 100), "Ping: " + pingTime.ToString());

    }

}
