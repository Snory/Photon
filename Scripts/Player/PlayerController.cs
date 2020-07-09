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
    public int PlayerId;

    [Header("Component")]
    [SerializeField]
    public Player PhotonPlayer;
    private UnitBase _selectedPlayerUnit;
    private UnitBase _selectedEnemyUnit;
    private UnitBase _pointedUnit;
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
            DisplayActionArea();

            if (Input.GetMouseButtonDown(0))
            {
                TrySelectPlayerUnit(mousePositionInWolrd);
            }

            if (Input.GetMouseButtonDown(1))
            {
                switch (_playerAction)
                {
                    case PlayerActionType.MOVE:
                        TryMoveUnit(mousePositionInWolrd);
                        break;
                    case PlayerActionType.ATTACK:
                        TryAttack();
                        break;
                }
            }
        }
    }

    private void UpdatePlayerAvailableAction(Vector3 position)
    {


        if (_selectedPlayerUnit != null)
        {
            HexTile tile = PathFinder.Instance.WalkableTileMap.GetHexTile(position);

            if (tile != null && !tile.Walkable)
            {
                UnitBase hooverOverUnit = GameManager.Instance.Units.Where(h => h.Movement.CurrentHexTile == tile).Where(h => !h.IsMine).FirstOrDefault();

                if (hooverOverUnit != null)
                {
                    _playerAction = PlayerActionType.ATTACK;
                    _selectedEnemyUnit = hooverOverUnit;
                }


            }
            else
            {
                _playerAction = PlayerActionType.MOVE;
                _selectedPlayerUnit.Attack.HideAttackRange();
            }

        }
        else
        {
            _playerAction = PlayerActionType.NONE;
            _selectedEnemyUnit = null;
            if(_selectedPlayerUnit != null)
            {
                _selectedPlayerUnit.Attack.HideAttackRange();
            }
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



    private void DisplayActionArea()
    {
        switch (_playerAction)
        {
            case PlayerActionType.ATTACK:
                _selectedPlayerUnit.Attack.DisplayAttackRange(_selectedEnemyUnit.Movement.CurrentHexTile, _selectedPlayerUnit.Movement.MaxDistance);
                break;
        }
    }

    private void TrySelectPlayerUnit(Vector3 position)
    {
        HexTile tile = PathFinder.Instance.WalkableTileMap.GetHexTile(position);
        UnitBase hooverOverUnit = GameManager.Instance.Units.Where(h => h.Movement.CurrentHexTile == tile).FirstOrDefault();


        if (hooverOverUnit != null)
        {
            UnselectedUnit();
            _selectedPlayerUnit = hooverOverUnit;
            _selectedPlayerUnit.Selected = true;
        }
        else
        {
            UnselectedUnit();
        }
    }

    private void TryMoveUnit(Vector3 position)
    {
        _selectedPlayerUnit.Movement.MoveTo(position);
        UnselectedUnit();
    }

    private void TryAttack()
    {

        if(_pointedUnit != null && !_pointedUnit.IsMine) {
       
            bool inEnemyInRange = _pointedUnit.Movement.CurrentHexTile.GetDistanceToCoordination(_selectedPlayerUnit.Movement.CurrentHexTile.GridCoordination) <= _selectedPlayerUnit.Movement.MaxDistance;
            if (inEnemyInRange)
            {
                Debug.Log("Attack");
                

            }
        }       
        //try to move to the point 
        //set attack to true
    }



    public void OnPathDestinationReached()
    {
        //if there should be attack, attack
    }

    private void UnselectedUnit()
    {
        if (_selectedPlayerUnit != null)
        {
            _selectedPlayerUnit.Selected = false;
            _selectedPlayerUnit.Attack.HideAttackRange();
            _selectedPlayerUnit = null;
        }
    }







    //private void Rotate()
    //{        
    //    Vector3 playerToMouseDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(this.transform.position);
    //    var angle = Mathf.Atan2(playerToMouseDirection.y, playerToMouseDirection.x) * Mathf.Rad2Deg;
    //     this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


    //}
}
