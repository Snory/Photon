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
    private Camera _mainCamera;
    private bool _attack;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }




    private void Update()
    {


        if (photonView.IsMine)
        {

            Vector3 mousePositionInWolrd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                TrySelectUnit(mousePositionInWolrd);
            }


            if (Input.GetMouseButtonDown(1))
            {
                if(_selectedEnemyUnit != null)
                {
                    TryAttack(mousePositionInWolrd);
                } else
                {
                    TryMoveUnit(mousePositionInWolrd);
                }
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



    private void TrySelectUnit(Vector3 position)
    {
        HexTile tile = PathFinder.Instance.WalkableTileMap.GetHexTile(position);
        UnitBase hooverOverUnit = GameManager.Instance.Units.Where(h => h.Movement.CurrentHexTile == tile).FirstOrDefault();

        if (hooverOverUnit != null)
        {
            if (hooverOverUnit.IsMine)
            {
                UnselectedPlayerUnit();
                _selectedPlayerUnit = hooverOverUnit;
                _selectedPlayerUnit.Selected = true;
                _selectedPlayerUnit.Movement.PathFinished += OnPathDestinationReached;
            } else
            {
                UnselectEnemyUnit();
                Debug.Log("Selected enemy unit");
                _selectedEnemyUnit = hooverOverUnit;
                if(_selectedPlayerUnit != null)
                {
                    _selectedPlayerUnit.Attack.DisplayAttackRange(_selectedEnemyUnit.Movement.CurrentHexTile, _selectedPlayerUnit.Movement, _selectedPlayerUnit.Attack.AttackRange); ;
                }
            }

        }
        else
        {
            UnselectedPlayerUnit();
            UnselectEnemyUnit();
        }
    }


    private void TryMoveUnit(Vector3 position)
    {
        if(_selectedPlayerUnit != null) { 
            _selectedPlayerUnit.Movement.MoveTo(position);
            UnselectedPlayerUnit();
        }
    }

    private void TryAttack(Vector3 position)
    {
        Debug.Log("Trying to attack");
        TryMoveUnit(position);
        _attack = true;

    }



    public void OnPathDestinationReached()
    {
        if (_attack)
        {
            Debug.Log("Attacking!");
        }
        _attack = false;
    }

    private void UnselectedPlayerUnit()
    {
        if (_selectedPlayerUnit != null)
        {
            _selectedPlayerUnit.Movement.PathFinished -= OnPathDestinationReached;
            _selectedPlayerUnit.Selected = false;
            _selectedPlayerUnit.Attack.HideAttackRange();
            _selectedPlayerUnit = null;
        }
    }

    private void UnselectEnemyUnit()
    {
        if (_selectedEnemyUnit != null)
        {
              _selectedEnemyUnit.Selected = false;
            _selectedEnemyUnit.Attack.HideAttackRange();
            _selectedEnemyUnit = null;
        }
    }





    //private void Rotate()
    //{        
    //    Vector3 playerToMouseDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(this.transform.position);
    //    var angle = Mathf.Atan2(playerToMouseDirection.y, playerToMouseDirection.x) * Mathf.Rad2Deg;
    //     this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


    //}
}
