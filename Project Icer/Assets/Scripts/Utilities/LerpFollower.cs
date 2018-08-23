using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpFollower : MonoBehaviour
{
    public GameObject ObjectToFollow;
    public float LerpPercentage = 1.0f;
    public Vector3 FollowOffset = Vector3.zero;

	// Use this for initialization
	private void Start ()
    {
		
	}
	
	// Update is called once per frame
	private void LateUpdate ()
    {
        if (ObjectToFollow)
        {
            if (LerpPercentage >= 1f)
                transform.position = ObjectToFollow.transform.position + FollowOffset;
            else
                transform.position = Vector3.Lerp(transform.position, ObjectToFollow.transform.position + FollowOffset, LerpPercentage);

        }
	}
}
