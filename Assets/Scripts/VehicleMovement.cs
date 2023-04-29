using System;
using System.Collections.Generic;
using System.Linq;

public sealed class VehicleMovement
{
    private readonly Func<bool> checkFinished;

    public bool Done => checkFinished();

    public VehicleMovement(Func<bool> checkFinished)
    {
        this.checkFinished = checkFinished;
    }

    public static VehicleMovement Composite(params VehicleMovement[] movements) =>
        Composite((IEnumerable<VehicleMovement>) movements);

    public static VehicleMovement Composite(IEnumerable<VehicleMovement> movements)
    {
        var enumerated = movements.ToList();
        return new VehicleMovement(() => enumerated.All(vm => vm.Done));
    }
}
