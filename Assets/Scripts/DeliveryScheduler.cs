using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class DeliveryScheduler : Singleton<DeliveryScheduler>
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private CityMap cityMap;
    [Readonly] private int points;

    private readonly List<Building> buildings = new();
    private readonly List<Order> orders = new();
    private readonly Dictionary<Vector3Int, Order> ordersByTile = new();

    public bool AllOrdersDelivered => orders.Count == 0;

    public void RegisterBuilding(Building building)
    {
        buildings.Add(building);
    }

    public void ProcessTurnStart(int newTurnNumber)
    {
        if (orders.Any(o => o.ExpiryTurn <= newTurnNumber))
        {
            TurnState.Instance.Lose();
        }
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
        var popupObj = Instantiate(popupPrefab, b.transform, true);
        popupObj.transform.position = cityMap.TileToCenterWorld(b.RoadTile);
        var popup = popupObj.GetComponent<OrderPopup>();

        var order = new Order(b, indicatorObj, popup, currentTurn, currentTurn + duration);
        popup.SetOrder(order);
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
        Debug.Log($"{points} points");
        Destroy(order.Indicator);
        Destroy(order.Popup.gameObject);
        orders.Remove(order);
        ordersByTile.Remove(tile);
    }

    public sealed record Order(
        Building Building, GameObject Indicator, OrderPopup Popup, int PlacedTurn, int ExpiryTurn)
    {
        public Building Building { get; } = Building;
        public GameObject Indicator { get; } = Indicator;
        public OrderPopup Popup { get; } = Popup;
        public int PlacedTurn { get; } = PlacedTurn;
        public int ExpiryTurn { get; } = ExpiryTurn;
    }
}
