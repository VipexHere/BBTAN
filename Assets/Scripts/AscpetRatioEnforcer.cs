using UnityEngine;
using System.Collections;

public class AspectRatioEnforcer : MonoBehaviour
{
    // Target aspect ratio (9:16)
    private float targetAspect = 9f / 16f;

    IEnumerator Start()
    {
        // Wait one frame for Unity to set up the resolution
        yield return null;
        EnforceAspectRatio();
    }

    private int lastWidth;
    private int lastHeight;

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            EnforceAspectRatio();
        }
    }

    void EnforceAspectRatio()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera cam = GetComponent<Camera>();

        if (scaleHeight < 1f)
        {
            // Bars on top and bottom
            Rect rect = cam.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;
            cam.rect = rect;
        }
        else
        {
            // Bars on left and right
            float scaleWidth = 1f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;
            cam.rect = rect;
        }
    }
}