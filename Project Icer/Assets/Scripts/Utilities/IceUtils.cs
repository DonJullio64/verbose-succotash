using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IceUtils
{
    
}


// ComponentTrackers
//public class CapsuleColliderTracker : MonoBehaviour
//{
//    public float Radius;
//    public float TopSphereOffset;

//    public CapsuleCollider COMP_CapsuleCollider;
//    public Rigidbody COMP_Rigibody;

//    private void Awake()
//    {
//        COMP_CapsuleCollider = GetComponent<CapsuleCollider>();
//        COMP_Rigibody = GetComponent<Rigidbody>();
//    }
//}

// Helper Structs
[System.Serializable]
public struct AnimationCurveProgresser
{
    public AnimationCurve AniCurve;
    public float Progression;
    public float ProgressionScalar;

    public float RecessionScalar;
    public float ProgressionMax;
    public bool InstantRecession;

    // Constructor
    public AnimationCurveProgresser(AnimationCurve anicurve, float progscalar, float regscalar, float progmax = 1, bool instantrecession = true)
    {
        AniCurve = anicurve;
        Progression = 0;
        ProgressionScalar = 1;
        RecessionScalar = 100;
        ProgressionMax = progmax;
        InstantRecession = instantrecession;
    }


    public float ProgressThisCurve(bool progressforward)
    {
        if (progressforward)
            ProgressForward();
        else
            RecessBackward();

        Progression =  Mathf.Clamp(Progression, 0, ProgressionMax);

        return AniCurve.Evaluate(Progression);
    }


    // Multiply Progression by DT for progression, not for velocity updated this frame
    private void ProgressForward()
    {
        Progression += Time.deltaTime * ProgressionScalar;
    }
    private void RecessBackward()
    {
        Progression -= Time.deltaTime * RecessionScalar;
    }
}
