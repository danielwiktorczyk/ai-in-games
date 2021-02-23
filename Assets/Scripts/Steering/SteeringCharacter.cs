//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices.WindowsRuntime;
//using UnityEngine;

public enum SteeringBehaviourSelection
{
    Idle,
    SteeringWander,
    SteeringSeek,
    SteeringFlee,
    SteeringArrive,
    SteeringPersue,
    SteeringEvade
}

public enum OrientationBehaviourSelection
{
    None,
    SteeringAlign,
    SteeringFace,
    SteeringFaceAway,
    SteeringLookWhereYoureGoing
}

//public class SteeringCharacter : MonoBehaviour, AIBody
//{
//    public SteeringBehaviourSelection SteeringBehaviourSelection;
//    public OrientationBehaviourSelection OrientationBehaviourSelection;

//    private SteeringOutput SteeringOutput;

//    public Vector3 CurrentVelocity;
//    public Vector3 CurrentAngularVelocity;
//    public float MaxSpeed;
//    public float MaxAcceleration;
//    public float MaxAngularSpeed;
//    public float MaxAngularAcceleration;
//    public AIBody Target;
//    private Collider Collider;

//    [SerializeField] private SteeringWander SteeringWander;
//    [SerializeField] private SteeringSeek SteeringSeek;
//    [SerializeField] private SteeringFlee SteeringFlee;
//    [SerializeField] private SteeringArrive SteeringArrive;
//    [SerializeField] private SteeringAlign SteeringAlign;
//    [SerializeField] private SteeringPersue SteeringPersue;
//    [SerializeField] private SteeringFace SteeringFace;
//    [SerializeField] private SteeringLookWhereYoureGoing SteeringLookWhereYoureGoing;
//    [SerializeField] private SteeringEvade SteeringEvade;

//    public void Awake()
//    {
//        Collider = GetComponent<Collider>();
//    }

//    AIBody AIBody.Target { get => Target; }
//    Vector3 AIBody.CurrentVelocity { get => CurrentVelocity; }
//    Vector3 AIBody.CurrentAngularVelocity { get => CurrentAngularVelocity; }
//    float AIBody.MaxSpeed { get => MaxSpeed; }
//    float AIBody.MaxAcceleration { get => MaxAcceleration; }
//    float AIBody.MaxAngularSpeed { get => MaxAngularSpeed; }
//    float AIBody.MaxAngularAcceleration { get => MaxAngularAcceleration; }
//    Collider AIBody.Collider => Collider;

//    Transform AIBody.transform => transform;

//    private void Move()
//    {
//        switch (SteeringBehaviourSelection)
//        {
//            case SteeringBehaviourSelection.SteeringWander:
//                SteeringOutput = SteeringWander.GetSteering();
//                break;
//            case SteeringBehaviourSelection.SteeringSeek:
//                SteeringOutput = SteeringSeek.GetSteering(Target.transform.position);
//                break;
//            case SteeringBehaviourSelection.SteeringFlee:
//                SteeringOutput = SteeringFlee.GetSteering(Target.transform.position);
//                break;
//            case SteeringBehaviourSelection.SteeringArrive:
//                SteeringOutput = SteeringArrive.GetSteering();
//                break;
//            case SteeringBehaviourSelection.SteeringPersue:
//                SteeringOutput = SteeringPersue.GetSteering();
//                break;
//            case SteeringBehaviourSelection.SteeringEvade:
//                SteeringOutput = SteeringEvade.GetSteering();
//                break;
//            default:
//                break;
//        }

//        switch (OrientationBehaviourSelection)
//        {
//            case OrientationBehaviourSelection.None:
//                break;
//            case OrientationBehaviourSelection.SteeringAlign:
//                SteeringOutput.Angular = SteeringAlign.GetSteering(Target.transform.rotation.eulerAngles.y).Angular;
//                break;
//            case OrientationBehaviourSelection.SteeringFace:
//                if (SteeringBehaviourSelection != SteeringBehaviourSelection.SteeringWander)
//                    SteeringOutput.Angular = SteeringFace.GetSteering(Target.transform.position).Angular; 
//                break;
//            case OrientationBehaviourSelection.SteeringLookWhereYoureGoing:
//                SteeringOutput.Angular = SteeringLookWhereYoureGoing.GetSteering().Angular; 
//                break;
//        };

//        UpdateKinematics();
//    }

//    void Update()
//    {
//        Move();
//    }

//    private void ClipMaxVelocities()
//    {
//        if (Mathf.Abs(CurrentVelocity.magnitude) > MaxSpeed)
//            CurrentVelocity = CurrentVelocity.normalized * MaxSpeed;

//        if (Mathf.Abs(CurrentAngularVelocity.magnitude) > MaxAngularSpeed)
//            CurrentAngularVelocity = CurrentAngularVelocity.normalized * MaxAngularSpeed;
//    }

//    private void UpdateKinematics()
//    {
//        // Update position and orientation
//        // TODO
//        transform.position += CurrentVelocity * Time.deltaTime;
//        // TODO Rotation

//        // Update velocity and angular velocity
//        CurrentVelocity += SteeringOutput.Linear * Time.deltaTime;
//        CurrentAngularVelocity += SteeringOutput.Angular * Time.deltaTime * Vector3.up;

//        ClipMaxVelocities();
//    }
//}
