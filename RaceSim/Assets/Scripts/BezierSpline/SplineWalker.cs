using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SplineWalkerMode {
    Once,
    Loop,
    PingPong
}

/// <summary>
/// The following code was hand typed using one of cat like codding tutorials for creating BezierSplines
/// The purpose of this was to create Track Pieces that could be utilised within the game.
/// Reference : http://catlikecoding.com/unity/tutorials/curves-and-splines/
/// </summary>
public class SplineWalker : MonoBehaviour {

    public BezierSpline spline;

    public float duration;

    private float progress;

    public bool lookForward;

    public SplineWalkerMode mode;

    private bool goingForward = true;

    private void Update() {
        if (goingForward) {
            progress += Time.deltaTime / duration;
            if (progress > 1f) {
                if (mode == SplineWalkerMode.Once) {
                    progress = 1f;
                } else if (mode == SplineWalkerMode.Loop) {
                    progress -= 1f;
                } else {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        } else {
            progress -= Time.deltaTime / duration;
            if (progress < 0f) {
                progress = -progress;
                goingForward = true;
            }
        }
        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = position;
        if (lookForward) {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }
}
