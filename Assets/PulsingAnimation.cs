using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PulsingAnimation : MonoBehaviour
{
    public int durationInMS = 2000; // Seconds
    int interpolationFramesCount; // Number of frames to completely interpolate between the 2 positions
    int elapsedFrames = 0;

    [Range(0f, 1f)] public float startOpacity = 0;
    [Range(0f, 1f)] public float endOpacity = 1;

    [Range(0f, 0.5f)] public float pauseRatio = 0.2f;
    float midRatio;

    float difference;

    CanvasGroup cv;

    private void Start()
    {
        interpolationFramesCount = durationInMS / 1000 * 60;
        midRatio = (1f - pauseRatio) / 2;
        difference = endOpacity - startOpacity;

        cv = GetComponent<CanvasGroup>();
    }

    void FixedUpdate()
    {
        float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;

        if (interpolationRatio < midRatio)
        {
            float animRatio = interpolationRatio / midRatio;
            cv.alpha = startOpacity + (difference * animRatio);
        }
        else if (interpolationRatio <= midRatio * 2)
        {
            float animRatio = (interpolationRatio - midRatio) / midRatio;
            cv.alpha = endOpacity - (difference * animRatio);
        }

        elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)
    }
}
