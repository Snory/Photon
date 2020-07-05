﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]

    public bool GameEnded;
    public float TimeToWin;
    public float InvicibleDuration;
    private float _hatPickUpTime;

    [Header("Players")]
    public string PlayerPrefabLocation;
    public Transform[] SpawnPoints;
    public PlayerController[] Players;
    public List<Unit> Units;
    private int _playersInGame;

    public static GameManager _instance;
    public static GameManager Instance { get => _instance; set => _instance = value; }


    private void Awake()
    {
        GameEnded = false;
        _playersInGame = 0;
        if(_instance == null)
        {
            Instance = this;
        }
    }


    private void Start()
    {
        Players = new PlayerController[PhotonNetwork.PlayerList.Length];
        Units = new List<Unit>();
        photonView.RPC("IAmInGame", RpcTarget.AllBuffered);    
    }

    [PunRPC]
    private void IAmInGame()
    {
        _playersInGame++;

        if(_playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }


    //jednou zavolám já sám, jednou se to spustí od dalšího hráče
    private void SpawnPlayer()
    {
 
        GameObject playerObj = PhotonNetwork.Instantiate(PlayerPrefabLocation, SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, Quaternion.identity);
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        //dám všem ostatním vědět, že jsem ve hře (včetně sebe)
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

    }

    public PlayerController GetPlayerController (int playerId)
    {
        return Players.First(x => x.PlayerId == playerId);
    }

    public PlayerController GetPlayerController(GameObject playerObj)
    {
        return Players.First(x => x.gameObject == playerObj);
    }
}
