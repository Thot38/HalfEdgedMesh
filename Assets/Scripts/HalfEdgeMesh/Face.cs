using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.HalfEdgeMeshes
{
  public class Face
  {
    public Face(HalfEdge halfEdge, int index)
    {
      HalfEdge = halfEdge;
      Index = index;
    }

    /// <summary>
    /// A bonding HalfEdge
    /// </summary>
    public HalfEdge HalfEdge { get; set; }

    public int Index { get; private set; }

    public MaterialType materialType { get; set; }
  }

  public enum MaterialType
  {
    Glas,
    Rock,
    Wood
  }
}
