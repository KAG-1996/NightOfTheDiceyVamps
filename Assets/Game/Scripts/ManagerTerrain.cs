using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTerrain : MonoBehaviour
{
    public Terrain terrain;
    public int detailPrototypeIndex = 0; // Index of the grass texture in the TerrainData's detailPrototypes array
    public float newNoiseSpreadValue = 5.0f; // The desired noise spread value

    private void Start()
    {
        NewSeeds();
    }
    [ContextMenu("NEW SEED")]
    void NewSeed()
    {
        if (terrain == null) return;
        int rnd = UnityEngine.Random.Range(0, 1000);

        TerrainData terrainData = terrain.terrainData;
        if (terrainData.detailPrototypes.Length <= detailPrototypeIndex) return; 

        // Get the existing DetailPrototype
        DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;
        DetailPrototype grassPrototype = detailPrototypes[detailPrototypeIndex];

        //grassPrototype.noiseSpread = newNoiseSpreadValue; // Modify the Noise Spread value
        grassPrototype.noiseSeed = rnd; // Modify the Noise Spread value
        terrainData.detailPrototypes = detailPrototypes; // Apply the modified prototype back to the TerrainData

        //ZDebug.Log($"Noise Spread for detail prototype at index {detailPrototypeIndex} set to {newNoiseSpreadValue}");
        ZDebug.Log($"Noise Spread for detail prototype at index {detailPrototypeIndex} set to {rnd} l {terrainData.detailPrototypes.Length}");
    }
    [ContextMenu("NEW SEEDS")]
    void NewSeeds()
    {
        if (terrain == null) return;
        int rnd = UnityEngine.Random.Range(0, 1000);

        TerrainData terrainData = terrain.terrainData;
        //if (terrainData.detailPrototypes.Length <= detailPrototypeIndex) return; 

        // Get the existing DetailPrototype
        DetailPrototype[] detailPrototypes = terrainData.detailPrototypes;
        foreach ( DetailPrototype a in detailPrototypes) a.noiseSeed = rnd; // Apply to each seed

        terrainData.detailPrototypes = detailPrototypes; // Apply the modified prototype back to the TerrainData

        //ZDebug.Log($"Noise Spread for detail prototype at index {detailPrototypeIndex} set to {newNoiseSpreadValue}");
        ZDebug.Log($"Noise Seed for detail prototype is set to {rnd}");
    }
}
