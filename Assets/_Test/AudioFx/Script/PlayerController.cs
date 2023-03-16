using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UniRx;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private CharacterController controller;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSensitivity;
    [SerializeField] private float minXRotation;
    [SerializeField] private float maxXRotation;

    private float currentXRotation;

    // Start is called before the first frame update
    private void Start()
    {
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                Move();
                Look();
            })
            .AddTo(gameObject);
    }

    private void Move()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");

        var direction = Vector3.right * x + Vector3.forward * z;

        direction *= moveSpeed * Time.deltaTime;

        controller.Move(direction);
    }

    private void Look()
    {
        var x = Input.GetAxis("Mouse X") * lookSensitivity;
        var y = Input.GetAxis("Mouse Y") * lookSensitivity;

        transform.eulerAngles += Vector3.up * x;

        currentXRotation += y;
        currentXRotation = Mathf.Clamp(currentXRotation, minXRotation, maxXRotation);

        cameraTransform.localEulerAngles = new Vector3(-1f * currentXRotation, 0f, 0f);
    }
}
