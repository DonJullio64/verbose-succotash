using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventMGR : MonoBehaviour
{
    #region VARIABLES
    
    private Dictionary<Type, HashSet<EventCallback> > EventDictionary;
    private static EventMGR EventManager;

    public delegate void EventCallback(BASE_Event baseevent);
    EventCallback CallbackFunctions;

    #endregion VARIABLES

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region INSTANCE

    public static EventMGR STATIC_EventMGR
    {
        get
        {
            if (!EventManager)
            {
                EventManager = FindObjectOfType(typeof(EventMGR)) as EventMGR;

                if (!EventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    EventManager.Init();
                }
            }

            return EventManager;
        }
    }

    #endregion INSTANCE

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_INITIALIZATION

    void Init()
    {
        if (EventDictionary == null)
        {
            EventDictionary = new Dictionary<Type, HashSet<EventCallback> >();
        }
    }

    #endregion METHODS_INITIALIZATION

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    #region METHODS_SPECIFIC

    public void SubscribeToEvent(Type eventType, EventCallback listener)
    {
        HashSet<EventCallback> thisEvent = null;
        if (STATIC_EventMGR.EventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.Add(listener);
        }
        else
        {
            Debug.Log("Attempting to subscribe to an event that is not logged in the dictionary.\nAdding new EVENT TYPE: " + eventType + " for  subscriber: " + listener);

            thisEvent = new HashSet<EventCallback>();
            thisEvent.Add(listener);
            STATIC_EventMGR.EventDictionary.Add(eventType, thisEvent);
        }
    }

    public void UnSubscribeFromEvent(Type eventType, EventCallback listener)
    {
        if (EventManager == null) return;

        HashSet<EventCallback> thisEvent = null;
        if (STATIC_EventMGR.EventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.Remove(listener);
        }
    }

    public void DispatchEvent(Type eventType, BASE_Event baseevent)
    {
        print("Triggering Event of Type: " + eventType);
        HashSet<EventCallback> thisEvent = null;

        if (STATIC_EventMGR.EventDictionary.TryGetValue(eventType, out thisEvent))
        {
            foreach (EventCallback callback in thisEvent)
                callback(baseevent);
        }
    }

    #endregion METHODS_SPECIFIC

}
