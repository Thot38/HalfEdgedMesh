using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Utilities;
using System;
using Assets.Scripts.HalfEdgeMeshes.BreakingBehaviourDeterminer;
using Assets.Scripts.HalfEdgeMeshes.Behaviours;

namespace Assets.HalfEdgeMeshes
{
  public class HalfEdgeMesh : MonoBehaviour
  {
    public Vector3 NextV = new Vector3(2, 1, 2);
    public int Next = 0;

    public MeshFilter meshFilter;

    public MeshRenderer meshRenderer;
    public Mesh mesh;

    private readonly IFaceSplittingBehaviour _faceSplittingBehaviour;
    private readonly IEdgeSplittingBehaviour _edgeSplittingBehaviour;
    private readonly ISmoothingBehaviour _smoothingBehaviour;
    private readonly IHalfEdgeMeshBuilder _halfEdgeMeshBuilder;

    public HalfEdgeMesh()
    {
      _faceSplittingBehaviour = new FaceSplittingBehaviour(this);
      _edgeSplittingBehaviour = new EdgeSplittingBehaviour(this);
      _smoothingBehaviour = new SmoothingBehaviour(this);
      _halfEdgeMeshBuilder = new HalfEdgeMeshBuilder(this);
    }

    #region Unity Mesh Hickhak

    public void GenerateUnityMesh()
    {
      meshFilter = gameObject.GetComponent<MeshFilter>();
      if (meshFilter == null)
        meshFilter = gameObject.AddComponent<MeshFilter>();

      mesh = meshFilter.sharedMesh;
      if (mesh == null)
        mesh = new Mesh() { name = "HalfEdgeMesh" };

      meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
      if (meshRenderer == null)
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

      ClearMesh();
      // --- Add vertices
      var vertices = Vertices.Select(p => p.Point).ToList();

      // --- Add triangles
      foreach (var face in Faces)
      {
        var adjacentHalfEdges = Faces.GetFaceCirculator(face).ToList();
        SetMeshTriangles(face.Index, adjacentHalfEdges.Select(p => p.OutgoingPoint.Index).ToList(), true);
      }

      AddMeshVertices(vertices);
      CommitMeshTriangles();

      meshFilter.sharedMesh = mesh;

    }

    public void ClearMesh()
    {
      mesh.Clear();
      _meshVertices = new List<Vector3>();
      _meshTriangles = new Dictionary<int, List<int>>();
    }

    public void BuildHalfEdgeMesh(Mesh mesh)
    {
      _meshVertices = new List<Vector3>();
      _meshTriangles = new Dictionary<int, List<int>>();
      _halfEdgeMeshBuilder.BuildHalfEdgeFromUnityMesh(mesh);
    }

    #region MeshVertices
    private List<Vector3> _meshVertices;
    public void AddMeshVertices(List<Vector3> vertices)
    {
      _meshVertices.AddRange(vertices);
      mesh.SetVertices(_meshVertices);
    }

    public void SetMeshVertex(int index, Vector3 vertex)
    {
      _meshVertices[index] = vertex;
      mesh.SetVertices(_meshVertices);
    }

    public void AddMeshVertex(Vector3 vertex)
    {
      if (_meshVertices.Contains(vertex))
        return;
      _meshVertices.Add(vertex);
      mesh.SetVertices(_meshVertices);
    }

    public void RemoveMeshVertex(Vector3 vertex)
    {
      var index = _meshVertices.IndexOf(vertex);

      _meshVertices.Remove(vertex);

      foreach (var kvp in _meshTriangles)
      {
        for (int i = 0; i < kvp.Value.Count; i++)
        {
          if (kvp.Value[i] >= index)
            kvp.Value[i]--;
        }
      }

      mesh.SetVertices(_meshVertices);
    }


    #endregion MeshVertices

