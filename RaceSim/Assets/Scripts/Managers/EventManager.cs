using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OneFloatEvent : UnityEvent<float> { }

/// <summary>
/// Reference: Unity3D - Events: Creating a simple messaging system
/// URL: https://unity3d.com/learn/tutorials/topics/scripting/events-creating-simple-messaging-system
/// Some modification have allowed me to supply argumants of one float value to be passed through with 
/// an event trigger. The start stop and trigger event functions below have been overloaded to allow for 
/// this option.
/// </summary>
public class EventManager : MonoBehaviour {

    private Dictionary<string, UnityEvent> eventDictionary;
    private Dictionary<string, OneFloatEvent> eventDictionaryOneFloat;
    private static EventManager eventManager;

    public static EventManager instance {
        get {
            if (!eventManager) {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!eventManager) {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                } else {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init() {
        if (eventDictionary == null) {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
        if (eventDictionaryOneFloat == null) {
            eventDictionaryOneFloat = new Dictionary<string, OneFloatEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener) {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.AddListener(listener);
        } else {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StartListening(string eventName, UnityAction<float> listener) {
        OneFloatEvent thisEvent = null;
        if (instance.eventDictionaryOneFloat.TryGetValue(eventName, out thisEvent)) {
            thisEvent.AddListener(listener);
        } else {
            thisEvent = new OneFloatEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionaryOneFloat.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener) {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void StopListening(string eventName, UnityAction<float> listener) {
        if (eventManager == null) return;
        OneFloatEvent thisEvent = null;
        if (instance.eventDictionaryOneFloat.TryGetValue(eventName, out thisEvent)) {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName) {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.Invoke();
        }
    }

    public static void TriggerEvent(string eventName, float value) {
        OneFloatEvent thisEvent = null;
        if (instance.eventDictionaryOneFloat.TryGetValue(eventName, out thisEvent)) {
            thisEvent.Invoke(value);
        }
    }
}
