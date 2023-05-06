using Assets.HalfEdgeMeshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.Scripts.Utilities
{
  public class VectorMath
  {
    public static Vector3 GetIntermediateVector(Vector3 v1, Vector3 v2)
    {
      var direction = new Vector3(v2.x - v1.x, v2.y - v1.y, v2.z - v1.z);
      return v1 + 0.5f * direction;
    }

    public static Vector3 GetFaceCentoid(Face face)
    {
      return (face.HalfEdge.OutgoingPoint.Point + face.HalfEdge.Next.OutgoingPoint.Point + face.HalfEdge.Next.Next.OutgoingPoint.Point) / 3;    
    }

    public static bool IsPointOnFace(Face face, Vector3 point)
    {
      var p0 = face.HalfEdge.OutgoingPoint.Point;
      var p1 = face.HalfEdge.Next.OutgoingPoint.Point;
      var p2 = face.HalfEdge.Next.Next.OutgoingPoint.Point;

      if (SameSide(point, p0, p1, p2) && SameSide(point, p1, p0, p2) && SameSide(point, p2, p0, p1))
      {
        Vector3 vc1 = Vector3.Cross(p0 - p1, p0 - p2);
        if (Math.Abs(Vector3.Dot(p0 - point, vc1)) <= .01f)
          return true;
      }

      return false;

    }

    private static bool SameSide(Vector3 p1, Vector3 p2, Vector3 A, Vector3 B)
    {
      Vector3 cp1 = Vector3.Cross(B - A, p1 - A);
      Vector3 cp2 = Vector3.Cross(B - A, p2 - A);
      if (Vector3.Dot(cp1, cp2) >= 0) return true;
      return false;

    }

    public static Vector3 GetFaceNormal(Face face)
    {
      var a = face.HalfEdge.OutgoingPoint.Point;
      var b = face.HalfEdge.Next.Next.OutgoingPoint.Point;
      var c = face.HalfEdge.Next.OutgoingPoint.Point;

      var dir1 = b - a;
      var dir2 = c - a;

      return Vector3.Cross(dir1, dir2).normalized;
    }

    public static float GetRadiusOfInsphere(Vector3 v1, Vector3 v2)
    {
      return (Mathf.Sqrt(6) / 4) * Vector3.Distance(v1, v2);
    }

    public static Vector3 GetTetrahedronCentoid(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
      return (a + b + c + d) / 4;
    }
  }
}
