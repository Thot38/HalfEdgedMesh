using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.HalfEdgeMeshes
{
  public class Vertex
  {
    #region Events

    public event EventHandler PositionChangedEvent;
    
    #endregion

    public Vertex(Vector3 point)
    {
      Point = point;
      Index = -1;
    }

    public Vertex(Vector3 point, int index)
    {
      Point = point;
      Index = index;
    }

    public Vertex(float x, float y, float z) : this(new Vector3(x, y, z))
    { }

    public Vertex(float x, float y, float z, int index) : this(new Vector3(x, y, z), index)
    { }

    /// <summary>
    /// The Index of the Vertex
    /// </summary>
    public int Index { get; set; }


    private Vector3 _point;
    /// <summary>
    /// The Point
    /// </summary>
    public Vector3 Point
    {
      get => _point;
      set
      {
        _point = value;
        PositionChangedEvent?.Invoke(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// The Outgoing HalfEdge
    /// </summary>
    public HalfEdge HalfEdge { get; set; }
  }
}