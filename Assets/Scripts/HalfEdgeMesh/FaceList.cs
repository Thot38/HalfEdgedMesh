using Assets.Scripts.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Assets.HalfEdgeMeshes
{
  public class FaceList : IEnumerable<Face>
  {
    private readonly List<Face> _faces;
    private readonly HalfEdgeMesh _mesh;

    public FaceList(HalfEdgeMesh mesh)
    {
      _faces = new List<Face>();
      _mesh = mesh;
    }

    public FaceList(HalfEdgeMesh mesh, List<Face> faces)
    {
      _faces = faces;
      _mesh = mesh;
    }

    public int Count => _faces.Count;

    #region Methods

    /// <summary>
    /// Creates a new Face adjacent to the half edge. Sets the reference to the halfEdge
    /// </summary>
    /// <param name="adjacentHalfEdge"></param>
    /// <returns>The created face</returns>
    public Face CreateFace(HalfEdge adjacentHalfEdge)
    {
      var face = new Face(adjacentHalfEdge, _faces.Count);
      adjacentHalfEdge.Face = face;
      _faces.Add(face);
      return face;
    }

    public Face this[int index]
    {
      get => _faces[index];
      private set => _faces[index] = value;
    }

    public List<Face> GetCopyOfFaces()
    {
      return new List<Face>(_faces);
    }

    public List<HalfEdge> GetFaceCirculator(Face f)
    {
      var result = new List<HalfEdge>();
      result.Add(f.HalfEdge);
      result.Add(f.HalfEdge.Previous);
      result.Add(f.HalfEdge.Previous.Previous);
      return result;
    }

    public void RemoveFace(Face f)
    {
      var halfEdges = GetFaceCirculator(f);
      foreach (var halfEdge in halfEdges)
      {
        if (halfEdge.Opposing != null)
          halfEdge.Opposing.Opposing = null;

        if(_mesh.Vertices.GetVertexCirculator(halfEdge.OutgoingPoint).Count <= 0)
        {
          _mesh.Vertices.RemoveVertex(halfEdge.OutgoingPoint);
        }
      }
      _mesh.HalfEdges.RemoveAll(p => halfEdges.Contains(p));

      _faces.Remove(f);
      //_mesh.RemoveMeshTriangle(f.Index);
    }

    #endregion

    #region Enumerator stuff

    public IEnumerator<Face> GetEnumerator()
    {
      return _faces.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _faces.GetEnumerator();
    }

    public bool TryGetFaceContainingPoint(Vector3 point, out Face outFace)
    {
      foreach(var face in _faces)
      {
        if (VectorMath.IsPointOnFace(face, point))
        {
          outFace = face;
          return true;
        }
      }
      outFace = null;
      return false;
    }

    #endregion
  }
}
