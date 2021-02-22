using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum KinematicBehaviourSelection
{
    Idle,
    KinematicWander,
    KinematicSeek,
    KinematicFlee,
    KinematicArrive
}

public class KinematicCharacter : MonoBehaviour, AIBody
{
    public KinematicBehaviourSelection KinematicBehaviourSelection;

    private KinematicSteeringOutput KinematicSteeringOutput;
    
    public AIBody Target;
    public Vector3 CurrentVelocity;
    public Vector3 CurrentAngularVelocity;
    public float MaxSpeed;
    private Collider Collider;

    [SerializeField] private KinematicWander KinematicWander;
    [SerializeField] private KinematicSeek KinematicSeek;
    [SerializeField] private KinematicFlee KinematicFlee;
    [SerializeField] private KinematicArrive KinematicArrive;

    public void Awake()
    {
        Collider = GetComponent<Collider>();
    }

    AIBody AIBody.Target { get => Target; }
    Vector3 AIBody.CurrentVelocity { get => CurrentVelocity; }
    Vector3 AIBody.CurrentAngularVelocity { get => CurrentAngularVelocity; }
    float AIBody.MaxSpeed { get => MaxSpeed; }
    float AIBody.MaxAcceleration { get => 0; }
    float AIBody.MaxAngularSpeed { get => 0; }
    float AIBody.MaxAngularAcceleration { get => 0; }
    Collider AIBody.Collider => Collider;


    Transform AIBody.transform => transform;

    private void Move()
    {
        switch (KinematicBehaviourSelection)
        {
            case KinematicBehaviourSelection.KinematicWander:
                KinematicSteeringOutput = KinematicWander.GetSteering();
                break;
            case KinematicBehaviourSelection.KinematicSeek:
                KinematicSteeringOutput = KinematicSeek.GetSteering();
                break;
            case KinematicBehaviourSelection.KinematicFlee:
                KinematicSteeringOutput = KinematicFlee.GetSteering();
                break;
            case KinematicBehaviourSelection.KinematicArrive:
                KinematicSteeringOutput = KinematicArrive.GetSteering();
                break;
            default:
                break;
        }

        UpdateKinematics();

        ClipMaxVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void ClipMaxVelocity()
    {
        if (CurrentVelocity.magnitude > MaxSpeed)
            CurrentVelocity = CurrentVelocity.normalized * MaxSpeed;
    }

    private void UpdateKinematics()
    {
        // Update position
        // TODO
        transform.position += CurrentVelocity * Time.deltaTime;
        Debug.DrawLine(transform.position, transform.position + CurrentVelocity);

        // Update orientation
        //float newRotationAngle = transform.rotation.eulerAngles.y + (KinematicSteeringOutput.Rotation * Time.deltaTime);
        float newRotationAngle = Mathf.Lerp(
            AngleMapper.MapDegreesMidpointZero(transform.rotation.eulerAngles.y),
            KinematicSteeringOutput.Rotation,
            1f * Time.deltaTime
        );
        CurrentAngularVelocity = Vector3.zero; // We want to force our rotations
        transform.rotation = Quaternion.AngleAxis(newRotationAngle, Vector3.up);

        // Update velocity
        CurrentVelocity = KinematicSteeringOutput.Velocity;

        // We do not update Angular velocity for KinematicMovement 
    }
}
