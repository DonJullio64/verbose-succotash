using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController STATIC_CameraController;

    #region VARIABLES

    public GameObject CenterPivot;
    public GameObject ArmPivot;
    public GameObject CameraPivot;

    public LayerMask LayerMask_CamAvoidance;
    public float ArmPivotMaxXOffset;
    public float ArmAvoidanceBuffer = 0.1f;
    public float CameraMaxZOffset;
    public float CameraAvoidanceBuffer = 0.1f;

    [HideInInspector] public Camera MainCamera;

    #endregion VARIABLES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_INITIALIZATION

    void Awake()
    {
        if (STATIC_CameraController == null)
            STATIC_CameraController = this;
        else
            Destroy(this);

        MainCamera = GetComponentInChildren<Camera>();
    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_UPDATE

    void Update()
    {
        CheckCameraBackAvoidance();
    }

    #endregion METHODS_UPDATE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFICS

    public void CheckCameraBackAvoidance()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(ArmPivot.transform.position, ArmPivot.transform.position + (ArmPivot.transform.forward * CameraMaxZOffset), Color.cyan);
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(ArmPivot.transform.position, ArmPivot.transform.forward, out hitInfo, CameraMaxZOffset, LayerMask_CamAvoidance, QueryTriggerInteraction.UseGlobal))
        {
            print("Cam Avoidance Collided with something!");
            CameraPivot.transform.position = ArmPivot.transform.position + (ArmPivot.transform.forward * (hitInfo.distance - CameraAvoidanceBuffer));
        }
        else
        {
            print("Cam Avoidance Fine.");
            CameraPivot.transform.position = ArmPivot.transform.position + (ArmPivot.transform.forward * CameraMaxZOffset);
        }

        print("ArmPivot.transform.position: " + ArmPivot.transform.position);
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

    //OnGizmo

    #endregion GIZMOS
}
