using Assets.HalfEdgeMeshes;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HalfEdgeMeshes.BreakingBehaviourDeterminer
{
  public class BreakingBehaviourDeterminer
  {
    public BreakingBehaviourDeterminer()
    {

    }

    /// <summary>
    /// This Method returns all points that should be considered to be added onto a surface in order to simulate breaking
    /// </summary>
    /// <param name="face"> The Face that was impacted</param>
    /// <param name="impactPoint"> The Position of the impact</param>
    /// <returns>List of Points on the face's plain to crack it</returns>
    public List<Vector3> GetBreakingPoints(Face face, Vector3 impactPoint)
    {
      var result = new List<Vector3>();

      var v1 = face.HalfEdge.OutgoingPoint.Point;
      var v2 = face.HalfEdge.Next.OutgoingPoint.Point;
      var v3 = face.HalfEdge.Next.Next.OutgoingPoint.Point;

      var materialBehaviour = GetMaterialBreakingBehaviour(face.materialType);

      for (int i = 0; i < materialBehaviour.Count; i++)
      {
        for (int j = 0; j < materialBehaviour[i]; j++)
        {
          var newPoint = UnityEngine.Random.insideUnitCircle * (i + .5f) + new Vector2(impactPoint.x, impactPoint.z);
          var addPoint = new Vector3(newPoint.x, impactPoint.y, newPoint.y);
          result.Add(addPoint);
        }
      }
      return result;
    }

    private List<int> GetMaterialBreakingBehaviour(MaterialType materialType)
    {
      switch (materialType)
      {
        case MaterialType.Glas:
          return new List<int> { 18, 8, 0, 0, 20 };
        case MaterialType.Rock:
          return new List<int> { 10, 30, 40, 100 };
        case MaterialType.Wood:
          return new List<int> { 5, 5, 5, 5, 5 };
        default:
          return new List<int> { 5, 5, 5, 5, 5 };
      }
    }
  }
}