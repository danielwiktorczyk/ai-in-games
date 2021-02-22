using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AIBody
{
    public AIBody Target { get; }
    public Vector3 CurrentVelocity { get; }
    public Vector3 CurrentAngularVelocity { get; }
    public float MaxSpeed { get; }
    public float MaxAcceleration { get; }
    public float MaxAngularSpeed { get; }
    public float MaxAngularAcceleration { get; }
    public Transform transform { get; }
}
