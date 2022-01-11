using UnityEngine;
using UnityEngine.UI;

// https://forum.unity.com/threads/test-if-ui-element-is-visible-on-screen.276549/#post-2978773
// https://stackoverflow.com/questions/30766020/how-to-scroll-to-a-specific-element-in-scrollrect-with-unity-ui/30769550

public static class RendererExtensions
{
    private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);

        int visibleCorners = 0;
        Vector3 tempScreenSpaceCorner; // Cached
        for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
        {
            tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]); // Transform world space position of corner to screen space
            if (screenBounds.Contains(tempScreenSpaceCorner)) // If the corner is inside the screen
            {
                visibleCorners++;
            }
        }
        return visibleCorners;
    }

    public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
    }

    public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
    }

    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }
}
