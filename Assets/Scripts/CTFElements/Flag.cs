using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour, AIBody
{
    public Teams Team;
    private Vector3 SpawnPosition;
    private Collider Collider;

    private void Awake()
    {
        SpawnPosition = transform.position;
        Collider = GetComponent<Collider>();
    }

    public AIBody Target => null;
    public Vector3 CurrentVelocity => Vector3.zero;
    public Vector3 CurrentAngularVelocity => Vector3.zero;
    public float MaxSpeed => 0;
    public float MaxAcceleration => 0;
    public float MaxAngularSpeed => 0;
    public float MaxAngularAcceleration => 0;
    Collider AIBody.Collider => Collider;

    internal void ResetPosition()
    {
        transform.SetPositionAndRotation(SpawnPosition, transform.rotation);
    }

    public void SetTarget(GameObject target)
    {
        throw new NotImplementedException();
    }
}
