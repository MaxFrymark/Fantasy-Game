using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenerateTerrainWithNoiseMap : MapGenerationStage
{
    protected int numberOfNoiseLayers;
    protected float frequencyMin;
    protected float frequencyMax;
    protected float[] terrainThreshholds;

    public GenerateTerrainWithNoiseMap(int mapWidth, int mapHeight, int numberOfNoiseLayers, float frequencyMin, float frequencyMax, float[] terrainThreshholds) : base(mapWidth, mapHeight)
    {
        this.numberOfNoiseLayers = numberOfNoiseLayers;
        this.frequencyMin = frequencyMin;
        this.frequencyMax = frequencyMax;
        this.terrainThreshholds = terrainThreshholds;
    }
    
    protected List<NoiseWave> GenerateNoiseWaves()
    {
        List<NoiseWave> noiseWaves = new List<NoiseWave>();
        for (int i = 0; i < numberOfNoiseLayers; i++)
        {
            float frequency = Random.Range(frequencyMin, frequencyMax);
            NoiseWave wave = new NoiseWave(Random.Range(0, 500), frequency, (float)i / numberOfNoiseLayers);
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
