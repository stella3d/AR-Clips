using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PointCloudVisuals : ARClipVisual
{
  [SerializeField]
  Mesh m_Mesh;

  const int k_MaxPoints = 1920 ;
  int[] m_Indices = new int[k_MaxVertices];

  new void Start()
  {
    base.Start();
    if (m_Mesh == null)
      m_Mesh = GetComponent<MeshFilter>().mesh;

    for (int i = 0; i < k_MaxVertices; i++)
        m_Indices[i] = i;
  }
	
  const int k_MaxVertices = 65534;

  Vector3[] previousCloud = new Vector3[500];
  int previousPointCount;
  Vector3[] concatCloud = new Vector3[65534];

  public int concatIndex;
  
  void Update()
  {
      var pointCloud = m_Reader.pointCloud;
      //m_Mesh = CreateMesh(pointCloud);
      if (m_Reader.pointCount != previousPointCount)
      {
        if (concatIndex + pointCloud.Length < k_MaxVertices)
        {
          pointCloud.CopyTo(concatCloud, concatIndex);
          concatIndex += m_Reader.pointCount;
        }
        else
        {
            concatIndex = 0;
            pointCloud.CopyTo(concatCloud, concatIndex);
            concatIndex += m_Reader.pointCount;
        }
      }

      // TODO - this allocates like hell.  find a better way to do this.
      m_Mesh.Clear();
      m_Mesh.vertices = concatCloud;
      m_Mesh.SetIndices(m_Indices, MeshTopology.Points, 0);
      previousCloud = pointCloud;
      previousPointCount = m_Reader.pointCount;
  }

  void CreateSelection(IEnumerable<Vector3> points)
     {
         var selection = new GameObject("Selection");
         MeshFilter meshFilter = (MeshFilter)selection.AddComponent(typeof(MeshFilter));
 
         meshFilter.mesh = CreateMesh(points);
 
         MeshRenderer renderer = selection.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
         renderer.material.shader = Shader.Find("Particles/Additive");
         Texture2D tex = new Texture2D(1, 1);
         tex.SetPixel(0, 0, Color.green);
         tex.Apply();
         renderer.material.mainTexture = tex;
         renderer.material.color = Color.green;
     }

     List<int> m_Triangles = new List<int>(1000);
 
     Mesh CreateMesh(IEnumerable<Vector3> stars)
     {
         Mesh m = new Mesh();
         m.name = "ScriptedMesh";
         m_Triangles.Clear();
 
         var vertices = stars.Select(x => new Vertex(x)).ToList();
 
         var result = MIConvexHull.ConvexHull.Create(vertices);
         m.vertices = result.Points.Select(x => x.ToVec()).ToArray();
         var xxx = result.Points.ToList();
 
         foreach(var face in result.Faces)
         {
             m_Triangles.Add(xxx.IndexOf(face.Vertices[0]));
             m_Triangles.Add(xxx.IndexOf(face.Vertices[1]));
             m_Triangles.Add(xxx.IndexOf(face.Vertices[2]));
         }
 
         m.triangles = m_Triangles.ToArray();
         m.RecalculateNormals();
         return m;
     }

}

