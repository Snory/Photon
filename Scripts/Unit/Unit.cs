using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviourPun
{
    private Coroutine _movingRoutine;
    public bool IsMine { get; set; }

    [Header("Info")]
    public float MoveSpeed;
    public float test;
    private Rigidbody2D _body;

    public HexTile CurrentHexTile;

    private void Awake()
    {
        _body = this.GetComponent<Rigidbody2D>();

    }



    [PunRPC]
    public void Initialize(bool isMine)
    {
        IsMine = isMine;

        if (!isMine)
            _body.isKinematic = true;

        GameManager.Instance.Units.Add(this);


    }

    private void Start()
    {
        CurrentHexTile = PathFinder.Instance.WalkableTileMap.GetHexTileOnWorldPosition(this.transform.position);
        this.transform.position = CurrentHexTile.WorldCoordination;
    }

    private void Move()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (PathFinder.Instance == null)
            {
                Debug.LogError("[PlayerController]: PathFinder is not available");
            }

            if (PathFinder.Instance.WalkableTileMap == null)
            {
                Debug.LogError("[PlayerController]: WalkableTileMap is not available");
            }

            HexTile destination = PathFinder.Instance.WalkableTileMap.GetHexTileOnWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            PathRequestManager.Instance.RequestPath(this.transform.position, destination.WorldCoordination, OnPathRequestDone);

        }

    }

    public void OnPathRequestDone(HexTile[] path, bool pathFound)
    {
        if (pathFound)
        {
            if (_movingRoutine != null)
            {
                StopCoroutine(_movingRoutine);
            }
            _movingRoutine = StartCoroutine(FollowPath(path));
        }
    }

    private IEnumerator FollowPath(HexTile[] path)
    {
        int currentWayPointIndex = 0;
        Vector3 wayPointCoordination = path[currentWayPointIndex].WorldCoordination;
        while (true)
        {
            if (this.transform.position == wayPointCoordination)
            {
                currentWayPointIndex++;
                if (path.Length > currentWayPointIndex)
                {
                    wayPointCoordination = path[currentWayPointIndex].WorldCoordination;
                }
                else
                {
                    CurrentHexTile = path[currentWayPointIndex];
                    yield break;
                }

            }

            this.transform.position = Vector3.MoveTowards(this.transform.position, wayPointCoordination, MoveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}