using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingAnimation : MonoBehaviour
{
    public int interpolationFramesCount = 60; // Number of frames to completely interpolate between the 2 positions
    int elapsedFrames = 0;

    Vector3 startPos;
    Vector3 endPos;
    [Range(0f, 1f)]
    public float midRatio = 0.4f;
    public Vector3 offset;

    private void Start()
    {
        startPos = transform.position;
        endPos = transform.position + offset;
    }

    void FixedUpdate()
    {
        float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;

        if (interpolationRatio < midRatio)
        {
            float animRatio = interpolationRatio / midRatio;
            Vector3 interpolatedPosition = Vector3.Lerp(startPos, endPos, animRatio);
            transform.position = interpolatedPosition;
        }
        else if (interpolationRatio < midRatio * 2)
        {
            float animRatio = (interpolationRatio - midRatio) / midRatio;
            Vector3 interpolatedPosition = Vector3.Lerp(endPos, startPos, animRatio);
            transform.position = interpolatedPosition;
        }

        elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)
    }
}
