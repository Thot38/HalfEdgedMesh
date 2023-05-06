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
  public interface IEdgeSplittingBehaviour
  {
    void SplitHalfEdge(int index, Vector3 splitPoint);
    void SplitHalfEdge(HalfEdge split, Vector3 splitPoint);
    void Smooth();
    void EdgeCollapse(HalfEdge halfEdge);
    List<Face> GetCreatedFaces();
    void ClearCreatedFaces();
  }

  public class EdgeSplittingBehaviour : IEdgeSplittingBehaviour
  {
    private readonly HalfEdgeMesh _mesh;

    public EdgeSplittingBehaviour(HalfEdgeMesh mesh)
    {
      _mesh = mesh;

      _createdFaces = new List<Face>();
    }

    public void SplitHalfEdge(int index, Vector3 splitPoint)
    {
      SplitHalfEdge(_mesh.HalfEdges[index], splitPoint);
    }

    private bool _lock = false;
    private HalfEdge _newHalfEdge, _split;

    public void SplitHalfEdge(HalfEdge split, Vector3 splitPoint)
    {
      split.Face.HalfEdge = split; // --- Set Reference of Face to split to know where the new face goes
      _split = split;

      var newPoint = _mesh.Vertices.CreateVertex(splitPoint);

      var newHalfEdge = _mesh.HalfEdges.CreateHalfEdge(split.OutgoingPoint, null, split);
      _newHalfEdge = newHalfEdge;
      var newFace = _mesh.Faces.CreateFace(newHalfEdge);
      split.OutgoingPoint = newPoint;

      _createdFaces.Add(split.Face);
      _createdFaces.Add(newFace);

      var newHalfEdgeToSplit = _mesh.HalfEdges.CreateHalfEdge(split.Next.EndPoint, split.Face, split);
      var newHalfEdgeFromNewHalfEdge = _mesh.HalfEdges.CreateHalfEdge(newPoint, newFace, split.Next.Next);
      _mesh.HalfEdges.CreatePair(newHalfEdgeToSplit, newHalfEdgeFromNewHalfEdge);

      newHalfEdge.Next = newHalfEdgeFromNewHalfEdge;
      newHalfEdgeFromNewHalfEdge.Next.Next = newHalfEdge;
      split.Next.Next = newHalfEdgeToSplit;

      _mesh.AddMeshVertex(newPoint.Point);
      _mesh.SetMeshTriangles(split.Face.Index, _mesh.Faces.GetFaceCirculator(split.Face).Select(p => p.OutgoingPoint.Index).ToList(), true);
      _mesh.SetMeshTriangles(newFace.Index, _mesh.Faces.GetFaceCirculator(newFace).Select(p => p.OutgoingPoint.Index).ToList(), true);
      if (!_lock)
      {
        if (split.Opposing != null)
        {
          _lock = true;
          SplitHalfEdge(split.Opposing, splitPoint);
          _mesh.HalfEdges.CreatePair(_split, newHalfEdge);
          _mesh.HalfEdges.CreatePair(split, _newHalfEdge);
          _lock = false;
        }
        _mesh.CommitMeshTriangles();
      }
    }

    public void Smooth()
    {
      var faces = _mesh.Faces.GetCopyOfFaces();
      //var face = faces[0];
      foreach (var face in faces)
      {
        foreach (var halfEdge in _mesh.Faces.GetFaceCirculator(face))
        {
          var newPoint = ((halfEdge.EndPoint.Point + halfEdge.OutgoingPoint.Point) * .5f).normalized;
          SplitHalfEdge(halfEdge, newPoint);
        }
      }
    }

    /// <summary>
    /// Collapses an Edge to the Endpoint of the HalfEdge
    /// </summary>
    /// <param name="halfEdge">The HalfEdge to collapse</param>
    public void EdgeCollapse(HalfEdge halfEdge)
    {
      // --- the point the halfEdge is collapsing to
      var collapsingPoint = halfEdge.EndPoint;
      var pointToRemove = halfEdge.OutgoingPoint;

      // --- determine the halfEdges to remove
      // --- those are the HalfEdges around the halfEdge Face and it's pairs face
      var pair = halfEdge.Opposing;

      var halfEdgesToRemove = _mesh.Faces.GetFaceCirculator(halfEdge.Face);
      halfEdgesToRemove.AddRange(_mesh.Faces.GetFaceCirculator(pair.Face));

      // --- set the neighbours right
      foreach(var he in halfEdgesToRemove)
      {
        if (he.Index == halfEdge.Index || he.Index == pair.Index || he.Next.Index == halfEdge.Index || he.Next.Index == pair.Index
          || he.Opposing == null || he.Next.Opposing == null)
          continue;

        _mesh.HalfEdges.CreatePair(he.Opposing, he.Next.Opposing);
      }

      _mesh.Faces.RemoveFace(halfEdge.Face);
      _mesh.Faces.RemoveFace(pair.Face);

      foreach (var x in _mesh.Vertices.GetVertexCirculator(pointToRemove))
      {
        if(x.OutgoingPoint.Index == pointToRemove.Index)
        {
          x.OutgoingPoint = collapsingPoint;
        }
      }

      _mesh.Vertices.RemoveVertex(pointToRemove);
      _mesh.GenerateUnityMesh();
    }

    private List<Face> _createdFaces;
    public List<Face> GetCreatedFaces()
    {
      return _createdFaces;
    }

    public void ClearCreatedFaces()
    {
      _createdFaces = new List<Face>();
    }
  }
}
