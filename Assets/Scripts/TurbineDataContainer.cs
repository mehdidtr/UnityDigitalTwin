using UnityEngine;

[CreateAssetMenu(fileName = "TurbineDataContainer", menuName = "ScriptableObjects/TurbineDataContainer")]
public class TurbineDataContainer : ScriptableObject
{
    public TurbineData[] turbines;

    public TurbineData GetTurbineDataByID(string turbineID)
    {
        foreach (var turbine in turbines)
        {
            if (turbine.turbineID == turbineID)
            {
                return turbine;
            }
        }
        return null; // If no turbine is found with the given ID
    }
}