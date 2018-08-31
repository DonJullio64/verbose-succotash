using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using IceEvents.Player;

public class PlayerHealth : Health
{
    #region VARIABLES

    
    public TextMeshProUGUI TMPUGUI_HealthPercent;
    public TextMeshProUGUI TMPUGUI_ConditionText;
    public TextMeshProUGUI TMPUGUI_Temperature;
    public Range_Unanchored RangeU_Temperature;
    public RectTransform RectTransform_HealthBarRoot;
    public Range_Unanchored RangeU_HealthBarWidth;
    public Image Image_HealthBar;
    public Image Image_HealthDot;

    public int Index_HealthStages;
    public List<Struct_HealthStage> LIST_HealthStages;
    [HideInInspector] public Struct_HealthStage Current_HealthStage;

    #endregion VARIABLES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_INITIALIZATION

    protected override void Awake()
    {
        base.Awake();

        EventMGR.STATIC_EventMGR.SubscribeToEvent(typeof(Ice_PlayerDamage), OnPlayerDamage);
        EventMGR.STATIC_EventMGR.SubscribeToEvent(typeof(Ice_PlayerHeal), OnPlayerHeal);
        EventMGR.STATIC_EventMGR.SubscribeToEvent(typeof(Ice_PlayerDEATH), OnPlayerDEATH);

        RangeU_Temperature.InitializeRangeU();
        RangeU_HealthBarWidth.InitializeRangeU();
        InitializeHealthStage();
    }


    private void InitializeHealthStage()
    {
        //Index_HealthStages = LIST_HealthStages.Count - 1;
        //Current_HealthStage = LIST_HealthStages[Index_HealthStages];

        SetNewHealthStage();
    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_UPDATE

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            EventMGR.STATIC_EventMGR.DispatchEvent(typeof(Ice_PlayerDamage), new Ice_PlayerDamage(10f));
        if (Input.GetKeyDown(KeyCode.L))
            EventMGR.STATIC_EventMGR.DispatchEvent(typeof(Ice_PlayerHeal), new Ice_PlayerHeal(10f));


    }

    #endregion METHODS_UPDATE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFICS

    private void UpdateHealthUI()
    {
        float hratio = GetHealthRatio();
        TMPUGUI_HealthPercent.text = (int)(hratio * 100) + " %";
        //TMPUGUI_ConditionText.text = ;
        TMPUGUI_Temperature.text = ( RangeU_Temperature.GetValueFromRatio(hratio) ).ToString("F1") + "°F";

        RectTransform_HealthBarRoot.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, RangeU_HealthBarWidth.GetValueFromRatio(hratio) );

        SetNewHealthStage();
    }


    /// <summary>
    /// REQUIRED: Update Index_HealthStages before calling this function!!!
    /// </summary>
    private void SetNewHealthStage()
    {
        float healthratio = GetHealthRatio();

        //print("Setting New Health Stage.");
        // Find the new health stage
        int i = 0;
        while (LIST_HealthStages[i].MaximumHealthPercent < healthratio)
        {
            i++;
        }
        Index_HealthStages = i;

        // Set new health  stage media
        Current_HealthStage = LIST_HealthStages[Index_HealthStages];


        // Update Health Stage UI
        Image_HealthBar.color = Current_HealthStage.HealthBarColor;
        Image_HealthDot.color = Current_HealthStage.HealthDotColor;
        TMPUGUI_ConditionText.text = Current_HealthStage.String_HealthCondition;
    }

    private void KILLPLAYER()
    {

    }

    #endregion METHODS_SPECIFICS

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_EVENTS

    private void OnPlayerDamage(IceEvent_BASE i)
    {
        Ice_PlayerDamage pd = (Ice_PlayerDamage)i;

        TakeDamage(pd.Damage);
        UpdateHealthUI();
    }

    private void OnPlayerHeal(IceEvent_BASE i)
    {
        Ice_PlayerHeal ph = (Ice_PlayerHeal)i;

        GainHealth(ph.Heal);
        UpdateHealthUI();
    }

    private void OnPlayerDEATH(IceEvent_BASE i)
    {
        Ice_PlayerDEATH pd = (Ice_PlayerDEATH)i;

        // Do THings
        KILLPLAYER();
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

[System.Serializable]
public struct Struct_HealthStage
{
    [Range(0f, 1f)]
    public float MaximumHealthPercent;
    public Color HealthBarColor;
    public Color HealthDotColor;
    public string String_HealthCondition;
}
