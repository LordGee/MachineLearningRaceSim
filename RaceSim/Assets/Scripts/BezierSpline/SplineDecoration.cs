using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The following code was hand typed using one of cat like codding tutorials for creating BezierSplines
/// The purpose of this was to create Track Pieces that could be utilised within the game.
/// Reference : http://catlikecoding.com/unity/tutorials/curves-and-splines/
/// </summary>
public class SplineDecoration : MonoBehaviour {

    public BezierSpline spline;

    public int frequency;

    public bool lookForward;

    public Transform[] items;

    private void Awake() {
        if (frequency <= 0 || items == null || items.Length == 0) {
            return;
        }
        float stepSize = frequency * items.Length;
        if (spline.Loop || stepSize == 1) {
            stepSize = 1f / stepSize;
        } else {
            stepSize = 1f / (stepSize - 1);
        }
        for (int p = 0, f = 0; f < frequency; f++) {
            for (int i = 0; i < items.Length; i++, p++) {
                Transform item = Instantiate(items[i]) as Transform;
                Vector3 position = spline.GetPoint(p * stepSize);
                item.transform.localPosition = position;
                if (lookForward) {
                    item.transform.LookAt(position + spline.GetDirection(p * stepSize));
                }
                item.transform.parent = transform;
            }
        }
    }
}
