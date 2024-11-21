using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveBall : MonoBehaviour
{
    public InputActionReference MouseHold;
    public InputActionReference Scroll;
    [Space(10)]
    public Transform Anchor;
    public float offset = 0;
    Transform _cam;
    public float Progress => _waitTillHold = _time / _waitTillHold;
    float _waitTillHold = .5f;
    float _time;
    static Vector3 invertForward = new(1, 0, 1);
    bool _isHolding = false;

    private void OnEnable()
    {
        if (Scroll != null)
            Scroll.action.performed += OnScroll;
        MouseHold.action.performed += HoldPerfomed;
        MouseHold.action.canceled += HoldEnded;
    }
    private void HoldPerfomed(InputAction.CallbackContext context)
    {
        if(_isHolding) return;
       _isHolding = true;
        StartCoroutine(RunHold());

    }
    private void OnDisable()
    {
        Scroll.action.performed -= OnScroll;
    }
    IEnumerator RunHold()
    {
        while (_isHolding)
        {
            CastObjToUpplane();
            yield return null;
        }
    }
    private void HoldEnded(InputAction.CallbackContext context)
    {
        _isHolding = false;
    }

    private void OnScroll(InputAction.CallbackContext context)
    {
        var dir = context.action.ReadValue<float>();
        transform.localScale *= 1 + dir * 0.1f;
    }

    private void Start()
    {
        
        bool isPointSet = Anchor != null;
        bool hasInput = MouseHold != null;

        if (!isPointSet | !hasInput)
        {
            this.enabled = false;
            return;
        }
        _cam = Camera.main.transform;

    } 



    private void CastObjToUpplane()
    {
        Vector3 planeNormal = _cam.position - Anchor.position;
        planeNormal.Scale(invertForward);

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); 
        Plane plane = new(planeNormal.normalized, Anchor.position);
        if (plane.Raycast(ray,out float distance))
        {
            Vector3 hitpoint = ray.GetPoint(distance + offset);
            transform.position = hitpoint;
            transform.LookAt(_cam.position);
        }

    }
}
