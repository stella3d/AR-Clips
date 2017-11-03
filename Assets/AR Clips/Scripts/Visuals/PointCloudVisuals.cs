using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointCloudVisuals : ARClipVisual
{
  [SerializeField]
  Mesh m_Mesh;

  const int k_MaxPoints = 1920 ;
  int[] m_Indices = new int[k_MaxPoints];

  new void Start()
  {
    base.Start();
    if (m_Mesh == null)
      m_Mesh = GetComponent<MeshFilter>().mesh;
  }
	
  void Update()
  {
    var pointCloud = m_Reader.pointCloud;
    // TODO - this allocates like hell.  find a better way to do this.
    m_Indices = new int[pointCloud.Length];
    for (int i = 0; i < pointCloud.Length; i++)
    {
      m_Indices[i] = i;
    }

    m_Mesh.Clear();
    m_Mesh.vertices = pointCloud;
    m_Mesh.SetIndices(m_Indices, MeshTopology.Points, 0);
  }
}

