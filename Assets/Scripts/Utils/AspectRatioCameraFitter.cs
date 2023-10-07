using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))][ExecuteAlways]
public class AspectRatioCameraFitter : MonoBehaviour
{
    [SerializeField] private Camera cam;
   
    private readonly Vector2 maxTargetAspectRatio = new Vector2(16,9);
    private readonly Vector2 rectCenter = new Vector2(0.5f, 0.5f);
 
    private Vector2 lastResolution;
 
    private void OnValidate()
    {
        cam ??= GetComponent<Camera>();
    }
 
    public void LateUpdate()
    {
        Vector2 currentScreenResolution = new Vector2(Screen.width, Screen.height);
 
        // Early exit if screen aspect ratio is under the max aspect ratio
        if((currentScreenResolution.x/currentScreenResolution.y) <= (maxTargetAspectRatio.x/maxTargetAspectRatio.y)) 
        {
            return;
        }

        // Don't run all the calculations if the screen resolution has not changed
        if (lastResolution != currentScreenResolution)
        {
            CalculateCameraRect(currentScreenResolution);
        }
 
        lastResolution = currentScreenResolution;
    }
 
    private void CalculateCameraRect(Vector2 currentScreenResolution)
    {
        Vector2 normalizedAspectRatio = maxTargetAspectRatio / currentScreenResolution;
        Vector2 size = normalizedAspectRatio / Mathf.Max(normalizedAspectRatio.x, normalizedAspectRatio.y);
        cam.rect = new Rect(default, size) { center = rectCenter };
    }
}

 