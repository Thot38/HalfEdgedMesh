using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Assets.HalfEdgeMeshes
{
  public class VertexList : IEnumerable<Vertex>
  {
    //--- ToDo: Decide if a Dictionary<Vector3, Vertex> is better
    private readonly List<Vertex> _vertices;
    private readonly HalfEdgeMesh _mesh;

    public VertexList(HalfEdgeMesh mesh)
    {
      _vertices = new List<Vertex>();
      _mesh = mesh;
    }

    public VertexList(HalfEdgeMesh mesh, List<Vertex> vertices)
    {
      _vertices = vertices;
      _mesh = mesh;
    }

    /// <summary>
    /// The Number of Vertices in the VertexList
    /// </summary>
    public int Count => _vertices.Count;

    /// <summary>
    /// Creates a Vertex to the VertexList
    /// </summary>
    /// <param name="point">The Point of a Vertex to be added</param>
    /// <returns>The newly created vertex</returns>
    public Vertex CreateVertex(Vector3 point)
    {
      var x = _vertices.FirstOrDefault(p => p.Point.Equals(point));
      if (x != null)
        return x;
      var vertex = _vertices.FirstOrDefault(p => p.Point.Equals(point)) ?? new Vertex(point, _vertices.Count);
      vertex.PositionChangedEvent += Vertex_PositionChanged;
      _vertices.Add(vertex);
      return vertex;
    }

    public void RemoveVertex(Vertex v)
    {
      _vertices.Remove(v);
      // --- Plainly removing a vertex results in a gap, causing trouble when the mesh is generated
      for (int i = 0; i < Count; i++)
      {
        _vertices[i].Index = i;
      }
      //_mesh.RemoveMeshVertex(v.Point);
    }

    public Vertex this[int index]
    {
      get => _vertices[index];
      private set => _vertices[index] = value;
    }

    public List<HalfEdge> GetVertexCirculator(Vertex v)
    {
      // --- Sorry :(
      return _mesh.HalfEdges.Where(p => p.OutgoingPoint.Index == v.Index).ToList();
      //var result = new List<HalfEdge>();
      //var halfEdge = v.HalfEdge;
      //var next = halfEdge;
      //do
      //{
      //  result.Add(next);
      //  if (next.Opposing == null)
      //    break;
      //  next = next.Opposing.Next;
      //} while (halfEdge.Index != next.Index);

      //var prev = halfEdge.Previous.Opposing;
      //while (prev != null && halfEdge.Index != prev.Index)
      //{
      //  if (result.Contains(prev))
      //    break;

      //  result.Add(prev);
      //  prev = prev.Previous.Opposing;
      //}
      //return result;
    }

    #region Vertex position changed

    private void Vertex_PositionChanged(object sender, EventArgs e)
    {
      if (sender is Vertex v)
        _mesh.SetMeshVertex(v.Index, v.Point);
    }

    #endregion

    #region Enumerator Stuff

    public IEnumerator<Vertex> GetEnumerator()
    {
      return _vertices.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _vertices.GetEnumerator();
    }

    #endregion
  }
}
