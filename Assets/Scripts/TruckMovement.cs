using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public sealed class TruckMovement : MonoBehaviour
{
    [SerializeField] private GameObject truck;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Grid grid;
    [SerializeField] private new Camera camera;
    private Truck truckComponent;

    private void Start()
    {
        truckComponent = truck.GetComponent<Truck>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, 0);
            if (plane.Raycast(ray, out var t))
            {
                var point = ray.GetPoint(t);
                var cell = grid.WorldToCell(point);
                truckComponent.SetDestination(cell);
            }
        }
    }
}
