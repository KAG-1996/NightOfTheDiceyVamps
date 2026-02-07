using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public Vector3 point1; // Assign these in the Inspector or dynamically
    public Vector3 point2;
    public LineRenderer lineRenderer;

    public LineDrawer _goMapTile;
    public List<Transform> _t1, _t2;
    public LineRenderer[] lineRends;


    /*void Start()
    {
        //lineRenderer.positionCount = _t1.Count; // A line between two points 
        lineRends[0].positionCount = _t1.Count; // A line between two points 
        lineRends[1].positionCount = _t2.Count; // A line between two points 
    }*/

    /*void Update()
    { 
         DrawLine(); 
    }*/
    void DrawLine()
    {
        //if (_t1.Count == 0) return; 
        for(int i = 0; i < _t1.Count; ++i) lineRends[0].SetPosition(i, _t1[i].position);
        //if (_t2.Count == 0) return; 
        for(int i = 0; i < _t2.Count; ++i) lineRends[1].SetPosition(i, _t2[i].position);
    }
    /*[ContextMenu("CreatePath")]
    public void CreatePath()
    {
        var obj = Instantiate(_goMapTile); 
        obj._t.Clear();
        obj._t.Add(obj.transform);
        obj._t.Add(this.transform);
        obj.lineRenderer.positionCount = _t.Count;
    }
    public int _connections = 0;*/
    public void CreatePath(LineDrawer line)
    {
        /*_t.Add(line.transform); 
        if (!_t.Contains(this.transform)) _t.Add(this.transform);
        lineRenderer.positionCount = _t.Count;
        DrawLine();*/

        if (!_t1.Contains(this.transform))
        {
            _t1.Add(line.transform);
            _t1.Add(this.transform);
            lineRends[0].positionCount = _t1.Count;
        }
        else
        {
            _t2.Add(line.transform);
            _t2.Add(this.transform);
            lineRends[1].positionCount = _t2.Count;
        }
        DrawLine();
    }
}