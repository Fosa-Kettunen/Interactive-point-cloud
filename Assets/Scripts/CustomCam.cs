using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCam : MonoBehaviour
{
   [SerializeField] Transform target;
    [SerializeField] float Speed =20;
    private void OnValidate()
    {
   
        if (Speed <= 0)
            Speed = 1;
    }
    private void Start()
    {
        if (target == null)
            this.enabled = false;
    }
    void Update()
    {
        transform.RotateAround(target.transform.position, Vector3.up, Speed * Time.deltaTime);
    }
    
}
