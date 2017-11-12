using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Lang;
using Array = System.Array;

public class BezierSpline : MonoBehaviour {

    [SerializeField] private Vector3[] points;
    [SerializeField] private BezierControlPointMode[] modes;
    [SerializeField] private bool loop;

    public void Reset() {
        points = new Vector3[]
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
        modes = new BezierControlPointMode[]
            {
                BezierControlPointMode.Free,
                BezierControlPointMode.Free
            };
    }

    public Vector3 GetPoint(float _t) {
        int i;
        if (_t >= 1f) {
            _t = 1f;
            i = points.Length - 4;
        } else {
            _t = Mathf.Clamp01(_t) * CurveCount;
            i = (int)_t;
            _t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], _t));
    }

    public Vector3 GetVelocity(float _t) {
        int i;
        if (_t >= 1f) {
            _t = 1f;
            i = points.Length - 4;
        } else {
            _t = Mathf.Clamp01(_t) * CurveCount;
            i = (int)_t;
            _t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], _t)) -
               transform.position;
    }

    public Vector3 GetDirection(float _t) {
        return GetVelocity(_t).normalized;
    }

    public void AddCurve() {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop) {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }

    public int CurveCount { get { return (points.Length - 1) / 3; } }

    public int ControlPointCount { get { return points.Length; } }

    public Vector3 GetControlPoints(int _index) {
        return points[_index];
    }

    public void SetControlPoint(int _index, Vector3 _point) {
        if (_index % 3 == 0) {
            Vector3 delta = _point - points[_index];
            if (loop) {
                if (_index == 0) {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = _point;
                } else if (_index == points.Length - 1) {
                    points[0] = _point;
                    points[1] += delta;
                    points[_index - 1] += delta;
                } else {
                    points[_index - 1] += delta;
                    points[_index + 1] += delta;
                }
            } else {
                if (_index > 0) {
                    points[_index - 1] += delta;
                }
                if (_index + 1 < points.Length) {
                    points[_index + 1] += delta;
                }
            }
        }
        points[_index] = _point;
        EnforceMode(_index);
    }

    public BezierControlPointMode GetControlPointMode(int _index) {
        return modes[(_index + 1) / 3];
    }

    public void SetControlPointMode(int _index, BezierControlPointMode _mode)
    {
        int modeIndex = (_index + 1) / 3;
        modes[modeIndex] = _mode;
        if (loop) {
            if (modeIndex == 0) {
                modes[modes.Length - 1] = _mode;
            }
            else if (modeIndex == modes.Length - 1) {
                modes[0] = _mode;
            }
        }
        EnforceMode(_index);
    }

    private void EnforceMode(int _index)
    {
        int modeIndex = (_index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
            return;
        }
        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (_index <= middleIndex) {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0) {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= points.Length) {
                enforcedIndex = 1;
            }
        } else {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= points.Length) {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0) {
                enforcedIndex = points.Length - 2;
            }
        }
        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned) {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }

    public bool Loop {
        get { return loop; }
        set {
            loop = value;
            if (value) {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }
}
