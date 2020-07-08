using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public enum PlayerActionType { NONE, MOVE, ATTACK }
public class PlayerController : MonoBehaviourPunCallbacks
{

    [HideInInspector]
    public int PlayerId;



    [Header("Component")]
    [SerializeField]
    public Player PhotonPlayer;
    private UnitBase _selectedUnit;
    private PlayerActionType _playerAction;
    private Camera _mainCamera;


    private void Awake()
    {
        _mainCamera = Camera.main;
    }


    private void Start()
    {
        _playerAction = PlayerActionType.NONE;
    }


    private void Update()
    {


        if (photonView.IsMine)
        {

            Vector3 mousePositionInWolrd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            UpdatePlayerAvailableAction(mousePositionInWolrd);

            if (Input.GetMouseButtonDown(0))
            {
                TrySelectUnit(mousePositionInWolrd);
            }

            if (Input.GetMouseButtonDown(1))
            {
                switch (_playerAction)
                {
                    case PlayerActionType.MOVE:
                        TryMoveUnit(mousePositionInWolrd);
                        break;
                    case PlayerActionType.ATTACK:
                        TryAttack(mousePositionInWolrd);
                        break;
                }            
                
            }
        }
    }

    private void UpdatePlayerAvailableAction(Vector3 position)
    {


        if (_selectedUnit != null)
        {
            HexTile tile = PathFinder.Instance.WalkableTileMap.GetHexTile(position);

            if (tile != null && !tile.Walkable)
            {
                UnitBase hooverOverUnit = GameManager.Instance.Units.Where(h => h.Movement.CurrentHexTile == tile).FirstOrDefault();

                if(hooverOverUnit != null)
                {
                    Debug.Log("Test");
                }

                //if (!hooverOverUnit.IsMine)
                //{
                //    _playerAction = PlayerActionType.ATTACK;
                //}
            } else
            {
                _playerAction = PlayerActionType.MOVE;
            }

        }
        else
        {
            _playerAction = PlayerActionType.NONE;
        }

    }

    [PunRPC]
    public void Initialize(Player player)
    {
        PhotonPlayer = player;
        PlayerId = player.ActorNumber;
        GameManager.Instance.Players[PlayerId - 1] = this;


        if (player.IsLocal)
        {
            SpawnUnits();
        }

    }


    private void SpawnUnits()
    {

        GameObject unitObj = PhotonNetwork.Instantiate("Unit", this.transform.position, Quaternion.identity);
        UnitBase unitScript = unitObj.GetComponent<UnitBase>();

        unitScript.photonView.RPC("Initialize", RpcTarget.Others, false);
        unitScript.photonView.RPC("Initialize", PhotonPlayer, true);

    }


    private void TrySelectUnit(Vector3 position)
    {
        HexTile tile = PathFinder.Instance.WalkableTileMap.GetHexTile(position);

        UnitBase selectedUnit = GameManager.Instance.Units.Where(h => h.Movement.CurrentHexTile == tile).Where(h => h.IsMine).FirstOrDefault();


        if (selectedUnit != null)
        {
            UnselectedUnit();
            _selectedUnit = selectedUnit;
            _selectedUnit.Selected = true;
        }
        else
        {
            UnselectedUnit();
        }
    }


    private void TryMoveUnit(Vector3 position)
    {
        _selectedUnit.Movement.MoveTo(position);
        UnselectedUnit();
    }

    private void TryAttack(Vector3 position)
    {
        //calculate closest point for attack
        HexTile tile = PathFinder.Instance.WalkableTileMap.GetHexTile(position);
        UnitBase enemyUnit = GameManager.Instance.Units.Where(h => h.Movement.CurrentHexTile == tile).Where(h => !h.IsMine).FirstOrDefault();

        Debug.Log("Attack");
        //try to move to the point 
        //set attack to true
    }

  

    public void OnPathDestinationReached()
    {
        //if there should be attack, attack
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
