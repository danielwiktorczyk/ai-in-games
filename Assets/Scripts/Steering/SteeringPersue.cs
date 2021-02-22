using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringPersue : SteeringSeek
{
    [SerializeField] private float TimeToTarget; // Over which to achieve a target speed
    [SerializeField] private float MaxPrediction; // The maximum prediction time

    private void Awake()
    {
        Character = GetComponent<AIBody>();
    }
    public SteeringOutput GetSteering()
    {
        // Calculate the target to delegate to seek
        var direction = Character.Target.transform.position - Character.transform.position;
        direction.y = 0;
        var distance = direction.magnitude;

        // Work out current speed
        var speed = Character.CurrentVelocity.magnitude;

        var prediction = 0f;
        // Check if speed gives a reasonable prediction time.
        if (speed <= distance / MaxPrediction)
            prediction = MaxPrediction;
        else // Otherwise calculate the prediction time
            prediction = distance / speed;

        // Put the target together
        var predictedTargetPosition = Character.Target.transform.position + Character.Target.CurrentVelocity * prediction;
        Debug.DrawLine(transform.position, predictedTargetPosition, Color.green);


        // Delegate to Seek
        return base.GetSteering(predictedTargetPosition);
    }
}
