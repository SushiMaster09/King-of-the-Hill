using System;
using System.Collections.Generic;
using UnityEngine;

public struct Face
{
    public List<Vector3> vertices { get; private set; }
    public List<int> triangles { get; private set; }
    public List<Vector2> uvs { get; private set; }

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class HexRenderer : MonoBehaviour
{
    private Mesh mMesh;
    private MeshFilter mMeshFilter;

    [HideInInspector] public MeshRenderer mMeshRenderer;

    private List<Face> mFaces;

    [Header("Hexagon Mesh Settings")]
    public float innerSize;
    public float outerSize;
    public float height;
    public bool isFlatTopped;
    public Material material;

    private void Awake()
    {
        mMeshFilter = GetComponent<MeshFilter>();
        mMeshRenderer = GetComponent<MeshRenderer>();

        mMesh = new Mesh();
        mMesh.name = "Hex";

        mMeshFilter.mesh = mMesh;
        mMeshRenderer.material = material;
    }

    private void OnEnable()
    {
        DrawMesh();
    }

    public void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
    }

    private void DrawFaces()
    {
        mFaces = new List<Face>();

        //Top faces
        for (int point = 0; point < 6; point++)
        {
            mFaces.Add(CreateFace(innerSize, outerSize, height / 2f, height / 2f, point));
        }

        //Outer faces
        for (int point = 0; point < 6; point++)
        {
            mFaces.Add(CreateFace(outerSize, outerSize, height / 2f, -height / 2f, point, true));
        }

        //Inner faces
        for (int point = 0; point < 6; point++)
        {
            mFaces.Add(CreateFace(innerSize, innerSize, height / 2f, -height / 2f, point, false));
        }
    }

    private void CombineFaces()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < mFaces.Count; i++)
        {
            vertices.AddRange(mFaces[i].vertices);
            uvs.AddRange(mFaces[i].uvs);

            int offset = (4 * i);
            foreach (int triangle in mFaces[i].triangles)
            {
                tris.Add(triangle + offset);
            }
        }

        mMesh.vertices = vertices.ToArray();
        mMesh.triangles = tris.ToArray();
        mMesh.uv = uvs.ToArray();
        mMesh.RecalculateNormals();
    }

    private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false)
    {
        Vector3 pA = GetPoint(innerRad, heightB, point);
        Vector3 pB = GetPoint(innerRad, heightB, (point < 5) ? point + 1 : 0);
        Vector3 pC = GetPoint(outerRad, heightA, (point < 5) ? point + 1 : 0);
        Vector3 pD = GetPoint(outerRad, heightA, point);

        List<Vector3> vertices = new List<Vector3>() { pA, pB, pC, pD };
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        if (reverse)
        {
            vertices.Reverse();
        }

        return new Face(vertices, triangles, uvs);
    }

    protected Vector3 GetPoint(float size, float height, int index)
    {
        float angleDeg = isFlatTopped ? 60 * index: 60 * index - 30;
        float angleRad = Mathf.PI / 180f * angleDeg;
        return new Vector3(size * Mathf.Cos(angleRad), height, size * Mathf.Sin(angleRad));
    }
}
