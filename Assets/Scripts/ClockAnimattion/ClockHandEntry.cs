using UnityEngine;

[System.Serializable]
public class ClockHandEntry
{
    public float DegreesPerUnit
    {
        get { return 360f / UnitsPerRevolution; }
    }

    public Transform HandBone;
    public TimeUnit TimeUnit;
    public bool IncludeNextLowerUnit;
    public float UnitsPerRevolution;
}