using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Assets.HalfEdgeMeshes;

[CustomEditor(typeof(SimpleMeshDebugger))]
public class SimpleMeshDebuggerEditor : Editor
{
  SimpleMeshDebugger debugger;
  MeshFilter meshFilter;
  MeshRenderer meshRenderer;
  Mesh mesh;

  #region InfoToggles
  public bool shouldShowVertexPositions;
  public bool shouldShowVertexIndices;
  public bool shouldShowUVs;
  public bool shouldShowVertexNormals;
  public bool shouldShowTriangelNumbers;
  public bool shouldShowTriangelVertexIndices;
  public bool shouldShowTriangelNormals;

  #endregion

  void Awake()
  {
    debugger = target as SimpleMeshDebugger;

    meshFilter = debugger.GetComponent<MeshFilter>();
    meshRenderer = debugger.GetComponent<MeshRenderer>();
    mesh = meshFilter.sharedMesh;
  }

  void OnSceneGUI()
  {
    Handles.matrix = debugger.transform.localToWorldMatrix;
    if (shouldShowVertexPositions)
      ShowVertexPositions();
    if (shouldShowVertexIndices)
      ShowVertexIndices();
    //if (shouldShowVertexNormals)
    //    ShowVertexNormals();
    if (shouldShowUVs)
      ShowUvs();
    if (shouldShowTriangelNumbers)
      ShowTriangleNumbers();
    if (shouldShowTriangelVertexIndices)
      ShowTriangleVertexIndices();
    //if (shouldShowTriangelNormals)
    //  ShowTriangleNormals();
  }

  private void ShowTriangleNormals()
  {
    for (int i = 0; i < mesh.triangles.Length; i += 3)
    {
      var p0Index = mesh.triangles[i + 0];
      var p1Index = mesh.triangles[i + 1];
      var p2Index = mesh.triangles[i + 2];
      var p0 = mesh.vertices[p0Index];
      var p1 = mesh.vertices[p1Index];
      var p2 = mesh.vertices[p2Index];

      var centroid = (p0 + p1 + p2) / 3f;
      var p0Normal = mesh.normals[p0Index];
      var p1Normal = mesh.normals[p1Index];
      var p2Normal = mesh.normals[p2Index];

      var triangelNormalAtCentroid = (p0Normal + p1Normal + p2Normal).normalized;

      Handles.color = new Color(Mathf.Abs(triangelNormalAtCentroid.x), Mathf.Abs(triangelNormalAtCentroid.y), Mathf.Abs(triangelNormalAtCentroid.z));
      Handles.DrawLine(centroid, centroid + triangelNormalAtCentroid);
    }
  }

  private void ShowTriangleVertexIndices()
  {
    for (int i = 0; i < mesh.triangles.Length; i += 3)
    {
      var p0Index = mesh.triangles[i + 0];
      var p1Index = mesh.triangles[i + 1];
      var p2Index = mesh.triangles[i + 2];
      var p0 = mesh.vertices[p0Index];
      var p1 = mesh.vertices[p1Index];
      var p2 = mesh.vertices[p2Index];

      var centroid = (p0 + p1 + p2) / 3f;

      var p0Lable = p0 + (centroid - p0) * 0.3f;
      var p1Lable = p1 + (centroid - p1) * 0.3f;
      var p2Lable = p2 + (centroid - p2) * 0.3f;

      Handles.Label(p0Lable, p0Index.ToString());
      Handles.Label(p1Lable, p1Index.ToString());
      Handles.Label(p2Lable, p2Index.ToString());
    }
  }

  private void ShowTriangleNumbers()
  {
    for (int i = 0; i < mesh.triangles.Length; i += 3)
    {
      var p0Index = mesh.triangles[i + 0];
      var p1Index = mesh.triangles[i + 1];
      var p2Index = mesh.triangles[i + 2];
      var p0 = mesh.vertices[p0Index];
      var p1 = mesh.vertices[p1Index];
      var p2 = mesh.vertices[p2Index];

      var centroid = (p0 + p1 + p2) / 3f;

      Handles.Label(centroid, (i / 3).ToString());
    }
  }

  private void ShowUvs()
  {
    for (int i = 0; i < mesh.vertexCount; i++)
    {
      Handles.Label(mesh.vertices[i], mesh.uv[i].ToString());
    }
  }

  private void ShowVertexNormals()
  {
    for (int i = 0; i < mesh.vertexCount; i++)
    {
      Handles.color = new Color(Mathf.Abs(mesh.normals[i].x), Mathf.Abs(mesh.normals[i].y), Mathf.Abs(mesh.normals[i].z));
      Handles.DrawLine(mesh.vertices[i], mesh.vertices[i] + mesh.normals[i]);
    }
  }

  private void ShowVertexIndices()
  {
    for (int i = 0; i < mesh.vertexCount; i++)
    {
      Handles.Label(mesh.vertices[i], i.ToString());
    }
  }

  private void ShowVertexPositions()
  {
    for (int i = 0; i < mesh.vertexCount; i++)
    {
      Handles.Label(mesh.vertices[i], mesh.vertices[i].ToString());
    }
  }

  public override void OnInspectorGUI()
  {
    shouldShowVertexPositions = EditorGUILayout.ToggleLeft("Show Vertex Position", shouldShowVertexPositions);
    shouldShowVertexIndices = EditorGUILayout.ToggleLeft("Show Vertex Indices", shouldShowVertexIndices);
    shouldShowVertexNormals = EditorGUILayout.ToggleLeft("Show Vertex Normals", shouldShowVertexNormals);
    shouldShowUVs = EditorGUILayout.ToggleLeft("Show UVs", shouldShowUVs);
    shouldShowTriangelNumbers = EditorGUILayout.ToggleLeft("Show Triangle Numbers", shouldShowTriangelNumbers);
    shouldShowTriangelVertexIndices = EditorGUILayout.ToggleLeft("Show Tirangle Vertex Indices", shouldShowTriangelVertexIndices);
    shouldShowTriangelNormals = EditorGUILayout.ToggleLeft("Show Triangle Normals", shouldShowTriangelNormals);
    EditorGUILayout.ToggleLeft("Zeit für Brüsli?", true);

    if (GUILayout.Button("Create HalfEdgeMesh"))
    {
      var go = new GameObject("HalfEdgeMesh");
      var he = go.AddComponent<HalfEdgeMesh>();
      he.BuildHalfEdgeMesh(mesh);

    }
  }
}
