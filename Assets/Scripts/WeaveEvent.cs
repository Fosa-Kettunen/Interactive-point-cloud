
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
public class WeaveEvent : MonoBehaviour
{
    // for getting visualEffect transfom
    public VisualEffect visualEffect;
    Matrix4x4 vfxObjTransform;
    [Space(10)]
    public WeaveController _weaveController;
    [Space(10)]
    public InputActionReference ClickPos;
    public InputActionReference Click;

    public PointChacheDataReader data = new();
     void Awake()
    {
        
        if (_weaveController == null)
        {
            Debug.LogWarning(_weaveController + " was not found", this);
            this.enabled = false;
            return;
        }

        UpdateVFXData();

        data.UpdateTextureToMemory();

    }
     void OnEnable()
    {
        // Subscribe
        Click.action.performed += OnClick;
        data.OnClosestPointFound += OnSuccessfulHit;
        data.OnClosestPointFailed += OnFailedHit;
    }

     void OnDisable()
    {
        Click.action.performed -= OnClick;
    }

    static readonly string AttributeName_position = "Transfrom_position";
    static readonly string AttributeName_scale = "Transfrom_scale";
    static readonly string AttributeName_angles = "Transfrom_angles";
    void UpdateVFXData()
    {
        // Ensure the VisualEffect component is assigned
        if (visualEffect == null)
        {
            Debug.LogWarning("VisualEffect component is not assigned");
            this.enabled = false;
            return;
        }
        // You get a warning and you get an warning
        if (!visualEffect.HasVector3(AttributeName_position))
            Debug.LogWarning($"{AttributeName_position} Transfrom attributes were not found");
   

        //get transform components
        Vector3 position =  visualEffect.GetVector3(AttributeName_position);
        Vector3 rotation = visualEffect.GetVector3(AttributeName_angles);
        Vector3 scale = visualEffect.GetVector3(AttributeName_scale);

        //assign
        vfxObjTransform = CreateTransformationMatrix(position, rotation, scale);
        
    }

    Matrix4x4 CreateTransformationMatrix(Vector3 translation, Vector3 eulerRotation, Vector3 scale)
    {
        // Set a Quaternion from the specified Euler angles.
        Quaternion rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, eulerRotation.z);

        // Set the translation, rotation and scale parameters.
        Matrix4x4 m = Matrix4x4.TRS(translation, rotation, scale);

        return m;
    }


     void OnClick(InputAction.CallbackContext context)
    {
        if (_weaveController.IsRuning) return;

        Ray ray = Camera.main.ScreenPointToRay(ClickPos.action.ReadValue<Vector2>());
        Debug.DrawRay(ray.origin, ray.direction*20, UnityEngine.Color.red,4f);
        
        // heavy operation probably can optimize
        data.ClosesltPoint(ray, vfxObjTransform);
    }
     void OnSuccessfulHit(Vector3 point) => _weaveController.StartEffect(point);

     void OnFailedHit() { }
}
