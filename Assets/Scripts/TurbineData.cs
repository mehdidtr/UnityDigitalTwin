using UnityEngine;

/// <summary>
/// A class to store the data of a single turbine, including its time intervals, event codes, and performance data.
/// </summary>
[System.Serializable]
public class TurbineData
{
    /// <summary>
    /// Unique identifier for the turbine.
    /// </summary>
    public string turbineID;

    /// <summary>
    /// Array of time intervals for the turbine's recorded data.
    /// </summary>
    public string[] timeIntervals;

    /// <summary>
    /// Array of event codes associated with the turbine.
    /// </summary>
    public int[] eventCodes;

    /// <summary>
    /// Array of descriptions corresponding to each event code.
    /// </summary>
    public string[] eventCodeDescriptions;

    /// <summary>
    /// Array of wind speeds recorded for the turbine at each time interval.
    /// </summary>
    public float[] windSpeeds;

    /// <summary>
    /// Array of ambient temperatures recorded for the turbine at each time interval.
    /// </summary>
    public float[] ambientTemperatures;

    /// <summary>
    /// Array of rotor speeds recorded for the turbine at each time interval.
    /// </summary>
    public float[] rotorSpeeds;

    /// <summary>
    /// Array of power outputs recorded for the turbine at each time interval.
    /// </summary>
    public float[] powers;
}
