using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.HalfEdgeMeshes
{
  public class HalfEdge
  {
    public HalfEdge(Vertex outgoing, Face face, HalfEdge next)
    {
      OutgoingPoint = outgoing;
      Face = face;
      Next = next;
      Index = -1;
    }

    public HalfEdge(Vertex outgoing, Face face, HalfEdge next, int index)
    {
      OutgoingPoint = outgoing;
      Face = face;
      Next = next;
      Index = index;
    }

    public int Index { get; private set; }

    /// <summary>
    /// The Vertex the HalfEdge comes from
    /// </summary>
    public Vertex OutgoingPoint { get; set; }

    /// <summary>
    /// The next HalfEdge, Counter Clockwise
    /// </summary>
    public HalfEdge Next { get; set; }

    /// <summary>
    /// The opposing HalfEdge
    /// </summary>
    public HalfEdge Opposing { get; set; }

    /// <summary>
    /// The face of the HalfEdge
    /// </summary>
    public Face Face { get; set; }

    /// <summary>
    /// Getter for the previous HalfEdge, for easier access (this.Next.Next)
    /// </summary>
    public HalfEdge Previous => Next.Next;

    /// <summary>
    /// Getter for the EndPoint of this HalfEdge, for easier access (this.Next.OutgoingPoint)
    /// </summary>
    public Vertex EndPoint => Next.OutgoingPoint;
  }
}