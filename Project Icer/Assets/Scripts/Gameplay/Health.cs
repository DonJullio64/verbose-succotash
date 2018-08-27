using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region VARIABLES

    public bool Invincible = false;
    public float MaxHealth = 10;
    public float CurrentHealth = -1;
    public float InitialHealth = 10;

    public float DangerHealthThreshold = 2.0f;
    public float FlinchCooldown = 3.0f;
    public bool IsCooling { get; private set; }

    float CooldownTimer = 0;

    [HideInInspector] public delegate void DeathFunction();
    DeathFunction deathFunction;
    ArrayList DeathFunctionArrayList = new ArrayList();    // declaration

    #endregion VARIABLES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_UPDATE

    private void Update()
    {
        if (IsCooling)
        {
            if (CooldownTimer >= FlinchCooldown)
                IsCooling = false;
            else
                CooldownTimer += Time.deltaTime;
        }
    }

    #endregion METHODS_UPDATE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_INITIALIZATION

    // Use this for initialization
    protected virtual void Awake ()
    {
        //RestoreHealth();
        SetHealth(InitialHealth);

    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFIC

    // Increases Max Health
    public void IncreaseMaxHealth(int increase, bool regen = true)
    {
        MaxHealth += increase;

        if (regen)
            RestoreHealth();
    }

    // Completely Restores health to MaxHealth
    public void RestoreHealth()
    {
        CurrentHealth = MaxHealth;
    }

    // Gain a given amount of health
    public void GainHealth(float gain)
    {
        //print("Entity gaining health: " + name);
        CurrentHealth += gain;
        ClampHealth();
    }

    // Lose all Health
    public void InstaKill()
    {
        //print("Instakilling Entity: " + name);
        CurrentHealth = 0;
        CheckDeath();
    }

    // Take a given amount of health
    public void TakeDamage(float damage)
    {
        //print("Entity taking damage: " + name);
        CurrentHealth -= damage;
        ClampHealth();

        if (CurrentHealth > DangerHealthThreshold)
        {
            IsCooling = true;
            CooldownTimer = 0;
        }

        CheckDeath();
    }

    private void SetHealth(float sethealth)
    {
        CurrentHealth = sethealth;
        ClampHealth();
        CheckDeath();
    }
	
    // Clamps Health from 0 to MaxHealth
	void ClampHealth()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    public float GetHealthRatio()
    {
        return CurrentHealth / MaxHealth;
    }

    public bool IsFullHealth()
    {
        return CurrentHealth >= MaxHealth ? true : false;
    }

    public bool IsAlive()
    {
        return CurrentHealth > 0;
    }
    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }

    public void CheckDeath()
    {
        if (CurrentHealth <= 0 && !Invincible)
        {
            /// @@TODO.........
            /// 
            Kill();
        }
    }

    public void SetMagicPixel()
    {
        CurrentHealth = 1;
    }

    public void AddFuncToDeathFuncs(DeathFunction dfunc)
    {
        DeathFunctionArrayList.Add(dfunc);
    }

    public void Kill()
    {
        print("ENTITY HAS DIED: " + name);

        // If no Death Functions, just destroy the object
        if (DeathFunctionArrayList.Count == 0)
            Destroy(gameObject);

        foreach (DeathFunction dfunc in DeathFunctionArrayList)
        {
            dfunc();
        }

        enabled = false;
    }

    #endregion METHODS_SPECIFIC
}
