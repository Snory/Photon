using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    private static NetworkManager _instance;
    public static NetworkManager Instance { get { return _instance; } }

    public TextMeshProUGUI LogText;


    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Debug.LogError("[NetworkManager]: trying to initialize another instance");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.DeleteAll();
        Log("Connecting to Photon network");
        ConnectToPhoton();
        
    }


    public override void OnConnectedToMaster()
    {
        Log("Connected to master");
    }

    public override void OnJoinedRoom()
    {
        Log("Joined room");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Disconnected due to {cause.ToString()}");
    }

    public void ConnectToPhoton()
    {
        Log("Connecting....");
        PhotonNetwork.ConnectUsingSettings();
    }


    public void CreateOrJoinRoom()
    {
        if(PhotonNetwork.CountOfRooms > 0)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(null, options);
            
        }
    }

    [PunRPC]
    public void LoadLevel (string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    public void Log(string text)
    {
        LogText.text = text;
        Debug.Log(text);
    }
}
