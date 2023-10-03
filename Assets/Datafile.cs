using System;
using System.IO;
using UnityEngine;

public class DataFile : MonoBehaviour
{
    private string _docPath;
    public string[] data;

    private void Start()
    {
        _docPath = Environment.CurrentDirectory + "//DataFile.txt";

        WriteFile();
        //ReadFile();
    }

    private void WriteFile()
    {
        string[] vertices =
        {
            "0, 0, 0",
            "0, 0.11f, 0.5191f",
            "0.5191f, 0, 0.5191f",
            "0.5191f, 0.21f, 0",
            "0, 0, 1.0382f",
            "0.5447f, 0.13f, 1.0688f",
    };

        string[] indices =
        {
            "0 1 2",
            "0 2 3",
            "4 2 1",
            "4 5 2"
        };

        string[] neighbours =
        {
            "1 -1 -1",
            "0 2 -1",
            "1 3 -1",
            "2 -1 -1"
        };

        using StreamWriter outputFile = new StreamWriter(Path.Combine(_docPath));

        // Adding the strings into the txt file
        {
            foreach (var vertex in vertices)
                outputFile.WriteLine(vertex);

            outputFile.WriteLine(string.Empty);

            foreach (var index in indices)
                outputFile.WriteLine(index);

            outputFile.WriteLine(string.Empty);

            foreach (var neighbour in neighbours)
                outputFile.WriteLine(neighbour);
        }
    }

    private void ReadFile()
    {
        int i = 0;

        using var sr = new StreamReader(_docPath);
        while (sr.Peek() >= 0)
        {
            //data[i] = sr.ReadLine();
            //Debug.Log(data[i]);
            i++;
        }
    }
}