    #region MeshTriangles
    private Dictionary<int, List<int>> _meshTriangles;
    /// <summary>
    /// Sets the index Array of the unity mesh
    /// </summary>
    /// <param name="index">Index of the Face</param>
    /// <param name="triangles">The indices of the points of the face</param>
    /// <param name="isTransaction">if IsTransaction, it will not set the unity mesh array it self</param>
    public void SetMeshTriangles(int index, List<int> triangles, bool isTransaction = false)
    {
      _meshTriangles[index] = triangles;
      if (!isTransaction)
        CommitMeshTriangles();
    }

    public void RemoveMeshTriangle(int index)
    {
      _meshTriangles[index] = new List<int>();
      CommitMeshTriangles();
    }

    /// <summary>
    /// Commits the mesh triangle changes to the unity mesh
    /// </summary>
    public void CommitMeshTriangles()
    {
      mesh.triangles = _meshTriangles.Values.SelectMany(p => p).ToArray();
    }

    #endregion MeshTriangles

    #endregion

    #region List datas

    private FaceList _faceList;
    public FaceList Faces
    {
      get => _faceList ?? (_faceList = new FaceList(this));
      set => _faceList = value;
    }

    private HalfEdgeList _halfEdges;
    public HalfEdgeList HalfEdges
    {
      get => _halfEdges ?? (_halfEdges = new HalfEdgeList(this));
      set => _halfEdges = value;
    }

    private VertexList _vertices;
    public VertexList Vertices
    {
      get => _vertices ?? (_vertices = new VertexList(this));
      set => _vertices = value;
    }
    #endregion

    #region for MeshCreation

    /// <summary>
    /// To create a new Mesh the CreateMesh Method creates a simple triangle
    /// </summary>
    public void CreateMesh()
    {
      var a = new Vector3(0, 0, 1);
      var b = new Vector3(1, 0, -1);
      var c = new Vector3(-1, 0, -1);

      CreateMesh(a, b, c);
    }

    public void CreateMesh(Vector3 va, Vector3 vb, Vector3 vc)
    {
      var a = Vertices.CreateVertex(va);
      var b = Vertices.CreateVertex(vb);
      var c = Vertices.CreateVertex(vc);

      var heA = HalfEdges.CreateHalfEdge(a, null, null);
      var face = Faces.CreateFace(heA);
      var heB = HalfEdges.CreateHalfEdge(b, face, heA);
      var heC = HalfEdges.CreateHalfEdge(c, face, heB);
      heA.Next = heC;

      GenerateUnityMesh();
    }

    /// <summary>
    /// Creates a everything that is necessarry to have a new face, opposite of the given halfEdge, spanning the given point
    /// </summary>
    /// <param name="opposingHalfEdge"></param>
    /// <param name="point"></param>
    public void AddFace(HalfEdge opposingHalfEdge, Vector3 point)
    {
      if (opposingHalfEdge.Opposing != null)
        return;

      var a = opposingHalfEdge.OutgoingPoint;
      var c = opposingHalfEdge.EndPoint;
      var d = Vertices.CreateVertex(point);

      var halfEdge1 = HalfEdges.CreateHalfEdge(c, null, null);
      HalfEdges.CreatePair(halfEdge1, opposingHalfEdge);
      var face = Faces.CreateFace(halfEdge1);
      var halfEdge2 = HalfEdges.CreateHalfEdge(d, face, halfEdge1);
      var halfEdge3 = HalfEdges.CreateHalfEdge(a, face, halfEdge2);
      halfEdge1.Next = halfEdge3;

      // extend the half mesh
      AddMeshVertex(d.Point);
      SetMeshTriangles(face.Index, Faces.GetFaceCirculator(face).Select(p => p.OutgoingPoint.Index).ToList());
    }

    public void CreateLandscape()
    {
      CreateMesh();

      for (int i = 0; i < 1; i++)
      {
        var halfEdgesToLookAt = HalfEdges.Where(p => p.Opposing == null).ToList();
        foreach (var he in halfEdgesToLookAt)
        {
          var newPoint = VectorMath.GetIntermediateVector(he.OutgoingPoint.Point, he.EndPoint.Point);
          newPoint = newPoint * 1.5f * i;
          newPoint.y = UnityEngine.Random.Range(-1, 1);

          AddFace(he, newPoint);
        }
      }
    }

