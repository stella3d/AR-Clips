using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class PointCloudPlayer : MonoBehaviour 
{
  const int MAX_POINT_COUNT = 15360;

  public string fileSource;

  public Mesh m_Mesh;
  public Mesh m_PlaneMesh;

  public Vector3 position;
  public Quaternion rotation;

  /// don't click an array this big, it crashes Unity
  [HideInInspector] 
  public Vector3[] pointCloud = new Vector3[MAX_POINT_COUNT];

  public Vector3[][] trackedPlanePolygons = new Vector3[64][];

  public Vector3[] anchorPositions = new Vector3[64];
  public Quaternion[] anchorRotations = new Quaternion[64];

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
    m_File = new FileStream(fileSource, FileMode.Open);
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

      // update pose
      transform.position = position;
      transform.rotation = rotation;
 
      // update point cloud mesh visuals
      m_Indices = new int[pointCloud.Length];
      for (int i = 0; i < pointCloud.Length; i++)
      {
        m_Indices[i] = i;
      }

      m_Mesh.Clear();
      m_Mesh.vertices = pointCloud;
      m_Mesh.SetIndices(m_Indices, MeshTopology.Points, 0);


      for (int i = 0; i < anchorPositions.Length; i++)
      {
        
      }

    }
  }

  void ReadFrame()
  {
    var frameIndex = m_BinaryReader.ReadInt32();

    ReadVector3(out position);

    ReadQuaternion(out rotation);

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

    var cloudLength = pointCloud.Length;
    if (pointCount < pointCloud.Length)
    {
      Array.Clear(pointCloud, pointCount, cloudLength - pointCount);
    }

    // Read Frame anchor data
    ReadAnchors();

    //ending char, probably can remove
    m_BinaryReader.ReadChar();
  }

  void ReadVector3(out Vector3 vec)
  {
    vec.x = m_BinaryReader.ReadSingle();
    vec.y = m_BinaryReader.ReadSingle();
    vec.z = m_BinaryReader.ReadSingle();
  }

  void ReadQuaternion(out Quaternion quat)
  {
    quat.w = m_BinaryReader.ReadSingle();
    quat.x = m_BinaryReader.ReadSingle();
    quat.y = m_BinaryReader.ReadSingle();
    quat.z = m_BinaryReader.ReadSingle();
  }

  void ReadAnchors()
  {
    var anchorCount = m_BinaryReader.ReadInt32();

    if (anchorCount > 0)
    {
      for (int i = 0; i < anchorCount; i++)
      {
        ReadVector3(out anchorPositions[i]);
        ReadQuaternion(out anchorRotations[i]);
      }
    }
  }

}
