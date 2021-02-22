using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour, AIBody
{
    public Teams Team;
    Vector3 SpawnPosition;

    public AIBody Target => null;
    public Vector3 CurrentVelocity => Vector3.zero;
    public Vector3 CurrentAngularVelocity => Vector3.zero;
    public float MaxSpeed => 0;
    public float MaxAcceleration => 0;
    public float MaxAngularSpeed => 0;
    public float MaxAngularAcceleration => 0;

    private void Awake()
    {
        SpawnPosition = transform.position;
    }

    internal void ResetPosition()
    {
        transform.position = SpawnPosition;
    }
}
