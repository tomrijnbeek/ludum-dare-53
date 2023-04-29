using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DeleteOnStart : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject);
    }
}
