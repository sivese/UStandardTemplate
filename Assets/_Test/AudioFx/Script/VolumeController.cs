using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

using UniRx;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private GameObject window;
    [SerializeField] private Slider masterSlider;

    private void Start()
    {
        if(PlayerPrefs.HasKey("MasterVolume"))
        {
            mixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        }
        
        SetSlider();

        masterSlider.ObserveEveryValueChanged(x => x.value)
            .Subscribe(_ => UpdateMasterVolume())
            .AddTo(gameObject);

    }

    private void SetSlider()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
    }

    public void UpdateMasterVolume()
    {
        mixer.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            window.SetActive(!window.activeInHierarchy);

            if (window.activeInHierarchy) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
