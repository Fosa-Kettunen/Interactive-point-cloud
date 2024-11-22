using UnityEngine;
using UnityEngine.InputSystem;

public class MoveAndScaleObject : MonoBehaviour
{
    [SerializeField] InputActionReference Scroll;
    [SerializeField] InputActionReference PointerPosition;
    [Space(10)]
    [SerializeField] Transform Anchor;
    [SerializeField] float AnchorOffset = 0;

    const float ScaleFactor = 0.1f;

    const float timeTillRest = 20f;
    float _timer;
    Ray _ray;
    Plane _plane;
    Camera _cam;
    Transform _cam_transform;
    readonly static Vector3 invertForward = new(1, 0, 1);

    void OnEnable()
    {
        // subscribe to inputs
        if (Scroll != null)
            Scroll.action.performed += OnScroll;
        if (PointerPosition != null)
            PointerPosition.action.performed += OnPointerMoved;
    }
    void OnDisable()
    {
        Scroll.action.performed -= OnScroll;
        PointerPosition.action.performed -= OnPointerMoved;
    }

    void Start()
    {
        if (Anchor == null)
        {
            this.enabled = false;
            Debug.LogWarning($"Anchor was  not set in {this}");
            return;
        }

        // Cache 
        _cam_transform = Camera.main.transform;
        _cam = Camera.main;
    }
    void Update()
    {
        MoveObjectToPoint();
    }
    /// <summary>
    /// Move object to pointer untill time runs out
    /// </summary>
    void MoveObjectToPoint()
    {

        if (_timer < timeTillRest)
        {
            CastObjToUpplane();
            _timer += Time.deltaTime;
        }

    }

    void OnPointerMoved(InputAction.CallbackContext context) => _timer = 0;

    void OnScroll(InputAction.CallbackContext context) => ScaleObj(context.action.ReadValue<float>());

    void ScaleObj(float scaleAmount)
    {
        transform.localScale *= 1 + scaleAmount * ScaleFactor;
    }

    /// <summary>
    /// Cast object to flat camera facing plane in a anchorpoint
    /// </summary>
    void CastObjToUpplane()
    {
        Vector3 planeNormal = _cam_transform.position - Anchor.position;
        planeNormal.Scale(invertForward);

        _ray = _cam.ScreenPointToRay(PointerPosition.action.ReadValue<Vector2>());
        _plane = new(planeNormal.normalized, Anchor.position);
        if (_plane.Raycast(_ray, out float distance))
        {
            Vector3 hitpoint = _ray.GetPoint(distance + AnchorOffset);

            // Apply position
            transform.position = hitpoint;
            transform.LookAt(_cam_transform.position);
        }

    }
}
