using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexTile
{
    public int FCost { get { return GCost + HCost; } }

    public int GCost { get; set; }
    public int HCost { get; set; }
    public Vector3Int GridCoordination { get; set; }

    public Vector3 WorldCoordination { get; set; }
    public Tilemap TileMap { get; set; }
    public HexTile Parent { get; set; }

    public Dictionary<Direction, int[,]> oddHexTileDirectionsCoordinates;

    public Dictionary<Direction, int[,]> evenHexTileDirectionsCoordinates;

    public HexTile(Vector3Int GridCoordination, Vector3 WorldCoordination, Tilemap Tilemap)
    {

        this.GridCoordination = GridCoordination;
        this.WorldCoordination = WorldCoordination;
        this.TileMap = Tilemap;

        oddHexTileDirectionsCoordinates = new Dictionary<Direction, int[,]>();
        evenHexTileDirectionsCoordinates = new Dictionary<Direction, int[,]>();

        evenHexTileDirectionsCoordinates.Add(Direction.LEFTTOP, new int[1, 2] { { -1, 1 } });
        evenHexTileDirectionsCoordinates.Add(Direction.RIGHTTOP, new int[1, 2] { { -1, 0 } });
        evenHexTileDirectionsCoordinates.Add(Direction.LEFT, new int[1, 2] { { -1, -1 } });
        evenHexTileDirectionsCoordinates.Add(Direction.RIGHT, new int[1, 2] { { 0, -1 } });
        evenHexTileDirectionsCoordinates.Add(Direction.LEFTBOT, new int[1, 2] { { 1, 0 } });
        evenHexTileDirectionsCoordinates.Add(Direction.RIGHTBOT, new int[1, 2] { { 0, 1 } });

        oddHexTileDirectionsCoordinates.Add(Direction.LEFTTOP, new int[1, 2] { { -1, 0 } });
        oddHexTileDirectionsCoordinates.Add(Direction.RIGHTTOP, new int[1, 2] { { 1, 0 } });
        oddHexTileDirectionsCoordinates.Add(Direction.LEFT, new int[1, 2] { { 0, -1 } });
        oddHexTileDirectionsCoordinates.Add(Direction.RIGHT, new int[1, 2] { { 1, -1 } });
        oddHexTileDirectionsCoordinates.Add(Direction.LEFTBOT, new int[1, 2] { { 0, 1 } });
        oddHexTileDirectionsCoordinates.Add(Direction.RIGHTBOT, new int[1, 2] { { 1, 1 } });


    }

    public float GetDistanceToCoordination(Vector3Int a)
    {
        int y1 = this.GridCoordination.y;
        int y2 = a.y;
        int x1 = this.GridCoordination.x;
        int x2 = a.x;

        int penalty = ((y1 % 2 == 0 && y2 % 2 != 0 && (x1 < x2)) || y2 % 2 == 0 && y1 % 2 != 0 && (x2 < x1)) == true ? 1 : 0;


        return Mathf.Max(Mathf.Abs(y1 - y2), Mathf.Abs(x1 - x2) + Mathf.Floor(Mathf.Abs(y1 - y2) / 2) + penalty);
    }

    public List<Vector3Int> GetNeighborCoordinations(int distance)
    {

        int[][,] arrayEven;
        int[][,] arrayOdd;

        //http://ondras.github.io/rot.js/manual/#hex/indexing

        arrayEven = new int[6][,];
        arrayEven[0] = evenHexTileDirectionsCoordinates[Direction.LEFTTOP];
        arrayEven[1] = evenHexTileDirectionsCoordinates[Direction.RIGHTTOP];
        arrayEven[2] = evenHexTileDirectionsCoordinates[Direction.LEFT];
        arrayEven[3] = evenHexTileDirectionsCoordinates[Direction.RIGHT];
        arrayEven[4] = evenHexTileDirectionsCoordinates[Direction.LEFTBOT];
        arrayEven[5] = evenHexTileDirectionsCoordinates[Direction.RIGHTBOT];

        arrayOdd = new int[6][,];
        arrayOdd[0] = oddHexTileDirectionsCoordinates[Direction.LEFTTOP];
        arrayOdd[1] = oddHexTileDirectionsCoordinates[Direction.RIGHTTOP];
        arrayOdd[2] = oddHexTileDirectionsCoordinates[Direction.LEFT];
        arrayOdd[3] = oddHexTileDirectionsCoordinates[Direction.RIGHT];
        arrayOdd[4] = oddHexTileDirectionsCoordinates[Direction.LEFTBOT];
        arrayOdd[5] = oddHexTileDirectionsCoordinates[Direction.RIGHTBOT];

        return FindNeighborInDistanceAndDirection(distance, arrayEven, arrayOdd);

    }

    public List<Vector3Int> GetNeighborCoordinations(int distance, Direction[] allowedDirections)
    {

        int[][,] arrayEven = new int[allowedDirections.Length][,];
        int[][,] arrayOdd = new int[allowedDirections.Length][,];

        //http://ondras.github.io/rot.js/manual/#hex/indexing

        for (int i = 0; i < allowedDirections.Length; i++)
        {
            arrayOdd[i] = oddHexTileDirectionsCoordinates[allowedDirections[i]];
            arrayEven[i] = evenHexTileDirectionsCoordinates[allowedDirections[i]];
        }

        return FindNeighborInDistanceAndDirection(distance, arrayEven, arrayOdd);

    }

    private List<Vector3Int> FindNeighborInDistanceAndDirection(int distance, int[][,] arrayEven, int[][,] arrayOdd)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        List<Vector3Int> notScannedNodes = new List<Vector3Int>();

        notScannedNodes.Add(this.GridCoordination);

        while (notScannedNodes.Count > 0)
        {

            Vector3Int currentTileCoordination = notScannedNodes[0];
            int odd = currentTileCoordination.y % 2 == 0 ? 0 : 1;


            for (int x = 0; x < arrayEven.Length; x++)
            {
                int checkX;
                int checkY;
                if (odd == 0)
                {

                    checkX = currentTileCoordination.x + arrayEven[x][0, 0];
                    checkY = currentTileCoordination.y + arrayEven[x][0, 1];
                }
                else
                {

                    checkX = currentTileCoordination.x + arrayOdd[x][0, 0];
                    checkY = currentTileCoordination.y + arrayOdd[x][0, 1];

                }

                Vector3Int neighborCoordination = new Vector3Int(checkX, checkY, TileMap.origin.z);

                TileBase neighbor = TileMap.GetTile(neighborCoordination);

                if (neighbor != null && GetDistanceToCoordination(neighborCoordination) <= distance)
                {

                    if (!notScannedNodes.Contains(neighborCoordination) && !neighbors.Contains(neighborCoordination))
                    {
                        notScannedNodes.Add(neighborCoordination);
                    }

                    if (!neighbors.Contains(neighborCoordination))
                    {
                        neighbors.Add(neighborCoordination);
                    }
                }
            }
            notScannedNodes.Remove(currentTileCoordination);
        }

        return neighbors;
    }
}

