using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] private float rotationAngle = 5;

    private void Update()
    {
        transform.Rotate(transform.forward * rotationAngle);
    }
}
