using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{ 

    [HideInInspector]
    public float CurHatTime { get; set; }

    public int PlayerId;

    public float JumpForce;
    public GameObject HatObject;


    [Header("Component")]
    [SerializeField]
    public Player PhotonPlayer;

    private Unit _selectedUnit;


    private void Awake()
    {
        
    }

    private void Update()
    {

        if (photonView.IsMine) { 
            if (Input.GetMouseButtonDown(0))
            {
                TrySelect(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        PhotonPlayer = player;
        PlayerId = player.ActorNumber;
        GameManager.Instance.Players[PlayerId - 1] = this;

        Debug.Log($"Player {player.NickName} inicialized and is min is {photonView.IsMine} and object name is {this.gameObject.name}");


        if (player.IsLocal)
        {
            SpawnUnits();
        }

    }


    private void SpawnUnits()
    {

        Debug.Log("Unit spawned");
        GameObject unitObj = PhotonNetwork.Instantiate("Unit", this.transform.position, Quaternion.identity);
        Unit unitScript = unitObj.GetComponent<Unit>();
        
        //dám všem ostatním vědět, že jsem vytvořil jednotku (včetně sebe)
        unitScript.photonView.RPC("Initialize", RpcTarget.All, unitScript.IsMine);

    }


    private void TrySelect(Vector3 position)
    {
        HexTile tile = PathFinder.Instance.WalkableTileMap.GetHexTileOnWorldPosition(position);

        if(tile == null)
        {
            Debug.Log("No tile selected");
        }

        foreach(var unit in GameManager.Instance.Units)
        {
            Debug.Log($"Spawned units are: {unit.CurrentHexTile.WorldCoordination.ToString()}");
        }

        Unit selectedUnit = GameManager.Instance.Units.Where(h => h.CurrentHexTile == tile).FirstOrDefault();


        if (_selectedUnit != null)
        {
            _selectedUnit.Selected = false;
        }


        if (selectedUnit != null)
        {
            Debug.Log("Unit selected!");
            _selectedUnit = selectedUnit;
            _selectedUnit.Selected = true;
        } else
        {         
            _selectedUnit = null;
            Debug.Log($"Selected tile coordination are {tile.GridCoordination}");
        }
    }






    //private void Rotate()
    //{        
    //    Vector3 playerToMouseDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(this.transform.position);
    //    var angle = Mathf.Atan2(playerToMouseDirection.y, playerToMouseDirection.x) * Mathf.Rad2Deg;
    //     this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


    //}
}
