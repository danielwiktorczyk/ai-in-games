using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringVelocityMatch : MonoBehaviour, ISteeringMovement
{
    private AIBody Character;

    [SerializeField] private float TimeToTarget; // Over which to achieve a target speed

    private void Awake()
    {
        Character = GetComponent<AIBody>();
    }

    public SteeringOutput GetSteering()
    {
        SteeringOutput output = new SteeringOutput();

        // Acceleration tries to get to the target velocity.
        output.Linear = (Character.Target.CurrentVelocity - Character.CurrentVelocity) / TimeToTarget;

        // Check if the acceleration is too fast.
        if (output.Linear.magnitude > Character.MaxAcceleration)
            output.Linear = output.Linear.normalized * Character.MaxAcceleration;

        output.Angular = 0;

        return output;
    }
}
