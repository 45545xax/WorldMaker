// Erosion Logic
// Original code by Sebastian Lague

// https://www.youtube.com/watch?v=9RHGLZLUuwc
// https://github.com/SebLague/Hydraulic-Erosion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainErosion
{
    public int mapWidth = 512;
    public int mapHeight = 256;
    public ComputeShader erosion;
    public ErosionSettings erosionSettings;

    #region Singleton
    static TerrainErosion myInstance = null;

    TerrainErosion()
    {
    }

    public static TerrainErosion instance
    {
        get
        {
            if (myInstance == null)
                myInstance = new TerrainErosion();
            return myInstance;
        }
    }
    #endregion

    public void Erode(ref float[] map)
    {
        //int mapSizeWithBorder = mapSize + erosionBrushRadius * 2;
        int numThreads = erosionSettings.numErosionIterations / 1024;
        if (numThreads <= 0) numThreads = 1;

        // Create brush
        List<int> brushIndexOffsets = new List<int>();
        List<float> brushWeights = new List<float>();

        float weightSum = 0;
        for (int brushY = -erosionSettings.erosionBrushRadius; brushY <= erosionSettings.erosionBrushRadius; brushY++)
        {
            for (int brushX = -erosionSettings.erosionBrushRadius; brushX <= erosionSettings.erosionBrushRadius; brushX++)
            {
                float sqrDst = brushX * brushX + brushY * brushY;
                if (sqrDst < erosionSettings.erosionBrushRadius * erosionSettings.erosionBrushRadius)
                {
                    brushIndexOffsets.Add(brushY * mapWidth + brushX);
                    float brushWeight = 1 - Mathf.Sqrt(sqrDst) / erosionSettings.erosionBrushRadius;
                    weightSum += brushWeight;
                    brushWeights.Add(brushWeight);
                }
            }
        }
        for (int i = 0; i < brushWeights.Count; i++)
        {
            brushWeights[i] /= weightSum;
        }

        // Send brush data to compute shader
        ComputeBuffer brushIndexBuffer = new ComputeBuffer(brushIndexOffsets.Count, sizeof(int));
        ComputeBuffer brushWeightBuffer = new ComputeBuffer(brushWeights.Count, sizeof(int));
        brushIndexBuffer.SetData(brushIndexOffsets);
        brushWeightBuffer.SetData(brushWeights);
        erosion.SetBuffer(0, "brushIndices", brushIndexBuffer);
        erosion.SetBuffer(0, "brushWeights", brushWeightBuffer);

        // Generate random indices for droplet placement
        int[] randomIndices = new int[erosionSettings.numErosionIterations];
        for (int i = 0; i < erosionSettings.numErosionIterations; i++)
        {
            int randomX = Random.Range(0, mapWidth);
            int randomY = Random.Range(0, mapHeight);
            randomIndices[i] = randomY * mapWidth + randomX;
        }

        // Send random indices to compute shader
        ComputeBuffer randomIndexBuffer = new ComputeBuffer(randomIndices.Length, sizeof(int));
        randomIndexBuffer.SetData(randomIndices);
        erosion.SetBuffer(0, "randomIndices", randomIndexBuffer);

        // Heightmap buffer
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
        mapBuffer.SetData(map);
        erosion.SetBuffer(0, "map", mapBuffer);

        // Settings
        erosion.SetInt("mapWidth", mapWidth);
        erosion.SetInt("mapHeight", mapHeight);
        erosion.SetInt("brushLength", brushIndexOffsets.Count);
        erosion.SetInt("maxLifetime", erosionSettings.maxLifetime);
        erosion.SetFloat("inertia", erosionSettings.inertia);
        erosion.SetFloat("sedimentCapacityFactor", erosionSettings.sedimentCapacityFactor);
        erosion.SetFloat("minSedimentCapacity", erosionSettings.minSedimentCapacity);
        erosion.SetFloat("depositSpeed", erosionSettings.depositSpeed);
        erosion.SetFloat("erodeSpeed", erosionSettings.erodeSpeed);
        erosion.SetFloat("evaporateSpeed", erosionSettings.evaporateSpeed);
        erosion.SetFloat("gravity", erosionSettings.gravity);
        erosion.SetFloat("startSpeed", erosionSettings.startSpeed);
        erosion.SetFloat("startWater", erosionSettings.startWater);

        // Run compute shader
        erosion.Dispatch(0, numThreads, 1, 1);
        mapBuffer.GetData(map);

        // Release buffers
        mapBuffer.Release();
        randomIndexBuffer.Release();
        brushIndexBuffer.Release();
        brushWeightBuffer.Release();
    }
}
