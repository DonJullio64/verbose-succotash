using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceEvents.Weapons;

public class CameraController : MonoBehaviour
{
    public static CameraController STATIC_CameraController;

    [System.Serializable]
    public struct CameraSettingsBlock
    {
        public float FOV;
        public Vector3 CameraZOffset;

        public CameraSettingsBlock(float fov, Vector3 camzoffset)
        {
            FOV = fov;
            CameraZOffset = camzoffset;
        }
    }

    #region VARIABLES


    // Inversion //
    public bool InvertX = false;
    public bool InvertY = false;

    // Pivot X Bounds //
    public float PivotPositiveXBound = 30;
    public float PivotNegativeXBound = -30;

    // Rotation Slerp Percent //
    public float SlerpPercent = 0.2f;

    // Cursor Locking //
    public KeyCode CursorUnlockKey = KeyCode.Escape;
    public bool m_CursorIsLocked = true;
    // Sensitivity //
    [HideInInspector] public float XSensitivity = 3.0f;
    [HideInInspector] public float YSensitivity = 2.0f;
    [HideInInspector] public float XSensitivity_Aiming = 1.0f;
    [HideInInspector] public float YSensitivity_Aiming = 1.0f;

    // Clamps //
    public float XMax = 5f;
    public float YMax = 2f;
    public float XMax_Aiming = 5f;
    public float YMax_Aiming = 2f;
    public Quaternion TargetRotation = Quaternion.identity;    // Used to track the angle the camera pivot should Slerp towards


    public GameObject CenterPivot;
    public GameObject ArmPivot;
    public GameObject CameraPivot;

    public LayerMask LayerMask_CamAvoidance;
    public float ArmPivotMaxXOffset;
    public float ArmAvoidanceBuffer = 0.1f;
    public float CameraMaxZOffset;
    public float CameraAvoidanceBuffer = 0.1f;

    public CameraSettingsBlock csb_Base;
    public CameraSettingsBlock csb_Aiming;
    CameraSettingsBlock csb_Current;
    [Range(0f, 1f)] public float LerpPercent_CamSettings = 0.2f;

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

        EventMGR.STATIC_EventMGR.SubscribeToEvent(typeof(Ice_DownWeaponState), OnDownWeaponState);
        EventMGR.STATIC_EventMGR.SubscribeToEvent(typeof(Ice_AimingWeaponState), OnAimingWeaponState);

        csb_Current = csb_Base;

