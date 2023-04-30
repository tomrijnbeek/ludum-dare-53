using System.Collections.Generic;
using UnityEngine;

public sealed class TurnState : MonoBehaviour
{
    [SerializeField] private PoliceManager policeManager;
    [SerializeField] private TruckMovement playerMovement;
    [SerializeField] private VehicleLocations vehicleLocations;
    [SerializeField] private float movementTicksPerSecond = 6;
    [Readonly] private State state;
    [Readonly] private int turnNumber;

    private DeliveryScheduler deliveries;

    private float movementTickDuration => 1 / movementTicksPerSecond;

    private readonly List<VehicleMovement> ongoingMovements = new();
    private float nextMovementTick;

    private void Start()
    {
        deliveries = GetComponent<DeliveryScheduler>();
        playerMovement.PathSelected += onPlayerPathSelected;
        toPlayerInputState();
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
        policeManager.PrepareTurn();
    }

    private void toMovementState()
    {
        state = State.Movement;
        ongoingMovements.Add(playerMovement.CommitMovement());
        ongoingMovements.AddRange(policeManager.CommitMovement());
        nextMovementTick = Time.time;
    }

    private enum State
    {
        PlayerInput,
        Movement,
    }
}
