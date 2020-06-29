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


    [Header("Info")]
    public float MoveSpeed;
    public float JumpForce;
    public GameObject HatObject;


    [Header("Component")]
    [SerializeField]
    private Rigidbody2D _body;
    public Player PhotonPlayer;


    [PunRPC]
    public void Initialize(Player player)
    {
        PhotonPlayer = player;
        PlayerId = player.ActorNumber;
        GameManager.Instance.Players[PlayerId - 1] = this;

        if (!photonView.IsMine)
            _body.isKinematic = true;

    }


    private void Update()
    {
        if (photonView.IsMine) { 
            Rotate();
            Move();
        }


    }


    private void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        Vector3 velocity = direction * MoveSpeed;
        _body.velocity = velocity;
    }

    private void Rotate()
    {        
        Vector3 playerToMouseDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(this.transform.position);
        var angle = Mathf.Atan2(playerToMouseDirection.y, playerToMouseDirection.x) * Mathf.Rad2Deg;
         this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
  
    
    }
}
