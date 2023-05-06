using Assets.HalfEdgeMeshes;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HalfEdgeMeshes.Behaviours
{
  public interface ISmoothingBehaviour
  {
    void Smooth();
  }

  public class SmoothingBehaviour : ISmoothingBehaviour
  {
    private readonly HalfEdgeMesh _mesh;

    private float threshold = Mathf.PI * .4f;
    private float maxDistance = 100;

    public SmoothingBehaviour(HalfEdgeMesh mesh)
    {
      _mesh = mesh;
    }

    public void Smooth()
    {
      Debug.Log("Begin Schmoos");
      var problems = new List<Vertex>();
      var adas = new List<float>();
      foreach (var vertex in _mesh.Vertices)
      {
        var adjacentFaces = _mesh.Vertices.GetVertexCirculator(vertex).Select(p => p.Face).Distinct().ToList();
        for (int i = 0; i < adjacentFaces.Count; i++)
        {
          for (int j = 0; j < adjacentFaces.Count; j++)
          {
            if (i == j)
              continue;
            var a = VectorMath.GetFaceNormal(adjacentFaces[i]);
            var b = VectorMath.GetFaceNormal(adjacentFaces[j]);

            var ada = Mathf.Abs(Mathf.Acos(Vector3.Dot(a, b)));
            adas.Add(ada);
            if (ada > threshold)
            {
              problems.Add(vertex);
              i = j = adjacentFaces.Count;
              continue;
            }
          }
        }
      }
      
      //problems = OrderVerticesByDistance(problems);
      for (int i = 0; i < problems.Count; i++)
      {
        var point = problems[i];
        var previous = point.HalfEdge.EndPoint;
        var next = point.HalfEdge.Previous.OutgoingPoint;

        if(point.HalfEdge.Opposing != null)
        {
          Debug.Log("New section");

          //var right = point.HalfEdge.Opposing.OutgoingPoint;
          var left = point.HalfEdge.Opposing.Next.EndPoint;

          point.Point = 0.7f * point.Point + 0.1f * previous.Point + 0.1f * next.Point + 0.1f * left.Point;
        }
        else
        {
          point.Point = 0.8f * point.Point + 0.1f * previous.Point + 0.1f * next.Point;

        }

        Debug.Log($"NewPoint: {point.Point}");
      }

    }
    private List<Vertex> OrderVerticesByDistance(List<Vertex> vertices)
    {
      var first = vertices.FirstOrDefault();
      return vertices.OrderBy(p => Vector3.Distance(p.Point, first.Point)).ToList();
    }
  }
}
