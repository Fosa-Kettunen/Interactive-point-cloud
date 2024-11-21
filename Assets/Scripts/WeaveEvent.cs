
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
public class WeaveEvent : MonoBehaviour
{
    // for gettign transfom
    public VisualEffect visualEffect;
    Matrix4x4 vfxObjTransform;
    [Space(10)]
    public WeaveController _weaveController;
    [Space(10)]
    public InputActionReference ClickPos;
    public InputActionReference Click;

    public PointChacheDataReader data = new();
    private void Awake()
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

        if (!visualEffect.HasVector3(AttributeName_position))
            Debug.LogWarning($"{AttributeName_position} Transfrom attributes were found");
        if (!visualEffect.HasVector3(AttributeName_scale))
            Debug.LogWarning($"{AttributeName_scale} Transfrom attributes were found");
        if (!visualEffect.HasVector3(AttributeName_angles))
            Debug.LogWarning($"{AttributeName_angles} Transfrom attributes were found");


        Vector3 position =  visualEffect.GetVector3(AttributeName_position);
        Vector3 rotation = visualEffect.GetVector3(AttributeName_angles);
        Vector3 scale = visualEffect.GetVector3(AttributeName_scale);

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


    private void OnEnable()
    {
        Click.action.performed += OnClick;
        data.OnClosestPointFound += OnSuccessfulHit;
        data.OnClosestPointFailed += OnFailedHit;
      
    }
    
    private void OnDisable()
    {
        Click.action.performed -= OnClick;
    }
    private void OnClick(InputAction.CallbackContext context)
    {
        if (_weaveController.IsRuning) return;
        Ray ray = Camera.main.ScreenPointToRay(ClickPos.action.ReadValue<Vector2>());
        Debug.DrawRay(ray.origin, ray.direction*20, UnityEngine.Color.red,4f);
        // heavy operation
        data.ClosesltPoint(ray, vfxObjTransform);
    }
    private void OnSuccessfulHit(Vector3 point)
    {

        _weaveController.StartEffect(point);
    }

    private void OnFailedHit()
    {
        
    }






}
