using System.Collections;
using UnityEngine;

public class TimedDeath : MonoBehaviour
{
    public bool StartCountdownOnSpawn;
    public float TimeToDeath = 1f;

    //private IEnumerator Coroutine;


    void Start ()
    {
        //Coroutine = CountdownToDeath();
        if (StartCountdownOnSpawn)
            StartDeathCountdown();
	}
	
	public void StartDeathCountdown()
    {
        StartCoroutine("CountdownToDeath");
    }

    IEnumerator CountdownToDeath()
    {
        yield return new WaitForSeconds(TimeToDeath);
        Destroy(gameObject);
    }
}
