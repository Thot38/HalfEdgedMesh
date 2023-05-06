using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.HalfEdgeMeshes;
using System.Linq;
using Assets.Scripts.Utilities;
using Assets.Scripts.Benchmark;

[CustomEditor(typeof(HalfEdgeMesh))]
public class HalfEdgeMeshEditor : Editor
{
  HalfEdgeMesh halfEdgeMesh;

  public string x, y, z;
  public bool showHundrets = false;

  void Awake()
  {
    halfEdgeMesh = target as HalfEdgeMesh;
  }

  public override void OnInspectorGUI()
  {
    if (GUILayout.Button("Split All Faces!"))
    {
      halfEdgeMesh.SplitAllFaces();
    }
    if (GUILayout.Button("Test, I'm too lazy to rename all the time"))
    {
      //halfEdgeMesh.SpitAllFaces();
      halfEdgeMesh.SimulateBreaking(halfEdgeMesh.NextV);

      //halfEdgeMesh.SplitOffFace(halfEdgeMesh.Faces[0], Vector3.down * .25f);
      //halfEdgeMesh.AddFace(halfEdgeMesh.HalfEdges[halfEdgeMesh.Next], halfEdgeMesh.NextV);
    }

    if (GUILayout.Button("Test2"))
    {
      halfEdgeMesh.SplitHalfEdge(2);
      //halfEdgeMesh.SplitOffFace(halfEdgeMesh.Faces[0], Vector3.down * .25f);
    }

    if (GUILayout.Button("Move Vertex"))
    {
      halfEdgeMesh.Vertices[0].Point += (Vector3.down * 4);
    }
    if (GUILayout.Button("Smooth"))
    {
      halfEdgeMesh.SmoothObject();
    }

    if (GUILayout.Button("Regenerate Mesh"))
    {
      halfEdgeMesh.GenerateUnityMesh();
    }

    if (GUILayout.Button("Edge Collapse"))
    {
      halfEdgeMesh.EdgeCollapse(halfEdgeMesh.Next);
    }

    if (GUILayout.Button("Split Edge"))
    {
      halfEdgeMesh.SplitHalfEdge(1, new Vector3(-1, 1, 2.5f));
    }
    if (GUILayout.Button("Create Edge Collapse Example"))
    {
      halfEdgeMesh.SplitHalfEdge(2);
      halfEdgeMesh.SplitHalfEdge(5);
    }

    showHundrets = EditorGUILayout.ToggleLeft("Show hundrets", showHundrets);

    DrawDefaultInspector();



  }

  public void OnSceneGUI()
  {
    if (showHundrets)
      ShowHundrets();
  }

  private void ShowHundrets()
  {
    for (int i = 0; i < halfEdgeMesh.HalfEdges.Count; i += 100)
    {
      var start = halfEdgeMesh.HalfEdges[i].OutgoingPoint;
      var end = halfEdgeMesh.HalfEdges[i].EndPoint;
      var center = VectorMath.GetIntermediateVector(start.Point, end.Point);

      Handles.Label(center, i.ToString());
    }
  }


  [MenuItem("GameObject/3D Object/HalfEdgeMesh %F1")]
  public static HalfEdgeMesh CreateHalfEdgeMesh()
  {
    var go = new GameObject("HalfEdgeMesh");
    var halfEdgeMesh = go.AddComponent<HalfEdgeMesh>();
    halfEdgeMesh.CreateMesh(new Vector3(0, 0, 1), new Vector3(0, 2 * Mathf.Sqrt(2), -1) / 3, new Vector3(-Mathf.Sqrt(6), -Mathf.Sqrt(2), -1) / 3);
    halfEdgeMesh.AddFace(halfEdgeMesh.HalfEdges[0], new Vector3(Mathf.Sqrt(6), -Mathf.Sqrt(2), -1) / 3);
    halfEdgeMesh.AddFace(halfEdgeMesh.HalfEdges[1], new Vector3(Mathf.Sqrt(6), -Mathf.Sqrt(2), -1) / 3);
    halfEdgeMesh.AddFace(halfEdgeMesh.HalfEdges[2], new Vector3(Mathf.Sqrt(6), -Mathf.Sqrt(2), -1) / 3);

    go.AddComponent<SimpleMeshDebugger>();

    return halfEdgeMesh;
  }

  [MenuItem("GameObject/3D Object/BreakingSample %F2")]
  public static HalfEdgeMesh CreateBreakingSample()
  {
    var go = new GameObject("HalfEdgeMesh");
    var halfEdgeMesh = go.AddComponent<HalfEdgeMesh>();
    halfEdgeMesh.CreateMesh(new Vector3(0, 1, 0), new Vector3(0, 1, 10), new Vector3(10, 1, 0));
    halfEdgeMesh.AddFace(halfEdgeMesh.HalfEdges[2], new Vector3(10, 1, 10));

    go.AddComponent<SimpleMeshDebugger>();

    return halfEdgeMesh;
  }


  [MenuItem("GameObject/3D Object/Triangle %F3")]
  public static HalfEdgeMesh CreateTri()
  {
    var go = new GameObject("Tri");
    var halfEdgeMesh = go.AddComponent<HalfEdgeMesh>();
    halfEdgeMesh.CreateMesh(new Vector3(0, 1, 0), new Vector3(0, 1, 10), new Vector3(10, 1, 0));

    go.AddComponent<SimpleMeshDebugger>();

    return halfEdgeMesh;
  }

  [MenuItem("GameObject/3D Object/Smoothable")]
  public static HalfEdgeMesh CreateSmoothingTest()
  {
    var go = new GameObject("SmoothingTest");
    var halfEdgeMesh = go.AddComponent<HalfEdgeMesh>();
    halfEdgeMesh.CreateMesh(new Vector3(0, 1, 1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1));

    halfEdgeMesh.AddFace(halfEdgeMesh.HalfEdges[0], new Vector3(-1f, 4, 1));
    halfEdgeMesh.AddFace(halfEdgeMesh.HalfEdges[2], new Vector3(-1f, 5, 0));


    go.AddComponent<SimpleMeshDebugger>();
    return halfEdgeMesh;
  }
  [MenuItem("GameObject/3D Object/Smoothable2 %F4")]
  public static HalfEdgeMesh CreateSmoothingTest2()
  {
    var go = new GameObject("SmoothingTest");
    var halfEdgeMesh = go.AddComponent<HalfEdgeMesh>();
    halfEdgeMesh.CreateMesh(new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, -1));

    halfEdgeMesh.AddFace(halfEdgeMesh.HalfEdges[2], new Vector3(1, 1, 1));

    halfEdgeMesh.SplitHalfEdgeWithY(2, 0);

    halfEdgeMesh.AddFace(halfEdgeMesh.HalfEdges[0], new Vector3(-1.25f, 3, -1.25f));
    halfEdgeMesh.AddFace(halfEdgeMesh.HalfEdges[halfEdgeMesh.HalfEdges.Count - 2], new Vector3(1.25f, 3, -1.25f));

    //halfEdgeMesh.CreateLandscape();

    go.AddComponent<SimpleMeshDebugger>();
    return halfEdgeMesh;
  }

  [MenuItem("Edit/Start Benchmark %F6")]
  public static void Benchmark()
  {
    new Benchmark().StartBenchmark();
  }
}
