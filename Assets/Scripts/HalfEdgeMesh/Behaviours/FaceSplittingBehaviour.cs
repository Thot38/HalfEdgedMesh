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
  public interface IFaceSplittingBehaviour
  {
    void SplitFace(Face face, Vector3 pointToSplit, bool isTransaction = false);
    void SplitAllFaces();
    void SplitOffFace(Face f, Vector3 force);
    //void SmoothObject(Vector3 center, float radius);

    List<Face> GetCreatedFaces();
    void ClearCreatedFaces();
  }

  public class FaceSplittingBehaviour : IFaceSplittingBehaviour
  {
    private readonly HalfEdgeMesh _mesh;

    public FaceSplittingBehaviour(HalfEdgeMesh mesh)
    {
      _mesh = mesh;

      _createdFaces = new List<Face>();
    }

    /// <summary>
    /// Splits all current faces into three
    /// </summary>
    public void SplitAllFaces()
    {
      var faces = _mesh.Faces.GetCopyOfFaces();

      foreach (var face in faces)
      {
        SplitFace(face, VectorMath.GetFaceCentoid(face), true);
      }
      _mesh.CommitMeshTriangles();
    }

    /// <summary>
    /// Splits a face at the given point into three smaller faces
    /// </summary>
    /// <param name="face">The face to split</param>
    /// <param name="pointToSplit">The point to split the face at</param>
    /// <param name="isTransaction">If [isTransaction] the Unity Mesh will not be commited and  <see cref="HalfEdgeMesh.CommitMeshTriangles"/> has to be called when the transaction is over</param>
    public void SplitFace(Face face, Vector3 pointToSplit, bool isTransaction = false)
    {
      var center = _mesh.Vertices.CreateVertex(pointToSplit);
      _mesh.AddMeshVertex(center.Point);

      var halfEdges = _mesh.Faces.GetFaceCirculator(face);
      var newHalfEdges = new List<HalfEdge>();

      for (int i = 0; i < halfEdges.Count; i++)
      {
        var halfEdge = halfEdges[i];
        var newFace = face;
        if (i == 0)        // --- reuse old face
        {
          newFace.HalfEdge = halfEdge;
        }
        else
        {
          newFace = _mesh.Faces.CreateFace(halfEdge);
        }

        _createdFaces.Add(newFace);

        var newHalfEdgeFromCenter = _mesh.HalfEdges.CreateHalfEdge(center, newFace, halfEdge);
        var newHalfEdgeToCenter = _mesh.HalfEdges.CreateHalfEdge(halfEdge.EndPoint, newFace, newHalfEdgeFromCenter);
        halfEdge.Next = newHalfEdgeToCenter;

        _mesh.SetMeshTriangles(newFace.Index, _mesh.Faces.GetFaceCirculator(newFace).Select(p => p.OutgoingPoint.Index).ToList(), true);
        newHalfEdges.AddRange(new List<HalfEdge> { newHalfEdgeFromCenter, newHalfEdgeToCenter, halfEdge });
      }

      _mesh.HalfEdges.CreatePair(newHalfEdges[0], newHalfEdges[4]);
      _mesh.HalfEdges.CreatePair(newHalfEdges[3], newHalfEdges[7]);
      _mesh.HalfEdges.CreatePair(newHalfEdges[6], newHalfEdges[1]);

      if(isTransaction)
        _mesh.CommitMeshTriangles();
    }

    public void SplitOffFace(Face f, Vector3 force)
    {
      // split face
      // move the in direction of force / along the normal or something
      // Create new HalfEdgeMesh
      //    with a top vertices on the splitted face
      //    faces to make a pyramide with triangular base
      //    Move mesh
      var center = _mesh.Vertices.CreateVertex(VectorMath.GetFaceCentoid(f));
      SplitFace(f, center.Point);
      center.Point += force;

      var go = new GameObject("Splinter");
      go.transform.parent = _mesh.transform;
      var newMesh = go.AddComponent<HalfEdgeMesh>();
      go.AddComponent<SimpleMeshDebugger>();

      newMesh.CreateMesh();
      newMesh.GenerateUnityMesh();

      newMesh.AddFace(newMesh.HalfEdges[0], center.Point);
      newMesh.AddFace(newMesh.HalfEdges[1], center.Point);
      newMesh.AddFace(newMesh.HalfEdges[2], center.Point);

      go.transform.Translate(-5 * force);
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
