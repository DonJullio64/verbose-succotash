using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController STATIC_PlayerController;

    #region VARIABLES

    // MOVEMENT
    public AnimationCurveProgresser CurveProg_MovementCurve;
    public float MoveSpeed = 10f;
    public LayerMask LayerMask_MovementChecking;
    public bool Grounded = false;
    public float GroundStepLength = 0.5f;
    public float OuterSkinThickness = 0.1f;

    public float MaxWalkableAngle = 45;
    private Vector3 GroundNormal;

    public float TargetVelocity;
    [HideInInspector] public Vector3 TargetPosition;
    //[HideInInspector] public Vector3 LastMovementDirection;

    public float Gravity = 9.8f;

    private CapsuleCollider COMP_CapsuleCollider;

    #endregion VARIABLES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_INITIALIZATION

    void Awake()
    {
        if (STATIC_PlayerController == null)
            STATIC_PlayerController = this;
        else
            Destroy(this);

        COMP_CapsuleCollider = GetComponent<CapsuleCollider>();
    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_UPDATE

    void FixedUpdate()
    {
        // Get movement
        Vector3 move = GetRawMovementInput();
        move = ApplyCameraDirectionalOffsetToMovement(move);
        CastMovementAhead(move);


        move = Vector3.ProjectOnPlane(move, GroundNormal);
    }

    #endregion METHODS_UPDATE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFICS

    private Vector3 GetRawMovementInput()
    {
        bool hasmoved = false;
        Vector3 input = new Vector3(0, 0, 0);

        // Forward/Back
        if (Input.GetKey(KeyCode.W))
        {
            input.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            input.z -= 1;
        }
        // Left/Right
        if (Input.GetKey(KeyCode.A))
        {
            input.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            input.x += 1;
        }

        // Check if movement input is recieved and progress movement Velocity as needed
        if (input != Vector3.zero)
            hasmoved = true;

        if (hasmoved)
            TargetVelocity = CurveProg_MovementCurve.ProgressThisCurve(true) * MoveSpeed;
        else
            TargetVelocity = CurveProg_MovementCurve.ProgressThisCurve(false) * MoveSpeed;

        return input;
    }

    private Vector3 ApplyCameraDirectionalOffsetToMovement(Vector3 move)
    {
        move = Vector3.Normalize(
            CameraController.STATIC_CameraController.transform.forward * move.z +
            CameraController.STATIC_CameraController.transform.right * move.x);

#if UNITY_EDITOR
        // Show ground check ray in editor
        Debug.DrawLine(transform.position, transform.position + (move * TargetVelocity), Color.white);
        Debug.DrawLine(transform.position, transform.position + (move * TargetVelocity * Time.deltaTime), Color.blue);
#endif

        move = Vector3.Scale(move, new Vector3(1, 0, 1));
        return Vector3.Normalize(move);
    }

    private void CastMovementAhead(Vector3 move)
    {
        RaycastHit hitInfo;
        Vector3 newpos = transform.position + (move * TargetVelocity * Time.deltaTime);


        // Test cast in movement direction
        if (Physics.CapsuleCast(
            transform.position + Vector3.up * (COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius),
            transform.position - Vector3.up * (COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius - GroundStepLength),
            COMP_CapsuleCollider.radius,// + OuterSkinThickness,
            move,
            out hitInfo,
            TargetVelocity * Time.deltaTime,// + OuterSkinThickness,
            LayerMask_MovementChecking,
            QueryTriggerInteraction.UseGlobal))
        {
            print("Forward Hit Detected");
            //newpos = transform.position + (move * hitInfo.distance);
            newpos = transform.position;

            // If falling, 
            if (!Grounded)
                newpos.y -= Time.deltaTime * Gravity;

            // Test cast down from newpos
            if (Physics.Raycast(
                newpos,
                Vector3.down,
                out hitInfo,
                COMP_CapsuleCollider.height * 0.5f + newpos.y,// + MaxDownDistance,
                LayerMask_MovementChecking,
                QueryTriggerInteraction.UseGlobal))
            {
                //newpos.y = hitInfo.point.y;
                //hitInfo.normal
                Grounded = true;

                print("Hitting Ground");
            }
            else
            {
                Grounded = false;
                print("Falling");
            }


        }
        else
        {
            print("Nothing Ahead.");
            // If falling, 
            if (!Grounded)
                newpos.y -= Time.deltaTime * Gravity;

            // Test cast down from newpos
            if (Physics.CapsuleCast(
                newpos + Vector3.up * ((COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius) + GroundStepLength),
                newpos - Vector3.up * ((COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius) - GroundStepLength),
                COMP_CapsuleCollider.radius,
                Vector3.down,
                out hitInfo,
                COMP_CapsuleCollider.height + GroundStepLength + newpos.y,
                LayerMask_MovementChecking,
                QueryTriggerInteraction.UseGlobal))
                //Physics.Raycast(
                //newpos,
                //Vector3.down,
                //out hitInfo,
                //COMP_CapsuleCollider.height * 0.5f + newpos.y,// + MaxDownDistance,
                //LayerMask_MovementChecking,
                //QueryTriggerInteraction.UseGlobal))
            {
                print("Casting Down Only.");
                if (!Grounded)
                {
                    newpos = newpos + (Vector3.down * hitInfo.distance);
                    //newpos.y = hitInfo.point.y;
                    Grounded = true;
                }

                print("Hitting Ground");
            }
            else
            {
                Grounded = false;
                print("Falling");
            }
        }

#if UNITY_EDITOR
        // Show ground check ray in editor
        Debug.DrawLine(transform.position, newpos + new Vector3(0, -COMP_CapsuleCollider.height * 0.5f, 0), Color.white);
        Debug.DrawLine(transform.position, transform.position + Vector3.up * (COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius), Color.grey);
        Debug.DrawLine(transform.position, transform.position - Vector3.up * (COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius), Color.grey);
#endif

        transform.position = newpos;
    }

    /*
    // Check for grounded state; Sets "Grounded" variable
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;

#if UNITY_EDITOR
        // Show ground check ray in editor
        Debug.DrawLine(transform.position + (Vector3.up * RayUpOffset), transform.position + (Vector3.up * RayUpOffset) + (Vector3.down * (GroundCheckDistance + RayUpOffset)));
#endif

        // The transform position is in the center of the character
        if (Physics.Raycast(transform.position + (Vector3.up * RayUpOffset), Vector3.down, out hitInfo, GroundCheckDistance + RayUpOffset, LayerMask_GroundChecking, QueryTriggerInteraction.Ignore))
        {
            GroundNormal = hitInfo.normal;
            //PrintDebug("GroundNormal in CheckGroundStatus: " + GroundNormal);
            Grounded = true;

            //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hitInfo.point.y + 1, transform.position.z), 0.2f);

            /// FLOOR SNAPPING ATTEMPT
            //Vector3 snapY = transform.position;
            //snapY.y = hitInfo.point.y + 1;
            //transform.position = Vector3.Lerp(transform.position, snapY, .1f);

            //Vector3 stoppoint = ControllerMGR.transform.position;
            //stoppoint.y = hitInfo.point.y + 1f;
            //ControllerMGR.transform.position = stoppoint;

            //PrintDebug("hitInfo.point + new Vector3(0f,0.01f,0f): " + hitInfo.point + new Vector3(0f, 0.01f, 0f));
            //PrintDebug("Grounded: " + Grounded);
        }
        else
        {
            GroundNormal = Vector3.up;
            Grounded = false;
        }
    }
    */

    /*
    RaycastHit hitInfo;
        Vector3 newpos = Vector3.Scale(new Vector3(1, 0, 1), transform.position) + (move * TargetVelocity * Time.deltaTime);

        print("Forward Distance: " + TargetVelocity * Time.deltaTime);
        // Test cast in movement direction
        if (Physics.CapsuleCast(
            transform.position + Vector3.up * ((COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius) + GroundStepLength),
            COMP_CapsuleCollider.radius,// + OuterSkinThickness,
            move,
            out hitInfo,
            TargetVelocity * Time.deltaTime,// + OuterSkinThickness,
            LayerMask_MovementChecking,
            QueryTriggerInteraction.UseGlobal))
        {
            print("Forward Hit Detected");
            //newpos = transform.position + (move * hitInfo.distance);
            newpos = transform.position;

            // If falling, 
            {
                print("Applying Gravity");
                newpos.y -= Time.deltaTime * Gravity;
            }

            print("Down Distance1: " + (COMP_CapsuleCollider.height * 0.5f + newpos.y));

            // Test cast down from newpos
            if (Physics.Raycast(
                newpos,
                Vector3.down,
                out hitInfo,
                COMP_CapsuleCollider.height * 0.5f + newpos.y,// + MaxDownDistance,
                LayerMask_MovementChecking,
                QueryTriggerInteraction.UseGlobal))
            {
                //newpos.y = hitInfo.point.y;
                //hitInfo.normal
                Grounded = true;

                print("Hitting Ground");
            }
            else
            {
                Grounded = false;
                print("Falling");
            }


        }
        else
        {
            print("Nothing Ahead.");
            // If falling, 
            if (!Grounded)
            {
                print("Applying Gravity");
                newpos.y -= Time.deltaTime * Gravity;
            }

            print("Down Distance2: " + (COMP_CapsuleCollider.height * 0.5f + newpos.y));
            // Test cast down from newpos
            if (Physics.CapsuleCast(
                newpos + Vector3.up * (COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius),
                newpos - Vector3.up * (COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius - GroundStepLength),
                COMP_CapsuleCollider.radius,
                Vector3.down,
                out hitInfo,
                COMP_CapsuleCollider.height * 0.5f + newpos.y,
                LayerMask_MovementChecking,
                QueryTriggerInteraction.UseGlobal))
                //Physics.Raycast(
                //newpos,
                //Vector3.down,
                //out hitInfo,
                //COMP_CapsuleCollider.height * 0.5f + newpos.y,// + MaxDownDistance,
                //LayerMask_MovementChecking,
                //QueryTriggerInteraction.UseGlobal))
            {
                print("Casting Down Only.");
                if (Grounded)
                    return;
                newpos = newpos + (Vector3.down * hitInfo.distance);
                //newpos.y = hitInfo.point.y;
                Grounded = true;

                print("Hitting Ground");
            }
            else
            {
                Grounded = false;
                print("Falling");
            }
        }

#if UNITY_EDITOR
        // Show ground check ray in editor
        Debug.DrawLine(transform.position, newpos + new Vector3(0, -COMP_CapsuleCollider.height * 0.5f, 0), Color.white);
        Debug.DrawLine(transform.position, transform.position + Vector3.up * (COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius), Color.grey);
        Debug.DrawLine(transform.position, transform.position - Vector3.up * (COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius), Color.grey);
#endif

        transform.position = newpos;
    */

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
    private void OnDrawGizmos()
    {
        if (COMP_CapsuleCollider == null)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position , COMP_CapsuleCollider.radius * 2);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * (COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius), COMP_CapsuleCollider.radius);
        Gizmos.DrawWireSphere(transform.position - Vector3.up * (COMP_CapsuleCollider.height * 0.5f - COMP_CapsuleCollider.radius), COMP_CapsuleCollider.radius);
    }

    #endregion GIZMOS
}
