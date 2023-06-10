using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CityNameGenerator
{
    public static CityNameGenerator instance;

    string filePath = Application.streamingAssetsPath + "/City Names.txt";
    List<string> cityNames;

    public static CityNameGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CityNameGenerator();
            }
            return instance;
        }
    }

    public string GetRandomCityName()
    {
        if(cityNames == null)
        {
            BuildCityNameList();
        }

        if(cityNames.Count == 0)
        {
            return "Out Of Names";
        }

        else
        {
            string cityName = cityNames[Random.Range(0, cityNames.Count)];
            cityNames.Remove(cityName);
            return cityName;
        }
    }

    private void BuildCityNameList()
    {
        cityNames = new List<string>();
        string[] nameArray = File.ReadAllLines(filePath);
        foreach(string cityName in nameArray)
        {
            cityNames.Add(cityName);
        }
    }
}
