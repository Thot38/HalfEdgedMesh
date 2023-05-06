using Assets.HalfEdgeMeshes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Benchmark
{
  public class Benchmark
  {
    public void StartBenchmark()
    {
      var csvTimes = new List<string>();
      csvTimes.Add("Action");
      csvTimes.Add("Number of vertices");
      csvTimes.Add("Time in ms");
      csvTimes.Add("\r\n");

      for (int f = 0; f < 4; f++)
      {
        Mesh teapot;
        switch (f)
        {
          default:
          case 0: teapot = Resources.Load(@"Prefabs\Teapot") as Mesh;
            break;
          case 1: teapot = Resources.Load(@"Prefabs\Teapot2") as Mesh;
            break;
          case 2: teapot = Resources.Load(@"Prefabs\Teapot5") as Mesh;
            break;
          case 3: teapot = Resources.Load(@"Prefabs\Teapot10") as Mesh;
            break;
        }

        var x = GameObject.Instantiate(teapot);

        // --- Split HalfEdge
        for (int i = 0; i < 25; i++)
        {
          var go = new GameObject("BenchmarkHE");

          var halfEdge = go.AddComponent<HalfEdgeMesh>();
          halfEdge.BuildHalfEdgeMesh(x);
          csvTimes.Add("SplitAllHalfEdge HalfEdgeMesh");
          csvTimes.Add($"{x.vertices.Count()}");

          using (var stopwatch = new Stopwatch(csvTimes))
          {
            var count = halfEdge.HalfEdges.Count();
            for (int j = 0; j < count; j++)
            {
              halfEdge.SplitHalfEdge(j);
            }
          }
          csvTimes.Add($"{Environment.NewLine}");
          GameObject.DestroyImmediate(go);
        }

        // --- Edge Collapse
        for (int i = 0; i < 25; i++)
        {
          var go = new GameObject("BenchmarkHE");

          var halfEdge = go.AddComponent<HalfEdgeMesh>();
          halfEdge.BuildHalfEdgeMesh(x);
          csvTimes.Add("EdgeCollapse HalfEdgeMesh");
          csvTimes.Add($"{x.vertices.Count()}");

          using (var stopwatch = new Stopwatch(csvTimes))
          {
            halfEdge.EdgeCollapse(halfEdge.HalfEdges.FirstOrDefault(p => p.Opposing != null).Index);
          }
          csvTimes.Add($"{Environment.NewLine}");
          GameObject.DestroyImmediate(go);
        }

        // --- Subdivision

        for (int i = 0; i < 25; i++)
        {
          var go = new GameObject("BenchmarkHE");

          var halfEdge = go.AddComponent<HalfEdgeMesh>();
          halfEdge.BuildHalfEdgeMesh(x);
          csvTimes.Add("Subdivision HalfEdgeMesh");
          csvTimes.Add($"{x.vertices.Count()}");

          using (var stopwatch = new Stopwatch(csvTimes))
          {
            halfEdge.SplitAllFaces();
          }
          csvTimes.Add($"{Environment.NewLine}");
          GameObject.DestroyImmediate(go);
        }

        using (var fs = new FileStream($"{ Environment.CurrentDirectory }\\Benchmark.csv", FileMode.OpenOrCreate))
        {
          using (var sw = new StreamWriter(fs))
          {
            sw.Write(string.Join(";", csvTimes));
            sw.Flush();
          }
        }
      }
    }

  }
}
