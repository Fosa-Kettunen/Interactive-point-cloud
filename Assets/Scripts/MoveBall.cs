using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveBall : MonoBehaviour
{
    [SerializeField] InputActionReference MouseHold;
    [SerializeField] InputActionReference Scroll;
    [Space(10)]
    [SerializeField] Transform Anchor;
    [SerializeField] float offset = 0;
    Transform _cam;

    static Vector3 invertForward = new(1, 0, 1);
    bool _isHolding = false;

    void OnEnable()
    {
        // subscribe to inputs
        if (Scroll != null)
            Scroll.action.performed += OnScroll;
        MouseHold.action.performed += HoldPerfomed;
        MouseHold.action.canceled += HoldEnded;
    }
    void OnDisable()
    {
        Scroll.action.performed -= OnScroll;
    }
    void Start()
    {
        if (Anchor == null)
        {
            this.enabled = false;
            Debug.LogWarning($"Anchor was  not set in {this}");
            return;
        }
        if (MouseHold == null)
        {
            this.enabled = false;
            Debug.LogWarning($"MouseHold was  not set in {this}");
            return;
        }
 
        _cam = Camera.main.transform;

    }
    void HoldPerfomed(InputAction.CallbackContext context)
    {
        if (_isHolding) return;
        _isHolding = true;
        StartCoroutine(RunHold());

    }
    IEnumerator RunHold()
    {
        while (_isHolding)
        {
            CastObjToUpplane();
            yield return null;
        }
    }
    void HoldEnded(InputAction.CallbackContext context)
    {
        _isHolding = false;
    }

    void OnScroll(InputAction.CallbackContext context)
    {
        var dir = context.action.ReadValue<float>();
        transform.localScale *= 1 + dir * 0.1f;
    }
    /// <summary>
    /// Cast object to flat camera facing plane in a anchorpoint
    /// </summary>
    void CastObjToUpplane()
    {
        Vector3 planeNormal = _cam.position - Anchor.position;
        planeNormal.Scale(invertForward);

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new(planeNormal.normalized, Anchor.position);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitpoint = ray.GetPoint(distance + offset);
            
            // Apply position
            transform.position = hitpoint;
            transform.LookAt(_cam.position);
        }

    }
}
