using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo Reserve", menuName = "Ammo Type/Ammo Reserve")]
public class AmmoReserve : ScriptableObject
{
    public string AmmoName;
    public Weapon_BASE.WeaponType AmmoOfWeaponType;
    public GameObject Prefab_Bullet;
    public GameObject Prefab_Shell;

    public int MaxAmmoReservable;
    public int CurrentAmmoReserved;

    public int MaxClipSize;
    public int AmmoInClip;

    public int FiringCost;
    public float FireRate;
    public float ReloadTime;
    public float Inaccuracy;  // In Degrees

    public Color AmmoAestheticalColor;

    public bool InfiniteAmmo;


 #region METHODS_RESERVEMANIPULATION

    private int AddToReserve(int amount)
    {
        // Check for full reserve, first
        if (IsReserveFull())
        {
            Debug.Log("Adding Ammo To Reserve: FAILED because full");
            return 0;
        }

        int amountgiven = amount;

        if (amount + CurrentAmmoReserved > MaxAmmoReservable)
            amountgiven = MaxAmmoReservable - CurrentAmmoReserved;

        CurrentAmmoReserved += amountgiven;

        Debug.Log("Adding Ammo To Reserve: SUCCESS");
        return amountgiven;
    }
    private int SubtractFromReserve(int amount)
    {
        // Check for full reserve, first
        if (IsReserveEmpty())
        {
            Debug.Log("Subtracting Ammo From Reserve: FAILED");
            return 0;
        }


        int amounttaken = amount;

        if (amount > CurrentAmmoReserved)
            amounttaken = CurrentAmmoReserved;

        CurrentAmmoReserved -= amounttaken;

        Debug.Log("Subtracting Ammo From Reserve: SUCCESS");
        return amounttaken;
    }


    public void GrantFullAmmoReserve()
    {
        CurrentAmmoReserved = MaxAmmoReservable;
    }
    public void ResetAmmoReserve()
    {
        CurrentAmmoReserved = 0;
    }

    #endregion METHODS_RESERVEMANIPULATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

#region METHODS_CLIPMANIPULATION

    public bool UseAmmoFromFiring()
    {
        if (IsClipEmpty())
        {
            Debug.Log("Firing From Clip: FAILED");
            return false;
        }

        AmmoInClip = Mathf.Clamp(AmmoInClip -= FiringCost, 0, MaxClipSize);

        return true;
    }
    // Used for reloading
    public bool FillAmmoClip(bool takefromreserve = true)
    {
        // INFINITE AMMO CHEAT
        if (InfiniteAmmo)
        {
            AmmoInClip = MaxClipSize;
            return true;
        }


        if (IsReserveEmpty())
        {
            Debug.Log("Refilling Ammo From Reserve: FAILED");
            return false;
        }

        // Subtraction from current reserve
        if (takefromreserve)
            AmmoInClip = SubtractFromReserve(MaxClipSize - AmmoInClip);
        else
            AmmoInClip = MaxClipSize;

        Debug.Log("Refilling Ammo From Reserve: SUCCESS");
        return true;
    }
    public void ResetAmmoClip(bool returnammotoreserve = true)
    {
        if (returnammotoreserve)
            AddToReserve(AmmoInClip);

        AmmoInClip = 0;
    }

    #endregion METHODS_CLIPMANIPULATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

#region METHODS_GETTERS

    public bool IsReserveFull()
    {
        return CurrentAmmoReserved == MaxAmmoReservable ? true : false;
    }
    public bool IsReserveEmpty()
    {
        if (InfiniteAmmo)
            return true;

        return CurrentAmmoReserved == 0 ? true : false;
    }
    public bool IsClipFull()
    {
        return AmmoInClip == MaxClipSize ? true : false;
    }
    public bool IsClipEmpty()
    {
        if (InfiniteAmmo)
            return true;

        return AmmoInClip == 0 ? true : false;
    }

    #endregion METHODS_GETTERS

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
}
