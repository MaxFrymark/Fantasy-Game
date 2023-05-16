using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenerateTerrainWithNoiseMap : MapGenerationStage
{
    protected int mapWidth;
    protected int mapHeight;
    
    protected int numberOfNoiseLayers;
    protected float frequencyMin;
    protected float frequencyMax;

    public void SetNoiseMapParameters(int mapWidth, int mapHeight, int numberOfNoiseLayers, float frequencyMin, float frequencyMax)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.numberOfNoiseLayers = numberOfNoiseLayers;
        this.frequencyMin = frequencyMin;
        this.frequencyMax = frequencyMax;
    }
    
    protected List<NoiseWave> GenerateNoiseWaves(int noiseLayers, float frequencyMin, float frequencyMax)
    {
        List<NoiseWave> noiseWaves = new List<NoiseWave>();
        for (int i = 0; i < noiseLayers; i++)
        {
            float frequency = Random.Range(frequencyMin, frequencyMax);
            NoiseWave wave = new NoiseWave(Random.Range(0, 500), frequency, (float)i / noiseLayers);
            noiseWaves.Add(wave);
        }

        return noiseWaves;
    }

    protected float[,] GenerateNoiseMap(List<NoiseWave> waves)
    {
        float[,] noiseMap = new float[mapWidth * 2 + 1, mapHeight * 2 + 1];
        for (int x = 0; x <= mapWidth * 2; x++)
        {
            for (int y = 0; y <= mapHeight * 2; y++)
            {
                float normalization = 0f;
                foreach (NoiseWave wave in waves)
                {
                    noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise(x * wave.frequency + wave.seed, y * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }
                noiseMap[x, y] /= normalization;
            }
        }

        return noiseMap;
    }
}

public class NoiseWave
{
    public float seed;
    public float frequency;
    public float amplitude;

    public NoiseWave(float seed, float frequency, float amplitude)
    {
        this.seed = seed;
        this.frequency = frequency;
        this.amplitude = amplitude;
    }
}
