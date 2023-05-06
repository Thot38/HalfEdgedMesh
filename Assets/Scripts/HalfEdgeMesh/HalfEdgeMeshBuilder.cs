using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.HalfEdgeMeshes
{
  public interface IHalfEdgeMeshBuilder
  {
    void BuildHalfEdgeFromUnityMesh(Mesh mesh);
  }

  public class HalfEdgeMeshBuilder : IHalfEdgeMeshBuilder
  {
    private HalfEdgeMesh _mesh;

    public HalfEdgeMeshBuilder(HalfEdgeMesh mesh)
    {
      _mesh = mesh;
    }

    public void BuildHalfEdgeFromUnityMesh(Mesh mesh)
    {
      var vertices = mesh.vertices;
      var triangles = mesh.triangles;
      // --- Add vertices to HalfEdgeMesh
      // --- 
      var indexChanges = new int[vertices.Length];
      for (int i = 0; i < vertices.Length; i++)
      {
        var point = vertices[i];
        var vertex = _mesh.Vertices.CreateVertex(point);

        indexChanges[i] = vertex.Index;
      }

      if (indexChanges.Any())
      {
        for (int i = 0; i < triangles.Length; i++)
        {
          triangles[i] = indexChanges[triangles[i]];
        }
      }

      //--- For every three indices in triangles (= a face) add a face and three halfEdges
      for (int i = 0; i < triangles.Length - 3; i += 3)
      {
        var point1 = _mesh.Vertices[triangles[i]];
        var point2 = _mesh.Vertices[triangles[i + 1]];
        var point3 = _mesh.Vertices[triangles[i + 2]];
        var indices = new[] { point1.Index, point2.Index, point3.Index };
        var pairs = _mesh.HalfEdges.Where(p => indices.Contains(p.OutgoingPoint.Index) && indices.Contains(p.EndPoint.Index));

        var halfEdge1 = _mesh.HalfEdges.CreateHalfEdge(point1, null, null);

        var face = _mesh.Faces.CreateFace(halfEdge1);

        var halfEdge2 = _mesh.HalfEdges.CreateHalfEdge(point2, face, halfEdge1);
        var halfEdge3 = _mesh.HalfEdges.CreateHalfEdge(point3, face, halfEdge2);
        halfEdge1.Next = halfEdge3;

        foreach (var pair in pairs)
        {
          if (pair.OutgoingPoint.Index == halfEdge1.EndPoint.Index && pair.EndPoint.Index == halfEdge1.OutgoingPoint.Index)
            _mesh.HalfEdges.CreatePair(pair, halfEdge1);
          else if (pair.OutgoingPoint.Index == halfEdge2.EndPoint.Index && pair.EndPoint.Index == halfEdge2.OutgoingPoint.Index)
            _mesh.HalfEdges.CreatePair(pair, halfEdge2);
          else if (pair.OutgoingPoint.Index == halfEdge3.EndPoint.Index && pair.EndPoint.Index == halfEdge3.OutgoingPoint.Index)
            _mesh.HalfEdges.CreatePair(pair, halfEdge3);
        }

      }
      _mesh.GenerateUnityMesh();
      Debug.Log($"Mesh created form {mesh.vertices.Length} Vertices and {mesh.triangles.Length} Indices and turned them into {_mesh.HalfEdges.Count} HalfEdges");
    }
  }
}
