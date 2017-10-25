using UnityEngine;
using System.Collections;
using System.IO;

public class PointCloudPlayer : MonoBehaviour 
{
  const int MAX_POINT_COUNT = 15360;

  public Mesh m_Mesh;
  public Mesh m_PlaneMesh;

  public Vector3 position;
  public Vector3[] pointCloud = new Vector3[MAX_POINT_COUNT];
  public Vector3[][] trackedPlanePolygons = new Vector3[64][];

  double m_LastPointCloudTimestamp;

  public int m_FrameIndex;
  public int frameSkip = 1;
  public int pointCount;
  public int planeCount;

  FileStream m_File;
  BinaryReader m_BinaryReader;

  int[] m_Indices = new int[MAX_POINT_COUNT];

  void Start () 
  {
    m_File = new FileStream("Assets/pointcloud.json", FileMode.Open);
    m_BinaryReader = new BinaryReader(m_File);

    if (m_Mesh == null)
      m_Mesh = GetComponent<MeshFilter>().mesh;
    
    m_Mesh.Clear();
  }

  void Update () 
  {
    m_FrameIndex++;

    if (m_FrameIndex % frameSkip == 0)
    {
      ReadFrame();

      transform.position = position;

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

  void ReadFrame()
  {
    var frameIndex = m_BinaryReader.ReadInt32();

    ReadVector3(out position);

    // read tracked plane data
    planeCount = m_BinaryReader.ReadInt32();

    for (int i = 0; i < planeCount; i++)
    {
      var thisPlanepointCount = m_BinaryReader.ReadInt32();
      trackedPlanePolygons[i] = new Vector3[thisPlanepointCount];

      for (int n = 0; n < thisPlanepointCount; n++)
      {
        ReadVector3(out trackedPlanePolygons[i][n]);
      }
    }

    // read point cloud data
    pointCount = m_BinaryReader.ReadInt32();
    for (int i = 0; i < pointCount; i++)
    {
      ReadVector3(out pointCloud[i]);
    }

    //ending char, probably can remove
    m_BinaryReader.ReadChar();
  }

  void ReadVector3(out Vector3 vec)
  {
    vec.x = m_BinaryReader.ReadSingle();
    vec.y = m_BinaryReader.ReadSingle();
    vec.z = m_BinaryReader.ReadSingle();
  }


}
