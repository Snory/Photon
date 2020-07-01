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


    public float GetDistanceToCoordination(Vector3Int a)
    {
        int y1 = this.GridCoordination.y;
        int y2 = a.y;
        int x1 = this.GridCoordination.x;
        int x2 = a.x;

        int penalty = ((y1 % 2 == 0 && y2 % 2 != 0 && (x1 < x2)) || y2 % 2 == 0 && y1 % 2 != 0 && (x2 < x1)) == true ? 1 : 0;


        return Mathf.Max(Mathf.Abs(y1 - y2), Mathf.Abs(x1 - x2) + Mathf.Floor(Mathf.Abs(y1 - y2) / 2) + penalty);
    }

    public List<Vector3Int> GetNeighborCoordinationsInDistance(int distance)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        List<Vector3Int> notScannedNodes = new List<Vector3Int>();
        List<Vector3Int> ScannedNodes = new List<Vector3Int>();

        //http://ondras.github.io/rot.js/manual/#hex/indexing
        int[][,] evenOdd = new int[6][,];
        evenOdd[0] = new int[1, 2] { { -1, 1 } };
        evenOdd[1] = new int[1, 2] { { -1, 0 } };
        evenOdd[2] = new int[1, 2] { { -1, -1 } };
        evenOdd[3] = new int[1, 2] { { 0, -1 } };
        evenOdd[4] = new int[1, 2] { { 1, 0 } };
        evenOdd[5] = new int[1, 2] { { 0, 1 } };

        int[][,] arrayOdd = new int[6][,];
        arrayOdd[0] = new int[1, 2] { { -1, 0 } };
        arrayOdd[1] = new int[1, 2] { { 1, 0 } };
        arrayOdd[2] = new int[1, 2] { { 0, -1 } };
        arrayOdd[3] = new int[1, 2] { { 1, -1 } };
        arrayOdd[4] = new int[1, 2] { { 0, 1 } };
        arrayOdd[5] = new int[1, 2] { { 1, 1 } };


        notScannedNodes.Add(this.GridCoordination);

        while (notScannedNodes.Count > 0)
        {

            Vector3Int currentTileCoordination = notScannedNodes[0];
            int odd = currentTileCoordination.y % 2 == 0 ? 0 : 1;


            for (int x = 0; x < 6; x++)
            {
                int checkX;
                int checkY;
                if (odd == 0)
                {

                    checkX = currentTileCoordination.x + evenOdd[x][0, 0];
                    checkY = currentTileCoordination.y + evenOdd[x][0, 1];
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

