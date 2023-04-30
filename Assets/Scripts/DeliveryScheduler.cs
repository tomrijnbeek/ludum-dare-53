using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class DeliveryScheduler : Singleton<DeliveryScheduler>
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private CityMap cityMap;
    [Readonly] private int points;

    private readonly List<Building> buildings = new();
    private readonly List<Order> orders = new();
    private readonly Dictionary<Vector3Int, Order> ordersByTile = new();

    private void Update()
    {
        if (orders.Count == 0)
        {
            PlaceOrder(0, 0);
        }
    }

    public void RegisterBuilding(Building building)
    {
        buildings.Add(building);
    }

    public void PlaceOrder(int currentTurn, int duration)
    {
        if (buildings.Count == 0)
        {
            Debug.LogWarning("Attempted to place order but no buildings spawned.");
            return;
        }

        var b = buildings[Random.Range(0, buildings.Count)];
        var indicatorObj = Instantiate(indicator, b.transform, true);
        indicatorObj.transform.position = cityMap.TileToCenterWorld(b.RoadTile);

        var order = new Order(b, indicatorObj, currentTurn, currentTurn + duration);
        orders.Add(order);
        ordersByTile.Add(b.RoadTile, order);
    }

    public void ProcessPlayerInTile(Vector3Int tile)
    {
        if (!ordersByTile.TryGetValue(tile, out var order))
        {
            return;
        }

        points++;
        Destroy(order.Indicator);
        orders.Remove(order);
        ordersByTile.Remove(tile);
    }

    public readonly struct Order
    {
        public Building Building { get; }
        public GameObject Indicator { get; }
        public int PlacedTurn { get; }
        public int ExpiryTurn { get; }

        public Order(Building building, GameObject indicator, int placedTurn, int expiryTurn)
        {
            Building = building;
            Indicator = indicator;
            PlacedTurn = placedTurn;
            ExpiryTurn = expiryTurn;
        }
    }
}
