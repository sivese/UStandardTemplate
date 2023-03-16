using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crickets : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float stopDistance;

    [SerializeField] private Transform player;
    [SerializeField] private float defaultVolume;

    private void Start()
    {
        defaultVolume = audioSource.volume;
        player = FindObjectOfType<PlayerController>().transform;
    }

    private void Update()
    {
        if (player != null) return;

        var dist = Vector3.Distance(transform.position, player.position);

        if (dist > stopDistance) audioSource.volume = defaultVolume;
        else audioSource.volume = 0.0f;
    }
}
