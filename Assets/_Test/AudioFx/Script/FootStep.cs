using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using UniRx;

public class FootStep : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private CharacterController controller;

    [SerializeField] private float footstepThreshold;
    [SerializeField] private float footstepRate;

    private float lastFootstepTime;

    private void Start()
    {
        Observable.EveryUpdate()
            .Subscribe(_ => PlaySound())
            .AddTo(gameObject);
    }
    private void PlaySound()
    {
        var magnitude = controller.velocity
            .magnitude;

        if(magnitude > footstepThreshold)
        {
            if (!TimePassed()) return;

            var leng = footstepClips.Length;

            lastFootstepTime = Time.time;
            audioSource.PlayOneShot(footstepClips[Random.Range(0, leng)]);
        }
    }
    
    private bool TimePassed()
    {
        var timeGap = Time.time - lastFootstepTime;

        return timeGap > footstepRate;
    }
}

