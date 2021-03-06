﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;



public class UnitMovement : MonoBehaviourPun
{
    private Coroutine _movingRoutine;
    public HexTile CurrentHexTile;
    public GameObject MovementVisualization;
    private List<GameObject> _movementVisualizationObjects;


    [Header("Info")]
    public float MoveSpeed;
    public int MaxDistance;
    private bool _moving;


    private void Start()
    {
        SetCurrentHexTile(this.transform.position);

        this.transform.position = CurrentHexTile.WorldCoordination;
        _movementVisualizationObjects = new List<GameObject>();
    }

    public void DisplayMovementArea(bool display)
    {
        if (display && !_moving)
        {
            List<Vector3Int> neighbors = CurrentHexTile.GetNeighborCoordinations(MaxDistance);
            foreach (Vector3Int neighbor in neighbors)
            {
                //instantiate tiles object to be removed later
                GameObject movementVisualization = Instantiate(MovementVisualization, PathFinder.Instance.WalkableTileMap.GetHexTile(neighbor).WorldCoordination, Quaternion.identity);
                movementVisualization.transform.parent = this.transform;
                _movementVisualizationObjects.Add(movementVisualization);
            }
        }
        else
        {
            foreach (GameObject obj in _movementVisualizationObjects)
            {
                Destroy(obj);
            }

            _movementVisualizationObjects.Clear();
        }
    }

    public void MoveTo(Vector3 position)
    {

        if (PathFinder.Instance == null)
        {
            Debug.LogError("[PlayerController]: PathFinder is not available");
        }
        if (PathFinder.Instance.WalkableTileMap == null)
        {
            Debug.LogError("[PlayerController]: WalkableTileMap is not available");
        }

        HexTile destination = PathFinder.Instance.WalkableTileMap.GetHexTile(position);


        if (destination != null)
        {
            if (CurrentHexTile.GetDistanceToCoordination(destination.GridCoordination) <= MaxDistance)
            {
                PathRequestManager.Instance.RequestPath(this.transform.position, destination.WorldCoordination, OnPathRequestProcessed);
            }
        }
    }

    public void OnPathRequestProcessed(HexTile[] path, bool pathFound)
    {
        if (pathFound && !_moving)
        {           
            _movingRoutine = StartCoroutine(FollowPath(path));
            _moving = true;
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
                    HexTile wayPointTile = path[currentWayPointIndex];
                    if (wayPointTile.Walkable)
                    {
                        wayPointCoordination = wayPointTile.WorldCoordination;
                        SetCurrentHexTile(wayPointCoordination);
                    }
                    else
                    {
                        MoveTo(path[path.Length - 1].WorldCoordination);
                        yield break;
                    }
                }
                else
                {
                    _moving = false;
                    SetCurrentHexTile(wayPointCoordination);
                    yield break;
                }
            }

            Vector3 targetDirection = wayPointCoordination - this.transform.position;
            var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            this.transform.position = Vector3.MoveTowards(this.transform.position, wayPointCoordination, MoveSpeed * Time.smoothDeltaTime);
            yield return null;
        }
    }


    private void SetCurrentHexTile(Vector3 worldCoordination)
    {   
        Debug.Log($"Setting tile on position {worldCoordination.ToString()}");
        HexTile tile = PathFinder.Instance.WalkableTileMap.GetHexTile(worldCoordination);

        if (CurrentHexTile != null)
        {
            HexTile oldTile = CurrentHexTile;
            PathFinder.Instance.WalkableTileMap.photonView.RPC("SetIsHexTileWalkable", RpcTarget.All, oldTile.WorldCoordination, true);
        }
        CurrentHexTile = tile;
        PathFinder.Instance.WalkableTileMap.photonView.RPC("SetIsHexTileWalkable", RpcTarget.All, CurrentHexTile.WorldCoordination, false);
     
    }


}