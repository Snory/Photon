﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;



public class Unit : MonoBehaviourPun
{
    private Coroutine _movingRoutine;
    public bool IsMine { get; set; }
    [Header("Info")]
    public float MoveSpeed;
    public int MaxDistance;
    private Rigidbody2D _body;
    public HexTile CurrentHexTile;
    private bool _selected;
    public GameObject MovementVisualization;
    private List<GameObject> _movementVisualizationObjects;

    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            DisplayMovementArea(value);
        }
    }


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
        CurrentHexTile = PathFinder.Instance.WalkableTileMap.GetHexTile(this.transform.position);
        this.transform.position = CurrentHexTile.WorldCoordination;
        _movementVisualizationObjects = new List<GameObject>();
    }

    public void DisplayMovementArea(bool display)
    {

        if (display)
        {
            List<Vector3Int> neighbors = CurrentHexTile.GetNeighborCoordinations(MaxDistance);
            foreach (Vector3Int neighbor in neighbors)
            {
                //instantiate tiles object to be removed later
                GameObject movementVisualization = Instantiate(MovementVisualization, PathFinder.Instance.WalkableTileMap.GetHexTile(neighbor).WorldCoordination, Quaternion.identity);
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
        if (CurrentHexTile.GetDistanceToCoordination(destination.GridCoordination) <= MaxDistance)
        {
            PathRequestManager.Instance.RequestPath(this.transform.position, destination.WorldCoordination, OnPathRequestProcessed);
        }


    }

    public void OnPathRequestProcessed(HexTile[] path, bool pathFound)
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
                    CurrentHexTile = path[currentWayPointIndex - 1];
                    yield break;
                }

            }

            this.transform.position = Vector3.MoveTowards(this.transform.position, wayPointCoordination, MoveSpeed * Time.smoothDeltaTime);
            yield return null;
        }
    }
}