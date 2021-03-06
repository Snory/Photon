﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexTileMap : MonoBehaviourPun
{

    private Tilemap _tilemap;

    public HexTile[,] Tiles { get; private set; }


    private void Awake()
    {
        _tilemap = this.gameObject.GetComponent<Tilemap>();

        if(_tilemap == null)
        {
            Debug.LogError("[HextTileMap]: Tilemap is not available.");
        }

        CreateTileMapArray();
    }

    private void CreateTileMapArray()
    {
        Tiles = new HexTile[_tilemap.size.x, _tilemap.size.y];

        for (int i = _tilemap.origin.x; i < _tilemap.origin.x + _tilemap.size.x; i++)
        {
            for (int j = _tilemap.origin.y; j < _tilemap.origin.y + _tilemap.size.y; j++)
            {
                Vector3Int tileGridCoordination = new Vector3Int(i, j, 0);
                TileBase tileBase = _tilemap.GetTile(tileGridCoordination);

                if (tileBase != null)
                {
                    HexTile currentTile = new HexTile(tileGridCoordination, _tilemap.CellToWorld(tileGridCoordination), _tilemap);                  

                    Tiles[i + Math.Abs(_tilemap.origin.x), j + Math.Abs(_tilemap.origin.y)] = currentTile;

                }

            }
        }
    }

    
    public HexTile GetHexTile(Vector3 worldPostion)
    {
        HexTile tile = null;

        Vector3Int tileCoordinatesInGrid = _tilemap.WorldToCell(new Vector3(worldPostion.x, worldPostion.y, _tilemap.origin.z));

        int checkX = tileCoordinatesInGrid.x + Math.Abs(_tilemap.origin.x);
        int checkY = tileCoordinatesInGrid.y + Math.Abs(_tilemap.origin.y);

        if (checkX < _tilemap.size.x && checkY < _tilemap.size.y)
        {
            tile = GetHexTile(checkX, checkY);
        }
        return tile;
    }

    public HexTile GetHexTile(Vector3Int gridPosition)
    {
        HexTile tile = null;

        int checkX = gridPosition.x + Math.Abs(_tilemap.origin.x);
        int checkY = gridPosition.y + Math.Abs(_tilemap.origin.y);

        if (checkX < _tilemap.size.x && checkY < _tilemap.size.y)
        {
            tile = GetHexTile(checkX, checkY);
        }
        return tile;
    }


    private HexTile GetHexTile(int row, int column)
    {
        if (row < _tilemap.size.x && column < _tilemap.size.y && column >= 0 && row >=0)
        {
            return Tiles[row, column];
        }

        return null;
    }

    [PunRPC]
    public void SetIsHexTileWalkable(Vector3 tilePosition, bool walkable)
    {
        GetHexTile(tilePosition).Walkable = walkable;
    }
}
