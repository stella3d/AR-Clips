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

  }

}