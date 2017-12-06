using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ThisEvent : UnityEvent<float> {
}
/// <summary>
/// Reference: Unity3D - Events: Creating a simple messaging system
/// URL: https://unity3d.com/learn/tutorials/topics/scripting/events-creating-simple-messaging-system
/// </summary>
public class EventManagerOneArg : MonoBehaviour {

    private Dictionary<string, ThisEvent> eventDictionary;

    private static EventManagerOneArg eventManagerArgs;

    public static EventManagerOneArg instance {
        get {
            if (!eventManagerArgs) {
                eventManagerArgs = FindObjectOfType(typeof(EventManagerOneArg)) as EventManagerOneArg;
                if (!eventManagerArgs) {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                } else {
                    eventManagerArgs.Init();
                }
            }
            return eventManagerArgs;
        }
    }

    void Init() {
        if (eventDictionary == null) {
            eventDictionary = new Dictionary<string, ThisEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction<float> listener) {
        ThisEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.AddListener(listener);
        } else {
            thisEvent = new ThisEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<float> listener) {
        if (eventManagerArgs == null) return;
        ThisEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, float value) {
        ThisEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.Invoke(value);
        }
    }
}