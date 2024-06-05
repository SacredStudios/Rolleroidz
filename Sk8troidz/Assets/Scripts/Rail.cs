using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;

public class Rail : MonoBehaviour
{


    public SplineContainer rail_spline;
    public float length;
    void Start()
    {
        rail_spline = GetComponent<SplineContainer>();
        length = rail_spline.CalculateLength();
    }

    public Vector3 LocalToWorld(float3 point)
    {
        return transform.TransformPoint(point);
    }
    public float3 WorldToLocal(Vector3 point)
    {
        return transform.InverseTransformPoint(point);
    }
    public float CalcTargetRailPoint(Vector3 player_pos, out Vector3 world_spline_pos)
    {
        float3 nearest_point;
        float time;
        SplineUtility.GetNearestPoint(rail_spline.Spline, WorldToLocal(player_pos), out nearest_point, out time);
        world_spline_pos = LocalToWorld(nearest_point);
        return time;
    }
    public bool CalcDirection(float3 rail_fwd, Vector3 player_fwd)
    {
    float angle = Vector3.Angle(rail_fwd, player_fwd.normalized);
        if (angle > 90f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
