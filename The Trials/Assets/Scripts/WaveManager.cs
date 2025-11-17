using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private int currentWaveID = 1;

    public int GetCurrentWaveID()
    {
        return currentWaveID;
    }

    public void IncrementWaveID()
    {
        currentWaveID++;
    }
}
