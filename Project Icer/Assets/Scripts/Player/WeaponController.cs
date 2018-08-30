using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceEvents.Weapons;

public enum WeaponStates
{
    Down,
    Aiming,
    Firing,
    Throwing
}
public class WeaponController : MonoBehaviour
{
    public static WeaponController STATIC_WeaponController;

    #region VARIABLES

    public WeaponStates CurrentWeaponState = WeaponStates.Down;

    public Weapon_BASE CurrentWeapon;
    public GameObject Prefab_CurrentBullet;

    public GameObject FiringTip;
    Animator Animator_FiringTip;
    public GameObject ShellEjector;
    public Animator Animator_WeaponModel;
    ParticleSystem ParticleSystem_FiringTip;

    // FiringLock
    bool Firing = false;
    IEnumerator FiringLockCoroutine;

    // Audio
    [HideInInspector] public AudioSource COMP_AudioSource;

    #endregion VARIABLES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_INITIALIZATION

    void Awake()
    {
        if (STATIC_WeaponController == null)
            STATIC_WeaponController = this;
        else
            Destroy(this);


        Animator_FiringTip = FiringTip.GetComponent<Animator>();
        ParticleSystem_FiringTip = FiringTip.GetComponentInChildren<ParticleSystem>();
        COMP_AudioSource = GetComponent<AudioSource>();

        EventMGR.STATIC_EventMGR.SubscribeToEvent(typeof(Ice_SetFiringLock), OnSetFiringLock);

        InitializeCurrentWeapon();
    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_UPDATE

    void Update()
    {
        CheckWeaponInput();
    }

    #endregion METHODS_UPDATE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFICS

    private void InitializeCurrentWeapon()
    {
        Prefab_CurrentBullet = CurrentWeapon.Reserve.Prefab_Bullet;
    }

    public void LockFiring(float lockingtime)
    {
        if (FiringLockCoroutine != null)
            StopCoroutine(FiringLockCoroutine);

        FiringLockCoroutine = LockFiring_Coroutine(lockingtime);
        StartCoroutine(FiringLockCoroutine);
    }

    #endregion METHODS_SPECIFICS

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_WEAPONSTATES

    private void CheckWeaponInput()
    {
        switch (CurrentWeaponState)
        {
            case WeaponStates.Down:
                {
                    CheckDownState();
                    break;
                }
            case WeaponStates.Aiming:
                {
                    CheckAimingState();
                    break;
                }
            case WeaponStates.Firing:
                {
                    if (!Firing)
                        CheckFiringState();

                    break;
                }
            case WeaponStates.Throwing:
                {
                    if (!Firing)
                        ResolveThrowingState();

                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void CheckDownState()
    {
        //Aim with Right Click
        if (Input.GetMouseButton(1))
        {
            ResolveAiming();
            Animator_WeaponModel.SetTrigger("Raise");
        }
    }
    private void ResolveDown()
    {
        CurrentWeaponState = WeaponStates.Down;
        EventMGR.STATIC_EventMGR.DispatchEvent(typeof(Ice_DownWeaponState), new Ice_DownWeaponState());

        Animator_WeaponModel.SetTrigger("Lower");
    }

    private void CheckAimingState()
    {
        CurrentWeaponState = WeaponStates.Aiming;

        //Aim with Right Click
        if (!Input.GetMouseButton(1))
        {
            ResolveDown();
        }
        else if (Input.GetMouseButton(0))
        {
            if (!Firing)
                ResolveFiring();
        }
    }
    private void ResolveAiming()
    {
        CurrentWeaponState = WeaponStates.Aiming;
        EventMGR.STATIC_EventMGR.DispatchEvent(typeof(Ice_AimingWeaponState), new Ice_AimingWeaponState());


    }

    private void CheckFiringState()
    {
        //Aim with Right Click
        if (Input.GetMouseButton(0))
        {
            if (Firing)
                return;

            ResolveFiring();
        }
        else if (Input.GetMouseButton(1))
        {
            if (!Firing)
                ResolveAiming();
        }
        else
        {
            ResolveDown();
        }
    }
    private void ResolveFiring()
    {
        CurrentWeaponState = WeaponStates.Firing;

        if (!CurrentWeapon.Fire())
            return;

        Animator_FiringTip.SetTrigger("Fire");
        ParticleSystem_FiringTip.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        ParticleSystem_FiringTip.Play();

        //COMP_AudioSource.Stop();
        //COMP_AudioSource.PlayOneShot(COMP_AudioSource.clip);
        AudioMGR.STATIC_AudioMGR.PlayNMG();

        EventMGR.STATIC_EventMGR.DispatchEvent(typeof(Ice_FiringWeaponState), new Ice_FiringWeaponState());

        Vector3 dir = FiringTip.transform.forward;

#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(FiringTip.transform.position, FiringTip.transform.position + dir, Color.black);
#endif

        float angle = CurrentWeapon.Reserve.Inaccuracy;
        dir = Quaternion.AngleAxis(Random.Range(-angle, angle), FiringTip.transform.up) * dir;
        dir = Quaternion.AngleAxis(Random.Range(-angle, angle), FiringTip.transform.right) * dir;

        print("dir: " + dir);
        GameObject bullet = Instantiate(Prefab_CurrentBullet, FiringTip.transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet_BASE>().InitializeDirection(dir);

        GameObject shell = Instantiate(CurrentWeapon.Reserve.Prefab_Shell, ShellEjector.transform.position, FiringTip.transform.rotation);
        shell.GetComponent<Rigidbody>().AddForce(ShellEjector.transform.forward * Random.Range(2, 5), ForceMode.Impulse);

        Animator_WeaponModel.SetTrigger("Fire");
    }


    private void CheckForThrowingInput()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    ResolveThrowingState();
        //}
    }
    private void ResolveThrowingState()
    {
        //CheckForThrowingInput();

        CurrentWeaponState = WeaponStates.Throwing;
    }


    public bool IsWeaponDown()
    {
        return CurrentWeaponState == WeaponStates.Down ? true : false;
    }
    public bool IsWeaponAiming()
    {
        return CurrentWeaponState == WeaponStates.Aiming ? true : false;
    }
    public bool IsWeaponFiring()
    {
        return CurrentWeaponState == WeaponStates.Firing ? true : false;
    }

    #endregion METHODS_WEAPONSTATES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_EVENTS

    private void OnSetFiringLock(IceEvent_BASE i)
    {
        Ice_SetFiringLock sfl = (Ice_SetFiringLock)i;

        LockFiring(sfl.LockTime);
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

    IEnumerator LockFiring_Coroutine(float locktime)
    {
        Firing = true;
        yield return new WaitForSeconds(locktime);
        Firing = false;
    }

    #endregion METHODS_COROUTINES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region GIZMOS

    //OnGizmo - private void OnDrawGizmos() { }

    #endregion GIZMOS
}
