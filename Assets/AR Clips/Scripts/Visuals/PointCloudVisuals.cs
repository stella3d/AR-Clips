using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace ARClips
{
  public class PointCloudVisuals : ARClipVisual
  {
    [SerializeField]
    Mesh m_Mesh;

    Mesh[] m_MeshBuffer = new Mesh[200];

    const int k_MaxPoints = 1920 ;
    int[] m_Indices = new int[k_MaxVertices];

    const int k_MaxVertices = 8192;

    Vector3[] previousCloud = new Vector3[500];
    int previousPointCount;
    Vector3[] concatCloud;

    public int concatIndex;
    public int meshBufferIndex;

    new void Start()
    {
      base.Start();
      if (m_Mesh == null)
        m_Mesh = GetComponent<MeshFilter>().mesh;

      for (int i = 0; i < k_MaxVertices; i++)
          m_Indices[i] = i;

      for (int i = 0; i < m_MeshBuffer.Length; i++)
      {
        var obj = new GameObject("mesh points " + i);
        var meshFilter = obj.AddComponent<MeshFilter>();

        //m_MeshBuffer[i] = new Mesh();
        meshFilter.mesh = new Mesh();
        m_MeshBuffer[i] = (Mesh)Instantiate(meshFilter.mesh);
        m_MeshBuffer[i].vertices = new Vector3[k_MaxVertices];
        m_MeshBuffer[i].SetIndices(m_Indices, MeshTopology.Points, 0);
      }
      concatCloud = m_MeshBuffer[0].vertices;
    }

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
              meshBufferIndex++;
              concatCloud = m_MeshBuffer[meshBufferIndex].vertices;
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

  }
}

