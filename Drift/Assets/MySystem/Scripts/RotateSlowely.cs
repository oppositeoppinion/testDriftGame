using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSlowely : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;
    private void Update()
    {
        transform.Rotate(Vector3.up, _rotateSpeed*Time.deltaTime);
    }
}
