using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimedEvent : MonoBehaviour
{
    public UnityEvent OnEvent;

    void Start()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(3);
        OnEvent.Invoke();
    }
}