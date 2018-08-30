using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotJoint : MonoBehaviour
{
    #region VARIABLES

    public GameObject TargetObject;
    public Vector3 Target;

    public Vector3 Axis;
    public Vector3 StartOffset;

    public RobotJoint[] Joints;
    public float[] Angles;

    public float SamplingDistance = 0.1f;
    public float LearningRate = 0.5f;
    public float DistanceThreshold = 0.5f;

    public bool IsEnd = false;

    #endregion VARIABLES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_INITIALIZATION

    void Awake()
    {
        Joints = GetComponentsInParent<RobotJoint>();
        Angles = new float[Joints.Length];
        StartOffset = transform.localPosition;
    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_UPDATE

    private void Update()
    {
        Target = TargetObject.transform.position;

        for (int i = 0; i < Angles.Length; i++)
        {
            if (IsEnd)
                print("Vector3.Dot(Joints[i].transform.localEulerAngles, Joints[i].Axis): " + Vector3.Dot(Joints[i].transform.localEulerAngles, Joints[i].Axis) + " where i is: " + i);
            Angles[i] = Vector3.Dot(Joints[i].transform.localEulerAngles, Joints[i].Axis);
        }

        Angles = InverseKinematics(Target, Angles);

        if (IsEnd)
        {
            for (int i = 0; i < Angles.Length; i++)
            {
                Joints[i].transform.localRotation = Quaternion.Euler(Joints[i].Axis * Angles[i]);
            }
        }
    }

    #endregion METHODS_UPDATE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFICS

    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }

    public Vector3 ForwardKinematics(float[] angles)
    {
        Vector3 prevpoint = Joints[0].transform.position;
        Quaternion rotation = Quaternion.identity;

        for (int i = 1; i < Joints.Length; i++)
        {
            // The rotation of this joint is the sum of it's parents' rootations along their axes
            rotation *= Quaternion.AngleAxis(angles[i - 1], Joints[i - 1].Axis);
            Vector3 nextpoint = prevpoint + rotation * Joints[i].StartOffset;  // (rotation * joints[i].StartOffset) is the fully rotated vector for the next joint

            prevpoint = nextpoint;
        }

        return prevpoint;
    }

    public float PartialGradient(Vector3 target, float[] angles, int i)
    {
        // Saves the angle,
        // it will be restored later
        float angle = angles[i];

        // Gradient : [F(x+SamplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget(target, angles);

        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);

        float gradient = (f_x_plus_d - f_x) / SamplingDistance;

        // Restores
        angles[i] = angle;

        return gradient;
    }

    public float[] InverseKinematics(Vector3 target, float[] angles)
    {
        for (int i = 0; i < Joints.Length; i++)
        {
            // Gradient descent
            // Update : Solution -= LearningRate * Gradient
            float gradient = PartialGradient(target, angles, i);
            angles[i] -= LearningRate * gradient;
        }

        return angles;
    }

    #endregion METHODS_SPECIFICS

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_EVENTS



    #endregion METHODS_EVENTS

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_COLLISION



    #endregion METHODS_COLLISION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_COROUTINES



    #endregion METHODS_COROUTINES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region GIZMOS

    //OnGizmo - private void OnDrawGizmos() { }

    #endregion GIZMOS
}
