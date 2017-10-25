using UnityEngine;
using System.Collections;
using System.IO;

public class PointCloudPlayer : MonoBehaviour 
{
  const int MAX_POINT_COUNT = 15360;

  public Mesh m_Mesh;

  public Vector3 m_Position;
  Vector3[] m_Points = new Vector3[MAX_POINT_COUNT];
  Vector3[] m_PreviousPoints = new Vector3[MAX_POINT_COUNT];

  double m_LastPointCloudTimestamp;

  public int m_FrameIndex;
  public int frameSkip = 1;
  public int pointCount;

  FileStream m_File;
  BinaryReader m_BinaryReader;

  int[] m_Indices = new int[MAX_POINT_COUNT];

  void Start () 
  {
    m_File = new FileStream("Assets/pointcloud.json", FileMode.Open);
    m_BinaryReader = new BinaryReader(m_File);

    //m_Mesh = GetComponent<MeshFilter>().mesh;
    m_Mesh.Clear();
  }

  void Update () 
  {
    //m_PreviousPoints = m_Points;
    m_FrameIndex++;

    if (m_FrameIndex % frameSkip == 0)
    {
      ReadFrame();
      transform.position = m_Position;

      m_Indices = new int[m_Points.Length];
      for (int i = 0; i < m_Points.Length; i++)
      {
        m_Indices[i] = i;
      }

      m_Mesh.Clear();
      m_Mesh.vertices = m_Points;
      m_Mesh.SetIndices(m_Indices, MeshTopology.Points, 0);

      pointCount = m_Mesh.vertices.Length;
    }
   
  }

  void ReadFrame()
  {
    var frameIndex = m_BinaryReader.ReadInt32();

    ReadVector3Binary(out m_Position);

    for (int i = 0; i < m_Points.Length; i++)
    {
      ReadVector3Binary(out m_Points[i]);
    }

    m_BinaryReader.ReadChar();
  }

  void ReadVector3Binary(out Vector3 vec)
  {
    vec.x = m_BinaryReader.ReadSingle();
    vec.y = m_BinaryReader.ReadSingle();
    vec.z = m_BinaryReader.ReadSingle();
  }
}
