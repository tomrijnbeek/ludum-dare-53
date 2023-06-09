using System.Collections.Generic;
using UnityEngine;

public sealed class TurnState : Singleton<TurnState>
{
    [SerializeField] private PoliceManager policeManager;
    [SerializeField] private TruckMovement playerMovement;
    [SerializeField] private VehicleLocations vehicleLocations;
    [SerializeField] private float movementTicksPerSecond = 6;
    [SerializeField] private int turnsBetweenOrders = 5;
    [SerializeField] private int turnsBetweenPoliceSpawns = 16;
    [SerializeField] private int turnsBetweenMinOrderIncreases = 27;
    [SerializeField] private int orderTimeout = 8;
    [Readonly] private State state;
    [Readonly] private int turnNumber;
    [Readonly] private int nextOrder = 1;
    [Readonly] private int nextPoliceSpawn = 1;

    public int TurnNumber => turnNumber;
    public bool FirstMovementDone { get; private set; }
    public bool GameOver => state == State.Lost;

    private DeliveryScheduler deliveries;

    private float movementTickDuration => 1 / movementTicksPerSecond;

    private readonly List<VehicleMovement> ongoingMovements = new();
    private float nextMovementTick;

    private void Start()
    {
        deliveries = GetComponent<DeliveryScheduler>();
        playerMovement.PathSelected += onPlayerPathSelected;
        state = State.Movement;
    }

    private void onPlayerPathSelected()
    {
        if (state == State.PlayerInput)
        {
            toMovementState();
        }
    }

    private void Update()
    {
        if (state != State.Movement) return;

        if (Time.time >= nextMovementTick)
        {
            var allDone = true;
            foreach (var vm in ongoingMovements)
            {
                allDone = allDone && vm.Done;
                if (!vm.Done)
                {
                    vm.DoTick(movementTickDuration);
                }
            }

            if (allDone)
            {
                toPlayerInputState();
            }
            else
            {
                vehicleLocations.CommitTransitions();
                nextMovementTick = Time.time + movementTickDuration;
            }
        }
    }

    private void toPlayerInputState()
    {
        turnNumber++;
        state = State.PlayerInput;
        ongoingMovements.Clear();
        deliveries.ProcessTurnStart(turnNumber);

        if (state == State.Lost)
        {
            return;
        }

        if (nextPoliceSpawn <= turnNumber)
        {
            policeManager.SpawnPoliceCar(playerMovement.CurrentTile);
            nextPoliceSpawn = turnNumber + turnsBetweenPoliceSpawns;
        }

        var minActiveOrders = 1 + turnNumber / turnsBetweenMinOrderIncreases;
        while (nextOrder <= turnNumber || deliveries.OpenOrderCount < minActiveOrders)
        {
            deliveries.PlaceOrder(turnNumber, orderTimeout);
            nextOrder = turnNumber + turnsBetweenOrders;
        }
        policeManager.PrepareTurn();
    }

    private void toMovementState()
    {
        state = State.Movement;
        ongoingMovements.Add(playerMovement.CommitMovement());
        ongoingMovements.AddRange(policeManager.CommitMovement());
        nextMovementTick = Time.time;
        FirstMovementDone = true;
    }

    public void Lose()
    {
        state = State.Lost;
    }

    private enum State
    {
        PlayerInput,
        Movement,
        Lost,
    }
}
