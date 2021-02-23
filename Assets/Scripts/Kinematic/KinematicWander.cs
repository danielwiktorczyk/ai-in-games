using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicWander : MonoBehaviour, IKinematicMovement
{
    public float MaxRotationSpeed;

    private AIBody Character;

    private void Awake()
    {
        Character = GetComponent<AIBody>();
    }

    public KinematicSteeringOutput GetSteering()
    {
        KinematicSteeringOutput output = new KinematicSteeringOutput
        {
            Velocity = Character.MaxSpeed * transform.forward, // Get velocity from the vector form of the orientation.
            Rotation = Character.transform.rotation.eulerAngles.y + RandomBinomial() * MaxRotationSpeed // Change our orientation randomly.
        };
        Debug.Log(output.Rotation);

        return output;
    }

    private static float RandomBinomial() => Random.value - Random.value;
}  
