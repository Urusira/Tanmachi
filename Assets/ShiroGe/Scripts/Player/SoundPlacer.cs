using UnityEngine;

public class SoundPlacer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public void PlayStep()
    {
        audioSource.Play();
    }
}
