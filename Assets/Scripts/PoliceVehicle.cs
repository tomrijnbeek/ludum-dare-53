using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PoliceVehicle : MonoBehaviour
{
    [SerializeField] private PoliceManager policeManager;
    private Vehicle vehicle;

    public Vehicle Vehicle => vehicle;

    // Start is called before the first frame update
    void Start()
    {
        vehicle = GetComponent<Vehicle>();
        policeManager.RegisterVehicle(this);
    }
}
