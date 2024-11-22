using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

//[VFXBinder("Transform/Distance")]
//https://docs.unity3d.com/Packages/com.unity.visualeffectgraph@7.1/manual/PropertyBinders.html
public class WeaveController : VFXBinderBase
{
    #region VFXBind
    [VFXPropertyBinding("UnityEngine.Vector3")]
    public ExposedProperty shockLocation;
    [VFXPropertyBinding("System.Single")]
    public ExposedProperty phase;
    #endregion
    public bool IsRuning => _isRuning;

    [Header("Adjustments")]
    public float rangeStart = -1;
    public float rangeEnd = 5;

    public float _cost;
    public float SpeedMultiplier;

    float _currentTime;
    bool _isRuning = false;
    bool _startRunning = false;
    Vector3 point;
    public override bool IsValid(VisualEffect component)
    {
        return component.HasVector3(shockLocation) & component.HasFloat(phase);
    }
    public override void UpdateBinding(VisualEffect component)
    {
        if (_startRunning)
        {
            RunEffect(point,component);
            _startRunning = false;
        }
    }
     void OnValidate()
    {
        if (SpeedMultiplier<.001f)
            SpeedMultiplier = 001f;
    }
    public void StartEffect(Vector3 point)
    {
        this.point = point;
        _startRunning = true;
    }
    void RunEffect(Vector3 point, VisualEffect _visualEffect)
    {
        if (_isRuning) return;
        _isRuning = true;
        _currentTime = rangeStart;
        _visualEffect.SetVector3((int)shockLocation, point);
        StartCoroutine(RunWeave(_visualEffect));
    }
    IEnumerator RunWeave(VisualEffect _visualEffect)
    {
        while (_currentTime < rangeEnd)
        {
            _visualEffect.SetFloat((int)phase, _currentTime);
            _currentTime += Time.deltaTime * SpeedMultiplier;
            yield return null;
        }
        _isRuning = false;
        _visualEffect.SetFloat((int)phase, OffsetedVoid);
    }

    float OffsetedVoid => rangeStart - 1;

    void OnApplicationQuit()
    {
        GetComponent<VisualEffect>().SetFloat((int)phase, OffsetedVoid);
    }
 
}