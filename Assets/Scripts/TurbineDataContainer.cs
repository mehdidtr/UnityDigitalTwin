using UnityEngine;

/// <summary>
/// A container for storing and managing turbine data.
/// </summary>
[CreateAssetMenu(fileName = "TurbineDataContainer", menuName = "ScriptableObjects/TurbineDataContainer")]
public class TurbineDataContainer : ScriptableObject
{
    /// <summary>
    /// Array of all turbine data objects.
    /// </summary>
    public TurbineData[] turbines;

    /// <summary>
    /// Retrieves the turbine data for a specific turbine by its ID.
    /// </summary>
    /// <param name="turbineID">The unique ID of the turbine.</param>
    /// <returns>The <see cref="TurbineData"/> associated with the provided ID, or null if not found.</returns>
    public TurbineData GetTurbineDataByID(string turbineID)
    {
        // Search through the turbines array to find the matching turbine by ID
        foreach (var turbine in turbines)
        {
            if (turbine.turbineID == turbineID)
            {
                return turbine;
            }
        }

        // Log an error if no turbine with the given ID was found
        Debug.LogError($"TurbineData for ID {turbineID} not found!");
        return null;
    }
}
