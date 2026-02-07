using UnityEngine;

public class ManagerColor : MonoBehaviour
{
    public Color _globalColor;

    public SpriteRenderer[] _spriteRends;
    public Material[] _materials;
    public Camera _camera;
    public Terrain terrain;

    private Color _initColor = Color.red;
    void Update()
    {
        foreach (var a in _spriteRends) a.color = _globalColor; 
        foreach (var a in _materials) a.color = _globalColor;
        _camera.backgroundColor = _globalColor;

        TerrainData terrainData = terrain.terrainData;
        DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;
        foreach (DetailPrototype a in detailPrototypes)
        {
            a.healthyColor = _globalColor;
            a.dryColor = _globalColor;
        }
        terrainData.detailPrototypes = detailPrototypes; // Apply the modified prototype back to the TerrainData
    }
    /*private void OnDestroy()
    {
        foreach (var a in _materials) a.color = _initColor;
        foreach (var a in _spriteRends) a.color = _initColor;
    }*/
}
