using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UniRx;

public class Flyer : MonoBehaviour
{
    [SerializeField] private Vector3 axis;
    [SerializeField] private float speed;

    private void Start()
    {
        Observable.EveryUpdate()
            .Subscribe(_ => transform.Rotate(axis, speed * Time.deltaTime))
            .AddTo(this);
    }
}
