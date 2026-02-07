// Example C# script
using UnityEngine;
using System.Collections.Generic;

public class MultiLineDrawer : MonoBehaviour
{
    public Transform originPoint;
    public List<Transform> targetPoints;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component not found!");
            return;
        }
    }

    void Update()
    {
        if (originPoint == null || targetPoints == null || targetPoints.Count == 0)
        {
            lineRenderer.positionCount = 0; // Clear lines if no points
            return;
        }

        // Set position count: 1 for origin + number of target points
        lineRenderer.positionCount = 1 + targetPoints.Count;
        lineRenderer.SetPosition(0, originPoint.position);

        for (int i = 0; i < targetPoints.Count; i++)
        {
            if (targetPoints[i] != null)
            {
                // Each line connects from the origin to a target point
                lineRenderer.SetPosition(i + 1, targetPoints[i].position);
            }
        }
    }
}