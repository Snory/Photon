using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
    public GameObject MainScreen;
    public GameObject LobbyScreen;

    [Header("Main screen")]
    public Button PlayButton;

    [Header("Lobby screen")]
    public TextMeshProUGUI Player1NameText;
    public TextMeshProUGUI Player2NameText;
    public TextMeshProUGUI GameStartingText;

    private void Start()
    {
        PlayButton.interactable = false;
        LobbyScreen.SetActive(false);
        MainScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        SetScreen(MainScreen);
    }

    public void SetScreen(GameObject screen)
    {
        LobbyScreen.SetActive(false);
        MainScreen.SetActive(false);
        screen.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdateLobbyUI();
    }


    public void OnUpdatePlayerNameInput(TMP_InputField nameInput)
    {
        PhotonNetwork.NickName = nameInput.text;
        if(nameInput.text.Length == 0)
        {
            PlayButton.interactable = false;
        }
        else
        {
            PlayButton.interactable = true;
        }
    }

    public void OnPlayButton()
    {
        NetworkManager.Instance.CreateOrJoinRoom();
    }

    public void OnLeaveButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(MainScreen);

    }

    public override void OnJoinedRoom()
    {
        SetScreen(LobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    [PunRPC]
    void UpdateLobbyUI()
    {

        Invoke("TryStartGame", 3.0f);
        
        //if (PhotonNetwork.PlayerList.Length > 0) { 
        //    Player1NameText.text = PhotonNetwork.CurrentRoom.GetPlayer(1).NickName;
        //    Player2NameText.text = PhotonNetwork.PlayerList.Length == 2 ? PhotonNetwork.CurrentRoom.GetPlayer(2).NickName : "...";

        //    if(PhotonNetwork.PlayerList.Length == 1)
        //    {
        //        GameStartingText.text = "Waiting for opponent";
        //    } else if (PhotonNetwork.PlayerList.Length == 2)
        //    {

        //        GameStartingText.text = "Starting game..";

        //        if (PhotonNetwork.IsMasterClient)
        //        {
        //            Invoke("TryStartGame", 3.0f);
        //        }
        //    }
        //}
    }

    void TryStartGame()
    {
        if(PhotonNetwork.PlayerList.Length == 1)
        {
            NetworkManager.Instance.photonView.RPC("LoadLevel", RpcTarget.All, "Game");
        }  else
        {
            UpdateLobbyUI();
        }
    }

}
