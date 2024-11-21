using System;
using UnityEngine;
using Color = UnityEngine.Color;
using diagnose = System.Diagnostics;
[Serializable]
public class PointChacheDataReader
{
    public Texture2D LocationTexture;
    public int ReadElements;
    public int MaxPointsToSearch = 10_000;
    public float rayRadius = 0.15f;
  

    public event Action<Vector3> OnClosestPointFound;
    public event Action OnClosestPointFailed;
    public bool HasAllData => CheckAllValues();
    diagnose.Stopwatch _Stopwatch = new();
    Color[] _colorReadArray;

    bool CheckAllValues()
    {
        return LocationTexture != null & ReadElements > 0 & MaxPointsToSearch>0;
    }
    public void UpdateTextureToMemory()
    {
        _colorReadArray = LocationTexture.GetPixels();
        //Could strip the texture here
    }
    public void CleanMemory() => _colorReadArray = null;
    public void ClosesltPoint(Ray ray,Matrix4x4 transfoms)
    {
        if (!HasAllData) return;
       StartStopWatch();
        _colorReadArray ??= LocationTexture.GetPixels();

        if (_colorReadArray.Length < ReadElements)
        {
            Debug.LogWarning("Length was set too long");
            return;
        }

        var storedD = float.MaxValue;
        var storedCoord = Vector3.zero;
        int loopSteps = Mathf.Max(1, (int)(ReadElements / MaxPointsToSearch)); // max for loop so pc wont freeze

        for (int i = 0; i < ReadElements; i += loopSteps)
        {
            // read point
            var coord = new Vector3(_colorReadArray[i].r, _colorReadArray[i].g, _colorReadArray[i].b);

            // apply obj world transform
            coord = transfoms.MultiplyPoint3x4(coord);


            var d = Vector3.Distance(coord, ray.origin);
            if ((DistanceToLine(ray, coord) <= rayRadius) & (d < storedD))
            {
                storedD = d;
                storedCoord = coord;
            }
        }
        StowatchEnd();
        
        if (storedD == float.MaxValue)
        {
            OnClosestPointFailed.Invoke();
            return;
        }
        OnClosestPointFound?.Invoke(storedCoord);

 
    }
    static float DistanceToLine(Ray ray, Vector3 point) 
        => Vector3.Cross(ray.direction, point - ray.origin).magnitude;
    
    #region Stopwatch
    diagnose.Stopwatch StartStopWatch()
    {
        _Stopwatch.Start();
        return _Stopwatch;
    }

    void StowatchEnd()
    {
        _Stopwatch.Stop();
        TimeSpan ts = _Stopwatch.Elapsed;
        _Stopwatch.Reset();
        string elapsedTime = String.Format("{0}ms", ts.Milliseconds);
        //Debug.Log(elapsedTime);
    }
    #endregion

}
