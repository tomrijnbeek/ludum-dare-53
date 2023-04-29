using UnityEngine;
using UnityEngine.Tilemaps;

public sealed class TruckMovement : MonoBehaviour
{
    [SerializeField] private GameObject truck;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Grid grid;

    private void Start()
    {
        //var cellCenter = grid.GetCellCenterWorld(new Vector3Int(2, 2));
        //truck.transform.position = cellCenter;
    }
}
