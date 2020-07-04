using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{ 

    [HideInInspector]
    public float CurHatTime { get; set; }
    [HideInInspector]
    public int PlayerId { get; set; }

    public float JumpForce;
    public GameObject HatObject;


    [Header("Component")]
    [SerializeField]
    public Player PhotonPlayer;

     


    [PunRPC]
    public void Initialize(Player player)
    {
        PhotonPlayer = player;
        PlayerId = player.ActorNumber;
        GameManager.Instance.Players[PlayerId - 1] = this;

        Debug.Log($"Player {player.NickName} inicialized and is min is {photonView.IsMine}");

        GameObject unitObj = PhotonNetwork.Instantiate("Unit", this.transform.position, Quaternion.identity);
        Unit unitScript = unitObj.GetComponent<Unit>();

        unitScript.IsMine = photonView.IsMine;

        Debug.Log("Unit initialized");

    }


   


    

    //private void Rotate()
    //{        
    //    Vector3 playerToMouseDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(this.transform.position);
    //    var angle = Mathf.Atan2(playerToMouseDirection.y, playerToMouseDirection.x) * Mathf.Rad2Deg;
    //     this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
  
    
    //}
}
