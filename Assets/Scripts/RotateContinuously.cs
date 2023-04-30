using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RotateContinuously : MonoBehaviour
{
    [SerializeField] private float angularSpeed = 90;
    [SerializeField] private Vector3 axis = Vector3.up;

    // Update is called once per frame
    void Update()
    {
        var angle = angularSpeed * Time.time % 360;
        transform.rotation = Quaternion.AngleAxis(angle, axis);
    }
}
