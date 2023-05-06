using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;
using System.Linq;
using System;

namespace Assets.HalfEdgeMeshes
{
  public class HalfEdgeList : IEnumerable<HalfEdge>
  {
    private readonly List<HalfEdge> _halfEdges;
    private readonly HalfEdgeMesh _mesh;


    public HalfEdgeList(HalfEdgeMesh mesh)
    {
      _halfEdges = new List<HalfEdge>();

      _mesh = mesh;
    }

    public HalfEdgeList(HalfEdgeMesh mesh, List<HalfEdge> halfEdges)
    {
      _halfEdges = halfEdges;

      _mesh = mesh;
    }


    private List<int> x => _halfEdges.Select(p => p.Index).ToList();

    public int Count => _halfEdges.Count;

    #region HalfEdge Maintainance

    public HalfEdge CreateHalfEdge(Vertex vertex, Face face, HalfEdge next)
    {
      var halfEdge = new HalfEdge(vertex, face, next, Count);
      vertex.HalfEdge = halfEdge;
      _halfEdges.Add(halfEdge);
      return halfEdge;
    }
    public HalfEdge this[int index]
    {
      get => _halfEdges[index];
      private set => _halfEdges[index] = value;
    }

    public void CreatePair(HalfEdge halfEdge1, HalfEdge halfEdge2)
    {
      halfEdge1.Opposing = halfEdge2;
      halfEdge2.Opposing = halfEdge1;
    }

    public void RemoveAll(Predicate<HalfEdge> match)
    {
      _halfEdges.RemoveAll(match);
    }

    #endregion

    #region Enumerator Stuff

    public IEnumerator<HalfEdge> GetEnumerator()
    {
      return _halfEdges.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _halfEdges.GetEnumerator();
    }
    #endregion
  }
}