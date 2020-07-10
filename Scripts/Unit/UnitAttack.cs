using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAttack : MonoBehaviour
{
    [Header("Stats")]
    [Range(1,5)]
    public int AttackRange;

    public GameObject AttackVisualization;
    private List<GameObject> _attackVisualization;


    private void Start()
    {
        _attackVisualization = new List<GameObject>();
    }


    public void DisplayAttackRange(HexTile aroundTile, UnitMovement movement, int maxDistance)
    {
        List<Vector3Int> neighbors = aroundTile.GetNeighborCoordinations(AttackRange);
         
        
        foreach (Vector3Int neighbor in neighbors.Where(n => movement.CurrentHexTile.GetDistanceToCoordination(n) <= movement.MaxDistance))
        {            

            GameObject attackVisualization = Instantiate(AttackVisualization, PathFinder.Instance.WalkableTileMap.GetHexTile(neighbor).WorldCoordination, Quaternion.identity);
            attackVisualization.transform.parent = this.transform;
            _attackVisualization.Add(attackVisualization);
        }
 
    }

    public void HideAttackRange()
    {
        foreach (GameObject obj in _attackVisualization)
        {
            Destroy(obj);
        }

        _attackVisualization.Clear();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
