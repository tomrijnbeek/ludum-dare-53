using JetBrains.Annotations;
using UnityEngine;

public sealed class TurnState : MonoBehaviour
{
    [SerializeField] private PoliceManager policeManager;
    [SerializeField] private TruckMovement playerMovement;
    [Readonly] private State state;

    [CanBeNull] private VehicleMovement ongoingMovement;

    private void Start()
    {
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
        if (state == State.Movement && (ongoingMovement?.Done ?? true))
        {
            toPlayerInputState();
        }
    }

    private void toPlayerInputState()
    {
        state = State.PlayerInput;
        ongoingMovement = null;
        policeManager.PrepareTurn();
    }

    private void toMovementState()
    {
        state = State.Movement;
        ongoingMovement = VehicleMovement.Composite(
            playerMovement.ExecuteTurn(),
            policeManager.ExecuteTurn());
    }

    private enum State
    {
        PlayerInput,
        Movement,
    }
}
