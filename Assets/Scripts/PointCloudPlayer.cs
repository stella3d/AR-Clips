using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class PointCloudPlayer : MonoBehaviour 
{
  public int updatesPerDeviceFrame;
  public string fileSource;
  public Mesh mesh;

  public Vector3 position;
  public Quaternion rotation;
  public int deviceFrameIndex;
  public double deviceTimestamp;
  public float lightEstimate;
  public int planeCount;
  public int pointCount;

  [HideInInspector]
  public Vector3[] pointCloud = new Vector3[k_MaxPoints];
  [HideInInspector]
  public Vector3[][] trackedPlanePolygons = new Vector3[k_MaxPlanes][];
  [HideInInspector]
  public Vector3[] anchorPositions = new Vector3[k_MaxAnchors];
  [HideInInspector]
  public Quaternion[] anchorRotations = new Quaternion[k_MaxAnchors];

  const int k_MaxPlanes = 512;        
  const int k_MaxAnchors = 512;        
  const int k_MaxPoints = 15360;

  FileStream m_File;
  BinaryReader m_Stream;

  int m_FrameSkipIndex;
  double m_LastPointCloudTimestamp;
  int[] m_Indices = new int[k_MaxPoints];

  void Start () 
  {
    m_File = new FileStream(fileSource, FileMode.Open);
    m_Stream = new BinaryReader(m_File);

    if (mesh == null)
      mesh = GetComponent<MeshFilter>().mesh;
    
    mesh.Clear();
  }

  void Update () 
  {
    m_FrameSkipIndex++;

    if (m_FrameSkipIndex % updatesPerDeviceFrame == 0)
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

      mesh.Clear();
      mesh.vertices = pointCloud;
      mesh.SetIndices(m_Indices, MeshTopology.Points, 0);
    }
  }

  void ReadPose()
  {
    ReadVector3(out position);
    ReadQuaternion(out rotation);
  }

  void ReadTrackedPlanes()
  {
    planeCount = m_Stream.ReadInt32();

    for (int i = 0; i < planeCount; i++)
    {
      var thisPlanepointCount = m_Stream.ReadInt32();
      trackedPlanePolygons[i] = new Vector3[thisPlanepointCount];

      for (int n = 0; n < thisPlanepointCount; n++)
      {
        ReadVector3(out trackedPlanePolygons[i][n]);
      }
    }
  }

  void ReadPointCloud()
  {
    pointCount = m_Stream.ReadInt32();
    for (int i = 0; i < pointCount; i++)
    {
      ReadVector3(out pointCloud[i]);
    }

    if (pointCount < pointCloud.Length)
    {
      Array.Clear(pointCloud, pointCount, pointCloud.Length - pointCount);
    }
  }


  void ReadFrame()
  {
    deviceFrameIndex = m_Stream.ReadInt32();
    deviceTimestamp = m_Stream.ReadDouble();
    lightEstimate = m_Stream.ReadSingle();

    ReadPose();
    ReadPointCloud();
    ReadTrackedPlanes();
    ReadAnchors();

    // frame delimiter
    m_Stream.ReadChar();
  }

  void ReadVector3(out Vector3 vec)
  {
    vec.x = m_Stream.ReadSingle();
    vec.y = m_Stream.ReadSingle();
    vec.z = m_Stream.ReadSingle();
  }

  void ReadQuaternion(out Quaternion quat)
  {
    quat.w = m_Stream.ReadSingle();
    quat.x = m_Stream.ReadSingle();
    quat.y = m_Stream.ReadSingle();
    quat.z = m_Stream.ReadSingle();
  }

  void ReadAnchors()
  {
    var anchorCount = m_Stream.ReadInt32();

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
