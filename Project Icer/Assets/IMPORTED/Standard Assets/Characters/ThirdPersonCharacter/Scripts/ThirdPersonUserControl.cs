using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


//[RequireComponent(typeof (ThirdPersonCharacter))]
[RequireComponent(typeof(ThirdPersonCharacter))]
public class ThirdPersonUserControl : MonoBehaviour
{
    private ThirdPersonCharacter COMP_Character; // A reference to the ThirdPersonCharacter on the object
    private Transform MainCamera;                  // A reference to the main camera in the scenes transform
    private Vector3 Dir_CamForward;             // The current forward direction of the camera
    private Vector3 MoveDir;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        
    private void Start()
    {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            MainCamera = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )
        COMP_Character = GetComponent<ThirdPersonCharacter>();
    }


    private void Update()
    {
        if (!m_Jump)
        {
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        // read inputs
        float horiz = CrossPlatformInputManager.GetAxis("Horizontal");
        float vert = CrossPlatformInputManager.GetAxis("Vertical");
        bool crouch = Input.GetKey(KeyCode.C);

        // calculate move direction to pass to character
        if (MainCamera != null)
        {
            // calculate camera relative direction to move:
            Dir_CamForward = Vector3.Scale(MainCamera.forward, new Vector3(1, 0, 1)).normalized;
            MoveDir = vert*Dir_CamForward + horiz*MainCamera.right;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            MoveDir = vert*Vector3.forward + horiz*Vector3.right;
        }
#if !MOBILE_INPUT
		// walk speed multiplier
	    if (!Input.GetKey(KeyCode.LeftShift)) MoveDir *= 0.5f;
#endif

        // pass all parameters to the character control script
        COMP_Character.Move(MoveDir, crouch, m_Jump);
        m_Jump = false;
    }
}

