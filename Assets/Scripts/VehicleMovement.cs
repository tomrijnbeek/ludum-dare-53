using System;
using System.Collections.Generic;

public sealed class VehicleMovement
{
    private readonly Vehicle vehicle;
    private readonly Queue<Direction> pathQueue = new();
    private readonly Func<bool> checkFinished;

    public bool Done => pathQueue.Count == 0;

    public VehicleMovement(Vehicle vehicle, Path path)
    {
        this.vehicle = vehicle;
        foreach (var d in path.Directions)
        {
            pathQueue.Enqueue(d);
        }
    }

    public void DoTick(float tickDuration)
    {
        if (!pathQueue.TryDequeue(out var dir))
        {
            return;
        }

        vehicle.MoveInDirection(dir, tickDuration);
    }
}
