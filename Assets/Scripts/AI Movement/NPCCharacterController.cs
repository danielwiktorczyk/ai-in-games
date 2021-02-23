using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum HumanHeuristicMode
{
    None,
    ReachingATarget,
    StayingAwayFromATarget
}

public class NPCCharacterController : MonoBehaviour, AIBody
{

    [SerializeField] protected bool KinematicModeEnabled;
    [SerializeField] protected bool HumanHeuristicsModeEnabled;

    public AIBody Target;
    public GameObject TargetGameObject;
    public Vector3 CurrentVelocity;
    public Vector3 CurrentAngularVelocity;
    public float CurrentSpeed;
    public float MaxSpeed;
    public float MaxAcceleration;
    public float MaxAngularSpeed;
    public float MaxAngularAcceleration;
    public Collider Collider;

    private float TargetRotationDelta;
    private float TargetRotationDeltaInterpolationT;

    [SerializeField] protected float SteppingRadius;
    [SerializeField] protected float SteppingSpeed;
    [SerializeField] protected float MaximumPerceptionZoneAngle;
    [SerializeField] protected float MinimumPerceptionZoneAngle;
    [SerializeField] protected float MaximumPerceptionZoneScale;
    [SerializeField] protected float MinimumPerceptionZoneSpeed;

    protected KinematicSteeringOutput KinematicSteeringOutput;
    protected SteeringOutput SteeringOutput;

    // Kinematic Movement
    public KinematicBehaviourSelection KinematicBehaviourSelection;
    [SerializeField] protected KinematicWander KinematicWander;
    [SerializeField] protected KinematicSeek KinematicSeek;
    [SerializeField] protected KinematicFlee KinematicFlee;
    [SerializeField] protected KinematicArrive KinematicArrive;

    // Steering Movement
    public SteeringBehaviourSelection SteeringBehaviourSelection;
    public OrientationBehaviourSelection OrientationBehaviourSelection;
    [SerializeField] protected SteeringWander SteeringWander;
    [SerializeField] protected SteeringSeek SteeringSeek;
    [SerializeField] protected SteeringFlee SteeringFlee;
    [SerializeField] protected SteeringArrive SteeringArrive;
    [SerializeField] protected SteeringAlign SteeringAlign;
    [SerializeField] protected SteeringPersue SteeringPersue;
    [SerializeField] protected SteeringFace SteeringFace;
    [SerializeField] protected SteeringLookWhereYoureGoing SteeringLookWhereYoureGoing;
    [SerializeField] protected SteeringEvade SteeringEvade;

    // Human Movement
    public HumanHeuristicMode HumanHeuristicMode;

    public void Awake()
    {
        Collider = GetComponent<Collider>();
        if (TargetGameObject != null)
            SetTarget(TargetGameObject);
    }

    AIBody AIBody.Target { get => Target; }
    Vector3 AIBody.CurrentVelocity { get => CurrentVelocity; }
    Vector3 AIBody.CurrentAngularVelocity { get => CurrentAngularVelocity; }
    float AIBody.MaxSpeed { get => MaxSpeed; }
    float AIBody.MaxAcceleration { get => MaxAcceleration; }
    float AIBody.MaxAngularSpeed { get => MaxAngularSpeed; }
    float AIBody.MaxAngularAcceleration { get => MaxAngularAcceleration; }
    Transform AIBody.transform => transform;
    Collider AIBody.Collider => Collider;


    // Update is called once per frame
    virtual public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ToggleKinematicMode();

        if (Input.GetKeyDown(KeyCode.H))
            ToggleHumanHeuristicsMode();


