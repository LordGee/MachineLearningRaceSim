﻿using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The following code was hand typed using one of cat like codding tutorials for creating BezierSplines
/// The purpose of this was to create Track Pieces that could be utilised within the game.
/// Reference : http://catlikecoding.com/unity/tutorials/curves-and-splines/
/// </summary>
[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor {

    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private int selectedIndex = -1;

    private const float directionScale = 0.5f;
    private const int stepsPerCurve = 10;
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;

    private static Color[] modeColors =
    {
        Color.white, Color.yellow, Color.cyan,
    };

    private void OnSceneGUI() {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < spline.ControlPointCount; i += 3) {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.grey;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }
        ShowDirections();
    }

    public override void OnInspectorGUI() {
        spline = target as BezierSpline;
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Toggle Loop");
            EditorUtility.SetDirty(spline);
            spline.Loop = loop;
        }
        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount) {
            DrawSelectedPointInspector();
        }
        if (GUILayout.Button("Add Curve")) {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }

    private void DrawSelectedPointInspector() {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoints(selectedIndex));
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, point);
        }
        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)
            EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }
    }

    private void ShowDirections() {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 0; i < steps; i++) {
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }

    

    private Vector3 ShowPoint(int _index) {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoints(_index));
        float size = HandleUtility.GetHandleSize(point);
        if (_index == 0) {
            size *= 2f;
        }
        Handles.color = modeColors[(int) spline.GetControlPointMode(_index)];
        if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap)) {
            selectedIndex = _index;
            Repaint();
        }
        if (selectedIndex == _index) {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(_index, handleTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }
}
