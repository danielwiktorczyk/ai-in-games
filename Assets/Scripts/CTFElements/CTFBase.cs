using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTFBase : MonoBehaviour, AIBody
{
    public Teams Team;

    public AIBody Target => null;
    public Vector3 CurrentVelocity => Vector3.zero;
    public Vector3 CurrentAngularVelocity => Vector3.zero;
    public float MaxSpeed => 0;
    public float MaxAcceleration => 0;
    public float MaxAngularSpeed => 0;
    public float MaxAngularAcceleration => 0;
}