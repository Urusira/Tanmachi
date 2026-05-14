using System;
using UnityEngine;

public class DayNightCycleManager : MonoBehaviour
{
    [SerializeField] private GameObject theSun;

    private void Start()
    {
        theSun.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void SetSunTimeRotation(float currentTime)
    {
        theSun.transform.rotation = Quaternion.Euler(currentTime/10, 0, 0);
    }
}
