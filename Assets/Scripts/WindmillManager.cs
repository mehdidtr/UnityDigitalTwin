using UnityEngine;

public class WindmillManager : MonoBehaviour
{
    public string turbineID; // Manually set the ID in the Inspector
    public TurbineDataContainer turbineDataContainer; // Drag and drop the TurbineDataContainer ScriptableObject

    private TurbineData turbineData; // Holds the data for the specific turbine

    private void Start()
    {
        // Fetch data for the turbineID from the container
        turbineData = turbineDataContainer.GetTurbineDataByID(turbineID);

        if (turbineData != null)
        {
            Debug.Log($"Windmill {turbineData.turbineID} initialized with {turbineData.timeIntervals.Length} data entries.");
            // You can now access rotorSpeed, power, etc., based on the turbineData
        }
        else
        {
            Debug.LogError($"No TurbineData found for ID: {turbineID}");
        }
    }
}