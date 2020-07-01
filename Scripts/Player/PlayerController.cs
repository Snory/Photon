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


    private Coroutine _movingRoutine;


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

        if (Input.GetMouseButtonDown(0)) { 
            if(PathFinder.Instance == null)
            {
                Debug.LogError("[PlayerController]: PathFinder is not available");
            }

            if (PathFinder.Instance.WalkableTileMap == null)
            {
                Debug.LogError("[PlayerController]: WalkableTileMap is not available");
            }

            HexTile destination = PathFinder.Instance.WalkableTileMap.GetHexTileOnWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            //Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            //Vector3 velocity = direction * MoveSpeed;
            //_body.velocity = velocity;

            if(destination != null) {
                if(_movingRoutine != null)
                {
                    StopCoroutine(_movingRoutine);
                }
                _movingRoutine =  StartCoroutine(MoveToPosition(destination.WorldCoordination));
            }
        }
    
    }

    private IEnumerator MoveToPosition(Vector3 destination)
    {
        while (true)
        {
            if(this.transform.position != destination)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * MoveSpeed);
                yield return null;
            } else
            {
                yield break;
            }

        }

        

    }

    private void Rotate()
    {        
        Vector3 playerToMouseDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(this.transform.position);
        var angle = Mathf.Atan2(playerToMouseDirection.y, playerToMouseDirection.x) * Mathf.Rad2Deg;
         this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
  
    
    }
}
