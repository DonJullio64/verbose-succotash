using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_BASE : MonoBehaviour
{
    #region VARIABLES

    public float Speed = 10f;
    public Vector3 Direction;
    public LayerMask LayerMask_BulletCollision;

    public float DamageOutput = 1;

    public GameObject Prefab_BulletHitParticles;
    public AudioClip AudioClip_DefaultCollision;
    public AudioClip AudioClip_EnemyCollision;

    #endregion VARIABLES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_INITIALIZATION

    void Awake()
    {

    }

    public void InitializeDirection(Vector3 dir)
    {
        Direction = Vector3.Normalize(dir);
    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_UPDATE

    void Update()
    {
        ResolveBulletMovement();
    }

    #endregion METHODS_UPDATE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFICS

    protected virtual void ResolveBulletMovement()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, Direction);
        if (Physics.Raycast(
            ray,
            out hitInfo,
            (Direction * Speed * Time.deltaTime).magnitude,
            LayerMask_BulletCollision,
            QueryTriggerInteraction.UseGlobal)
            )
        {
            transform.position = hitInfo.point;
            ResolveCollision(hitInfo.transform);
        }
        else
            transform.Translate(Direction * Speed * Time.deltaTime);
    }

    protected virtual void ResolveCollision(Transform othertrans)
    {
        HurtBox hurtbox = othertrans.GetComponent<HurtBox>();
        if (hurtbox)
        {
            hurtbox.DamageHurtBox(DamageOutput);
            AudioMGR.STATIC_AudioMGR.PlayGlobalSFXOneShot(AudioClip_EnemyCollision, 0.1f);
        }

        GameObject hitparticles = Instantiate(Prefab_BulletHitParticles, transform.position, Quaternion.identity);
        hitparticles.GetComponent<AudioSource>().PlayOneShot(AudioClip_DefaultCollision, 0.75f);
        Destroy(gameObject);
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

    private void OnCollisionEnter(Collision collision)
    {
        ResolveCollision(collision.transform);
    }
    private void OnTriggerEnter(Collider collider)
    {
        ResolveCollision(collider.transform);
    }

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
