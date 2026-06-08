using System;
using UnityEngine;

public class DayNightCycleManager : MonoBehaviour
{
    [SerializeField] private GameObject theSun;

    private void Start()
    {
        theSun.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void SetSunTimeRotation(float newAngle)
    {
        theSun.transform.rotation = Quaternion.Euler(newAngle, 0, 0);
    }
}