    #endregion

    #region Fun with Meshes

    public void RemoveAFace(Face face)
    {
      Faces.RemoveFace(face);
      GenerateUnityMesh();
    }

    public void SplitAllFaces()
    {
      _faceSplittingBehaviour.SplitAllFaces();
    }

    public void SplitOffFace(Face f, Vector3 force)
    {
      _faceSplittingBehaviour.SplitOffFace(f, force);
    }

    public void SmoothObject()
    {
      _smoothingBehaviour.Smooth();
    }

    public void EdgeCollapse(int i)
    {
      _edgeSplittingBehaviour.EdgeCollapse(_halfEdges[i]);
    }


    public void SimulateBreaking(Vector3 impactPoint)
    {
      // 1. find face to start breaking
      // 2. get Points where it should break
      // foreach point:
      //    3. Split mesh across those points
      //    4. Split newly created halfedges at random position(s)
      if (Faces.TryGetFaceContainingPoint(impactPoint, out var impactedFace))
      {
        var bbd = new BreakingBehaviourDeterminer();

        var breakpoints = bbd.GetBreakingPoints(impactedFace, impactPoint);
        var halfEdges = Faces.GetFaceCirculator(impactedFace).ToList();
        _faceSplittingBehaviour.SplitFace(impactedFace, impactPoint);

        foreach (var oldHe in halfEdges)
        {
          _edgeSplittingBehaviour.SplitHalfEdge(oldHe, VectorMath.GetIntermediateVector(oldHe.OutgoingPoint.Point, oldHe.EndPoint.Point));
        }

        foreach (var point in breakpoints)
        {
          if (Faces.TryGetFaceContainingPoint(point, out var face))
          {
            var oldHalfEdgesOfFace = Faces.GetFaceCirculator(face).Select(p => p.Next);
            _faceSplittingBehaviour.SplitFace(face, point);

            //foreach (var oldHe in oldHalfEdgesOfFace)
            //{
            //  _edgeSplittingBehaviour.SplitHalfEdge(oldHe, VectorMath.GetIntermediateVector(oldHe.OutgoingPoint.Point, oldHe.EndPoint.Point));
            //}
          }
        }

        var faces = _edgeSplittingBehaviour.GetCreatedFaces();
        faces.AddRange(_faceSplittingBehaviour.GetCreatedFaces());

        foreach (var face in faces)
        {
          var go = new GameObject("Shard");

          var mesh = go.AddComponent<HalfEdgeMesh>();
          go.AddComponent<Rigidbody>().useGravity = true;
          

          var createdHalfEdges = Faces.GetFaceCirculator(face).ToList();
          var vertices = createdHalfEdges.Select(p => p.OutgoingPoint).ToList();

          mesh.CreateMesh(vertices[0].Point, vertices[1].Point, vertices[2].Point);
          Faces.RemoveFace(face);
        }

        _edgeSplittingBehaviour.ClearCreatedFaces();
        _faceSplittingBehaviour.ClearCreatedFaces();
      }
    }

    public void SplitHalfEdge(int index)
    {
      _edgeSplittingBehaviour.SplitHalfEdge(index, VectorMath.GetIntermediateVector(HalfEdges[index].OutgoingPoint.Point, HalfEdges[index].EndPoint.Point));
    }

    public void SplitHalfEdge(int index, Vector3 point)
    {
      _edgeSplittingBehaviour.SplitHalfEdge(index, point);
    }

    public void SplitHalfEdgeWithY(int index, float y)
    {
      var vector = VectorMath.GetIntermediateVector(HalfEdges[index].OutgoingPoint.Point, HalfEdges[index].EndPoint.Point);
      vector.y = y;
      _edgeSplittingBehaviour.SplitHalfEdge(index, vector);
    }

    #endregion fun with meshes
  }
}
