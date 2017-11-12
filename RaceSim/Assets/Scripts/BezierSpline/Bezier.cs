using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier {

    public static Vector3 GetPoint(Vector3 _p0, Vector3 _p1, Vector3 _p2, float _t)
    {
        _t = Mathf.Clamp01(_t);
        float oneMinusT = 1f - _t;
        // B(t) = (1 - t)2 P0 + 2 (1 - t) t P1 + t2 P2
        return oneMinusT * oneMinusT * _p0 + 2f * oneMinusT * _t * _p1 + _t * _t * _p2;
        // Use the quadratic formula instead of three calls to Vector3.Lerp.
        // return Vector3.Lerp(Vector3.Lerp(_p0, _p1, _t), Vector3.Lerp(_p1, _p2, _t), _t);
    }

    public static Vector3 GetPoint(Vector3 _p0, Vector3 _p1, Vector3 _p2, Vector3 _p3, float _t) {
        _t = Mathf.Clamp01(_t);
        float oneMinusT = 1f - _t;
        // B(t) = (1 - t)3 P0 + 3 (1 - t)2 t P1 + 3 (1 - t) t2 P2 + t3 P3
        return oneMinusT * oneMinusT * oneMinusT * _p0 + 
            3f * oneMinusT * oneMinusT * _t * _p1 + 
            3f * oneMinusT* _t * _t * _p2 + 
            _t * _t * _t * _p3;
    }

    public static Vector3 GetFirstDerivative(Vector3 _p0, Vector3 _p1, Vector3 _p2, float _t)
    {
        // The first derivative of our quadratic Beziér curve is 
        // B'(t) = 2 (1 - t) (P1 - P0) + 2 t (P2 - P1)
        return 2f * (1f - _t) * (_p1 - _p0) + 2f * _t * (_p2 - _p1);
    }

    public static Vector3 GetFirstDerivative(Vector3 _p0, Vector3 _p1, Vector3 _p2, Vector3 _p3, float _t) {
        _t = Mathf.Clamp01(_t);
        float oneMinusT = 1f - _t;
        return 3f * oneMinusT * oneMinusT * (_p1 - _p0) + 
            6f * oneMinusT * _t * (_p2 - _p1) + 
            3f * _t * _t * (_p3 - _p2);
    }


}
