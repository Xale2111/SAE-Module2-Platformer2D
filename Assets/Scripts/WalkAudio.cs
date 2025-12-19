using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class WalkAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] walkSounds;
    
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayWalkAudio()
    {
        _audioSource.clip = walkSounds[Random.Range(0, walkSounds.Length)];
        _audioSource.pitch = Random.Range(.8f, 1.2f);
        _audioSource.Play();

        //_audioSource.PlayOneShot(_clip);

    }
}