        if (HumanHeuristicsModeEnabled)
            HumanHeuristicsModeUpdate();
        else
            Move();
    }

    protected void ToggleKinematicMode()
    {
        KinematicModeEnabled = !KinematicModeEnabled;
    }

    protected void ToggleHumanHeuristicsMode()
    {
        HumanHeuristicsModeEnabled = !HumanHeuristicsModeEnabled;
    }

    protected void Move()
    {
        if (KinematicModeEnabled)
            KinematicMove();
        else
            SteeringMove();
    }

    protected void HumanHeuristicsModeUpdate()
    {
        switch (HumanHeuristicMode)
        {
            case HumanHeuristicMode.None:
                if (KinematicModeEnabled)
                {
                    if (KinematicBehaviourSelection != KinematicBehaviourSelection.KinematicWander)
                        return;

                    KinematicSteeringOutput = KinematicWander.GetSteering();
                    UpdateKinematics();
                }
                else
                {
                    if (SteeringBehaviourSelection != SteeringBehaviourSelection.SteeringWander)
                        return;

                    SteeringOutput = SteeringWander.GetSteering();
                    UpdateSteeringKinematics();
                }
                break;
            case HumanHeuristicMode.ReachingATarget:
                if (CurrentVelocity.magnitude <= SteppingSpeed)
                    HeuristicA();
                else
                    HeuristicB();
                break;
            case HumanHeuristicMode.StayingAwayFromATarget:
                HeuristicC();
                break;
        }
    }

    private void HeuristicA()
    {
        // A If the character is stationary or moving very slowly then 

        Vector3 direction = Target.transform.position - transform.position;
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance < SteppingRadius)
        {
            // i    If it is a very small distance from its target, it will step there directly, even if this
            //          involves moving backward or sidestepping
            transform.position = new Vector3(
                Target.transform.position.x,
                transform.position.y,
                Target.transform.position.z);
        } else
        {
            // ii   Else if the target is farther away, the character will first turn on the spot to face
            //      its target and then move forward to reach it.

            float facingRotation = Quaternion.LookRotation(direction, Vector3.up).eulerAngles.y;

            if (Quaternion.LookRotation(transform.forward).eulerAngles.y - facingRotation <= 0.01f)
            {
                Move();
                return;
            }
            else
            {
                if (KinematicModeEnabled)
                {
                    KinematicSteeringOutput = new KinematicSteeringOutput
                    {
                        Rotation = facingRotation
                    };
                    UpdateKinematics();
                }
                else
                {
                    SteeringOutput = SteeringFace.GetSteering(Target.transform.position);
                    UpdateSteeringKinematics();
                }

                return;
            }
        }
    }

    private void HeuristicB()
    {
        // B    Else if the character is moving with some speed then 

        float perceptionZoneAngle = Mathf.Lerp(
            MaximumPerceptionZoneAngle,
            MinimumPerceptionZoneAngle,
            (Mathf.Pow(CurrentVelocity.magnitude, MaximumPerceptionZoneScale)) / (MaxSpeed - MinimumPerceptionZoneSpeed));
        perceptionZoneAngle = AngleMapper.MapDegreesMidpointZero(perceptionZoneAngle);
        Debug.DrawLine(
            transform.position,
            transform.position + Quaternion.AngleAxis(perceptionZoneAngle / 2, Vector3.up) * transform.forward * 2,
            Color.white);
        Debug.DrawLine(
            transform.position,
            transform.position + Quaternion.AngleAxis(-perceptionZoneAngle / 2, Vector3.up) * transform.forward * 2,
            Color.white);

        float targetRotation = Quaternion.LookRotation(Target.transform.position - transform.position, Vector3.up).eulerAngles.y;
        targetRotation = AngleMapper.MapDegreesMidpointZero(targetRotation);
        float currentRotation = transform.rotation.eulerAngles.y;
        currentRotation = AngleMapper.MapDegreesMidpointZero(currentRotation);

        if (Mathf.Abs(currentRotation - targetRotation) < (perceptionZoneAngle / 2))
        {
            // i    If the target is within a speed-dependent arc (like a narrow perception zone, or a
            //      cone of perception) in front of it, then it will continue to move forward but add
            //      a rotational component to incrementally turn toward the target,
            Move(); // incremental is done in UpdateKinematics
        }
        else
        {
            // ii   Else if the target is outside its arc, then it will stop moving and change direction
            //      on the spot before setting off once more. 
            if (KinematicModeEnabled)
            {
                KinematicSteeringOutput = new KinematicSteeringOutput();
                UpdateKinematics();
            }
            else
            {
                SteeringOutput = new SteeringOutput
                {
                    Linear = -CurrentVelocity.normalized * MaxAcceleration // Stops
                };
                UpdateSteeringKinematics();
            }
        }

        return;
    }

    private void HeuristicC()
    {
        // C    Regardless of the speed of the character:

        Vector3 direction = Target.transform.position - transform.position;
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance < SteppingRadius)
        {
            // i    If it is a very small distance from its target, it will step away directly, even if this
            //      involves moving backward or sidestepping,
            transform.position = new Vector3(
                Target.transform.position.x,
                transform.position.y,
                Target.transform.position.z);
            transform.position += -SteppingRadius * direction.normalized;
        }
        else
        {
            // ii   Else if the target is farther away, the character will first come to a stop, turn on
            //      the spot to face away from the target, and then move away. 

            float facingRotation = Quaternion.LookRotation(direction, Vector3.up).eulerAngles.y;

            if (AngleMapper.MapDegreesMidpointZero(Quaternion.LookRotation(transform.forward).eulerAngles.y - facingRotation + 180f) <= 0.01f)
            { // if we are faced away
                Move();
                return;
            }
            else
            {
                if (KinematicModeEnabled)
                {
                    KinematicSteeringOutput = new KinematicSteeringOutput
                    {
                        Rotation = AngleMapper.MapDegreesMidpointZero(facingRotation + 180f) // face away
                    };
                    UpdateKinematics();
                }
                else
                {
                    SteeringOutput = SteeringAlign.GetSteering(AngleMapper.MapDegreesMidpointZero(facingRotation + 180f));
                    SteeringOutput.Linear = -1 * direction.normalized * MaxAcceleration;
                    UpdateSteeringKinematics();
                }
            }
        }
    }

    private void KinematicMove()
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

        ClipMaxVelocities();
    }

    private void SteeringMove()
    {
        switch (SteeringBehaviourSelection)
        {
            case SteeringBehaviourSelection.SteeringWander:
                SteeringOutput = SteeringWander.GetSteering();
                break;
            case SteeringBehaviourSelection.SteeringSeek:
                SteeringOutput = SteeringSeek.GetSteering(Target.transform.position);
                break;
            case SteeringBehaviourSelection.SteeringFlee:
                SteeringOutput = SteeringFlee.GetSteering(Target.transform.position);
                break;
            case SteeringBehaviourSelection.SteeringArrive:
                SteeringOutput = SteeringArrive.GetSteering();
                break;
            case SteeringBehaviourSelection.SteeringPersue:
                SteeringOutput = SteeringPersue.GetSteering();
                break;
            case SteeringBehaviourSelection.SteeringEvade:
                SteeringOutput = SteeringEvade.GetSteering();
                break;
            default:
                break;
        }

        if (SteeringBehaviourSelection != SteeringBehaviourSelection.SteeringWander)
        {
            switch (OrientationBehaviourSelection)
            {
                case OrientationBehaviourSelection.None:
                    break;
                case OrientationBehaviourSelection.SteeringAlign:
                    SteeringOutput.Angular = SteeringAlign.GetSteering(Target.transform.rotation.eulerAngles.y).Angular;
                    break;
                case OrientationBehaviourSelection.SteeringFace:
                    SteeringOutput.Angular = SteeringFace.GetSteering(Target.transform.position).Angular;
                    break;
                case OrientationBehaviourSelection.SteeringFaceAway:
                    SteeringOutput.Angular = SteeringFace.GetSteering(Target.transform.position, true).Angular;
                    break;
                case OrientationBehaviourSelection.SteeringLookWhereYoureGoing:
                    SteeringOutput.Angular = SteeringLookWhereYoureGoing.GetSteering().Angular;
                    break;
            };
        }

        UpdateSteeringKinematics();
    }

    private void ClipMaxVelocities()
    {
        if (Mathf.Abs(CurrentVelocity.magnitude) > MaxSpeed)
            CurrentVelocity = CurrentVelocity.normalized * MaxSpeed;

        if (Mathf.Abs(CurrentAngularVelocity.magnitude) > MaxAngularSpeed)
            CurrentAngularVelocity = CurrentAngularVelocity.normalized * MaxAngularSpeed;
    }

    private void UpdateKinematics()
    {
        // Update position
        Vector3 newPosition = transform.position + CurrentVelocity * Time.deltaTime;
        Debug.DrawLine(transform.position, transform.position + 5f * CurrentVelocity.normalized);
        
        // Update orientation
        float targetRotation = AngleMapper.MapDegreesMidpointZero(KinematicSteeringOutput.Rotation);
        float currentRotation = AngleMapper.MapDegreesMidpointZero(transform.rotation.eulerAngles.y);
        float rotationDelta = targetRotation - currentRotation;
        Quaternion newRotation;
        if (KinematicBehaviourSelection == KinematicBehaviourSelection.KinematicWander)
            newRotation = Quaternion.Euler(0, currentRotation + rotationDelta, 0);
        else
        {
            if (TargetRotationDelta == rotationDelta)
                TargetRotationDeltaInterpolationT += Time.deltaTime;
            else
                TargetRotationDeltaInterpolationT = Time.deltaTime;

            float interpolatedRotationDelta = Mathf.Lerp(0, rotationDelta, 4 * TargetRotationDeltaInterpolationT);
            newRotation = Quaternion.Euler(0, currentRotation + interpolatedRotationDelta, 0);
        }
        transform.SetPositionAndRotation(newPosition, newRotation);

        // Update velocity
        CurrentVelocity = KinematicSteeringOutput.Velocity;
        CurrentAngularVelocity = Vector3.zero; // We do not update Angular velocity for KinematicMovement 
        CurrentSpeed = CurrentVelocity.magnitude;

        ClipMaxVelocities();
    }

    private void UpdateSteeringKinematics()
    {
        // Update position and orientation
        transform.position += CurrentVelocity * Time.deltaTime;
        transform.Rotate(new Vector3(0, (180f / Mathf.PI) * CurrentAngularVelocity.y * Time.deltaTime, 0));

        // Update velocity and angular velocity
        CurrentVelocity += SteeringOutput.Linear * Time.deltaTime;
        CurrentAngularVelocity += SteeringOutput.Angular * Vector3.up * Time.deltaTime;
        CurrentSpeed = CurrentVelocity.magnitude;

        ClipMaxVelocities();
    }

    public void SetTarget(GameObject target)
    {
        TargetGameObject = target;
        var targetAIBody = target.GetComponent<AIBody>();
        if (targetAIBody == null)
            Debug.LogWarning("SetTarget failed");
        Target = targetAIBody;
    }
}
