using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class ARClipReader : MonoBehaviour, IARClipReader
{
  // increase this number to slow down playback
  public int updatesPerDeviceUpdate;
  public string fileSource;

  public Vector3 position;
  public Quaternion rotation;
  public double timestamp;
  public float lightEstimate;

  // these are just informational / to look at in the inspector
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

  // to support scrubbing 
  public Dictionary<double, long> timePositions { get; private set; }

  const int k_MaxPlanes = 512;        
  const int k_MaxAnchors = 512;        
  const int k_MaxPoints = 15360;

  FileStream m_File;
  BinaryReader m_Stream;

  int m_FrameSkipIndex;
  double m_LastPointCloudTimestamp;

  void Start () 
  {
    m_File = new FileStream(fileSource, FileMode.Open);
    m_Stream = new BinaryReader(m_File);

    timePositions = new Dictionary<double, long>();
  }

  void Update () 
  {
    m_FrameSkipIndex++;

    if (m_FrameSkipIndex % updatesPerDeviceUpdate == 0)
    {
      ReadFrame();
    }
  }

  public void ReadFrame()
  {
    if (m_File.Position >= m_File.Length)
      return;

    var frameBeginPosition = m_File.Position;

    timestamp = m_Stream.ReadDouble();
    lightEstimate = m_Stream.ReadSingle();

    ReadPose();
    ReadPointCloud();
    ReadTrackedPlanes();
    ReadAnchors();

    // frame delimiter
    m_Stream.ReadChar();

    // link this timestamp to the stream position before the frame
    timePositions.Add(timestamp, frameBeginPosition);
  }

  void ReadPose()
  {
    ReadVector3(out position);
    ReadQuaternion(out rotation);
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

  void ReadTrackedPlanes()
  {
    planeCount = m_Stream.ReadInt32();

    for (int i = 0; i < planeCount; i++)
    {
      var vertexCount = m_Stream.ReadInt32();

      // TODO - don't allocate a new array here,  find a way to set length 
      trackedPlanePolygons[i] = new Vector3[vertexCount];

      for (int n = 0; n < vertexCount; n++)
      {
        ReadVector3(out trackedPlanePolygons[i][n]);
      }
    }
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

  // right now this only supports seeking backwards, since it relies
  // on already having the stream position for a timestamp.  
  // to seek forward, implement reading forward until we find the timestamp
  public void SeekToTime(double timeStamp)
  {
    long targetPosition;
    timePositions.TryGetValue(timeStamp, out targetPosition);

    m_File.Seek(targetPosition, SeekOrigin.Begin);
    ReadFrame();
  }

}
