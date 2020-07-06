using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
                if (_selectedUnit == null) { 
                    TrySelectUnit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                } else
                {
                    TryMoveUnit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                UnselectedUnit();
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
        unitScript.photonView.RPC("Initialize", RpcTarget.Others, false);
        unitScript.photonView.RPC("Initialize", PhotonPlayer, true);

    }


    private void TrySelectUnit(Vector3 position)
    {
        HexTile tile = PathFinder.Instance.WalkableTileMap.GetHexTile(position);

        if(tile == null)
        {
            Debug.Log("No tile selected");
        }

        foreach(var unit in GameManager.Instance.Units)
        {
            Debug.Log($"Spawned units are: {unit.CurrentHexTile.WorldCoordination.ToString()}");
        }

        Unit selectedUnit = GameManager.Instance.Units.Where(h => h.CurrentHexTile == tile).Where(h => h.IsMine).FirstOrDefault();


        if (selectedUnit != null)
        {

            UnselectedUnit();
            Debug.Log("Unit selected!");
            _selectedUnit = selectedUnit;
            _selectedUnit.Selected = true;
        } 
    }


    private void TryMoveUnit(Vector3 position)
    {

        _selectedUnit.MoveTo(position);
        UnselectedUnit();
    }

    private void UnselectedUnit()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit.Selected = false;
            _selectedUnit = null;
        }
    }






    //private void Rotate()
    //{        
    //    Vector3 playerToMouseDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(this.transform.position);
    //    var angle = Mathf.Atan2(playerToMouseDirection.y, playerToMouseDirection.x) * Mathf.Rad2Deg;
    //     this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


    //}
}
