using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WaveCountdownUI : MonoBehaviour
{
    [SerializeField] GameObject[] countdownObjects;

    [SerializeField] WaveManager waveManager;

    public void PlayWaveCountdown()
    {
        StartCoroutine(WaveCountdown());
    }

    IEnumerator WaveCountdown()
    {
        for (int i = 0; i < countdownObjects.Length; i++)
        {
            countdownObjects[i].SetActive(true);
            countdownObjects[i].GetComponent<Animator>().SetTrigger("GrowAndFade");
            yield return new WaitForSeconds(1f);
            countdownObjects[i].SetActive(false);
        }
        yield return new WaitForSeconds(2f);
        waveManager.StartWave(waveManager.waveDuration);
    }
}