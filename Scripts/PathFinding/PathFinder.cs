using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public HexTileMap WalkableTileMap;

    private static PathFinder _instance;
    public static PathFinder Instance { get => _instance; set => _instance = value; }



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("[PathFinder]: Someone is trying to initialize pathfinder");
        }
    }

    internal void StartFindPath(Vector3 pathStart, Vector3 pathEnd, PathResult pathResultCallback)
    {
        StartCoroutine(FindPath(pathStart, pathEnd, pathResultCallback));
    }


    private HexTile[] RetracePath(HexTile startTile, HexTile endTile)
    {
        List<HexTile> path = new List<HexTile>();
        HexTile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.Parent;
        }
        path.Add(currentTile);
        path.Reverse();

        return path.ToArray();
    }
    // Start is called before the first frame update
    private IEnumerator FindPath(Vector3 startPostion, Vector3 endPosition, PathResult pathResultCallback)
    {

        HexTile[] wayPoints = new HexTile[0];
        bool pathSuccess = false;


        HexTile startTile = WalkableTileMap.GetHexTileOnWorldPosition(startPostion);
        HexTile endTile = WalkableTileMap.GetHexTileOnWorldPosition(endPosition);


        List<HexTile> openSet = new List<HexTile>();
        HashSet<HexTile> closedSet = new HashSet<HexTile>();

        openSet.Add(startTile);

        while (openSet.Count > 0 && endTile != null)
        {

            //find note in the open set with lowest fcost
            HexTile currentTile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentTile.FCost || openSet[i].FCost == currentTile.FCost && openSet[i].HCost < currentTile.HCost)
                {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if (currentTile == endTile)
            {
                pathSuccess = true;
                break;
            }

            //musime najit sousedy
            foreach (Vector3Int neighbourCoordination in currentTile.GetNeighborCoordinationsInDistance(1))
            {

                HexTile neighbour = WalkableTileMap.GetHexTileOnGridPosition(neighbourCoordination);


                if (neighbour == null)
                {
                    continue;
                }

                //walkable, close list
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = (int)currentTile.GCost + (int)currentTile.GetDistanceToCoordination(neighbourCoordination);

                if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = (int)neighbour.GetDistanceToCoordination(endTile.GridCoordination);
                    neighbour.Parent = currentTile;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);


                    }
                }

            }
        }

        yield return null; //wait for one frame before returning;

        if (pathSuccess == true)
        {
            wayPoints = RetracePath(startTile, endTile);
        }

        pathResultCallback(wayPoints, pathSuccess);
    }
}
