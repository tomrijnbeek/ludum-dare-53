using UnityEngine;

public sealed class PoliceVehicle : MonoBehaviour
{
    private Vehicle vehicle;
    public Vehicle Vehicle => vehicle;

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }
}