        InitializeSensitivities();
    }

    private void InitializeSensitivities()
    {
        // Check player prefs for mouse sensitivity.
        // Make one in player prefs if it doesn't exist
        // XSENSITIVITY
        if (PlayerPrefs.HasKey("MouseXSensitivity"))
        {
            //PrintDebug(this.ToString() + " *$$$* 'MouseXSensitivity' found in PlayerPrefs");

            XSensitivity = PlayerPrefs.GetFloat("MouseXSensitivity");
        }
        else
        {
            //  PrintDebug(this.ToString() + " *@@@* 'MouseXSensitivity' NOT found in PlayerPrefs");

            PlayerPrefs.SetFloat("MouseXSensitivity", 2.0f);
            XSensitivity = 2.0f;
        }
        // YSENSITIVITY
        if (PlayerPrefs.HasKey("MouseYSensitivity"))
        {
            //  PrintDebug(this.ToString() + " *$$$* 'MouseYSensitivity' found in PlayerPrefs");

            YSensitivity = PlayerPrefs.GetFloat("MouseYSensitivity");
        }
        else
        {
            // PrintDebug(this.ToString() + " *@@@* 'MouseYSensitivity' NOT found in PlayerPrefs");

            PlayerPrefs.SetFloat("MouseYSensitivity", 2.0f);
            YSensitivity = 2.0f;
        }

        // XSENSITIVITY_Aiming
        if (PlayerPrefs.HasKey("MouseXSensitivity_Aiming"))
        {
            XSensitivity_Aiming = PlayerPrefs.GetFloat("MouseXSensitivity_Aiming");
        }
        else
        {
            PlayerPrefs.SetFloat("MouseXSensitivity_Aiming", 1.0f);
            XSensitivity_Aiming = 1.0f;
        }
        // YSENSITIVITY
        if (PlayerPrefs.HasKey("MouseYSensitivity_Aiming"))
        {
            YSensitivity_Aiming = PlayerPrefs.GetFloat("MouseYSensitivity_Aiming");
        }
        else
        {
            PlayerPrefs.SetFloat("MouseYSensitivity_Aiming", 1.0f);
            YSensitivity_Aiming = 1.0f;
        }
    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_UPDATE

    void Update()
    {
        UpdateCameraRotation();

        CheckCameraBackAvoidance();
        ResolveCameraSettings();

        UpdateCursorLock();
    }

    #endregion METHODS_UPDATE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFICS

    private void UpdateCameraRotation()
    {
        // Mouse Input
        float yRot, xRot;

        if (WeaponController.STATIC_WeaponController.IsWeaponAiming())
        {
            yRot = Mathf.Clamp(Input.GetAxis("Mouse X"), -XMax_Aiming, XMax_Aiming) * XSensitivity_Aiming;
            xRot = Mathf.Clamp(Input.GetAxis("Mouse Y"), -YMax_Aiming, YMax_Aiming) * YSensitivity_Aiming;
        }
        else
        {
            yRot = Mathf.Clamp(Input.GetAxis("Mouse X"), -XMax, XMax) * XSensitivity;
            xRot = Mathf.Clamp(Input.GetAxis("Mouse Y"), -YMax, YMax) * YSensitivity;
        }

        // Inverted Axes
        if (InvertX) yRot = -yRot;
        if (InvertY) xRot = -xRot;


        Vector3 eularTarget = TargetRotation.eulerAngles;

        // Y-movement on the mouse translates to turning the camera pivot up/down (rolling along pivot's x axis)
        // X-movement on the mouse translates to turning the camera pivot left/right (rolling along pivot's y axis)
        //eularTarget += new Vector3(mouseposOffset.y, -mouseposOffset.x, 0);
        eularTarget += new Vector3(-xRot, yRot, 0);

        // Clamp and reset the  rotation 
        if (eularTarget.y < -180.0f)
            eularTarget.y = 180 - (eularTarget.y - 180);
        else if (eularTarget.y > 180.0f)
            eularTarget.y = -180 + (eularTarget.y - 180);

        if (eularTarget.x < -180.0f)
            eularTarget.x = 180 - (eularTarget.x - 180);
        else if (eularTarget.x > 180.0f)
            eularTarget.x = -180 + (eularTarget.x - 180);

        // Clamp the x angle, so camera doesn't roll upside down
        eularTarget.x = Mathf.Clamp(eularTarget.x, PivotNegativeXBound, PivotPositiveXBound);

        TargetRotation = Quaternion.Euler(eularTarget.x, eularTarget.y, 0);

        transform.rotation = TargetRotation;
    }

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
            //print("Cam Avoidance Fine.");
            CameraPivot.transform.position = ArmPivot.transform.position + (ArmPivot.transform.forward * CameraMaxZOffset);
        }

        //print("ArmPivot.transform.position: " + ArmPivot.transform.position);
    }

    private void ResolveCameraSettings()
    {
        MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, csb_Current.FOV, LerpPercent_CamSettings);
        //CameraPivot.transform.position
    }

    public void UpdateCursorLock()
    {
        // Lock Check
        if (m_CursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_CursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    #endregion METHODS_SPECIFICS

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_EVENTS

    private void OnDownWeaponState(IceEvent_BASE i)
    {
        csb_Current = csb_Base;
    }
    private void OnAimingWeaponState(IceEvent_BASE i)
    {
        csb_Current = csb_Aiming;
    }


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
