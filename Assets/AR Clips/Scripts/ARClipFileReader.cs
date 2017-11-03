using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ARClipFileReader : IARClipReader
{
  public ARClip clip;

  public double elapsed;
  public int totalFrameCount;
  public int pointCount;
  public int planeCount;
  public int anchorCount;

  // to support scrubbing 
  public Dictionary<double, long> timePositions { get; private set; }

  // public members below all have data from platform's AR service
  public Vector3 position;
  public Quaternion rotation;
  public double timestamp;
  public float lightEstimate;

  [HideInInspector]
  public Vector3[] pointCloud = new Vector3[k_MaxPoints];
  [HideInInspector]
  public Vector3[][] planePolygons = new Vector3[k_MaxPlanes][];
  [HideInInspector]
  public Vector3[] anchorPositions = new Vector3[k_MaxAnchors];
  [HideInInspector]
  public Quaternion[] anchorRotations = new Quaternion[k_MaxAnchors];

  const int k_MaxPlanes = 512;        
  const int k_MaxAnchors = 512;        
  const int k_MaxPoints = 2048;

  long m_FrameBeginStreamPosition;
  double m_LastPointCloudTimestamp;
  double m_FirstPointCloudTimeStamp;

  FileStream m_File;
  MemoryStream m_Buffer;
  public Stream m_BaseStream;
  BinaryReader m_Stream;

  public ARClipFileReader(string fileSource) 
  {
    m_File = new FileStream(fileSource, FileMode.Open);
    m_BaseStream = m_File;
    m_Stream = new BinaryReader(m_File);
    timePositions = new Dictionary<double, long>();
  }

  public ARClipFileReader(ARClip clip) 
  {
    this.clip = clip;
    m_Buffer = new MemoryStream(clip.data);
    m_BaseStream = m_Buffer;
    m_Stream = new BinaryReader(m_Buffer);
    timePositions = new Dictionary<double, long>();
  }

  // used for analyzing the file on import
  public void ReadToEnd () 
  {
    while (m_BaseStream.Position < m_BaseStream.Length)
    {
      try
      {
        ReadFrame();
      }
      catch
      {
        m_File.Flush();
        m_Stream.Close();
        m_File.Close();
        break;
      }
    }
  }

  public void ReadFrame()
  {
    m_FrameBeginStreamPosition = m_BaseStream.Position;
    if (m_BaseStream.Position >= m_BaseStream.Length)
      return;

    timestamp = m_Stream.ReadDouble();
    lightEstimate = m_Stream.ReadSingle();

    ReadPose();
    ReadPointCloud();
    ReadTrackedPlanes();
    ReadAnchors();
    // frame delimiter
    m_Stream.ReadChar();    

    TrackTime();
  }

  void TrackTime()
  {
    if (m_FirstPointCloudTimeStamp <= 0)
      m_FirstPointCloudTimeStamp = timestamp;
    else
      elapsed = timestamp - m_FirstPointCloudTimeStamp;

    // link this timestamp to the stream position before the frame
    long tempPos;
    if(!timePositions.TryGetValue(timestamp, out tempPos))
    {
      timePositions.Add(timestamp, m_FrameBeginStreamPosition);
      totalFrameCount++;
    }

  }

  void ReadPose()
  {
    Read(out position);
    Read(out rotation);
  }

  void ReadPointCloud()
  {
    pointCount = m_Stream.ReadInt32();
    for (int i = 0; i < pointCount; i++)
    {
      Read(out pointCloud[i]);
    }

    if (pointCount < pointCloud.Length)
    {
      // instead of clearing the array, set the remaining point to last one
      for (int n = pointCount; n < pointCloud.Length; n++)
      {
        pointCloud[n] = pointCloud[pointCount - 1];
      }
    }
  }

  void ReadTrackedPlanes()
  {
    planeCount = m_Stream.ReadInt32();

    for (int i = 0; i < planeCount; i++)
    {
      var vertexCount = m_Stream.ReadInt32();
      // TODO - don't allocate a new array here,  find a way to set length 
      planePolygons[i] = new Vector3[vertexCount];

      for (int n = 0; n < vertexCount; n++)
        Read(out planePolygons[i][n]);
    }
  }

  void ReadAnchors()
  {
    anchorCount = m_Stream.ReadInt32();

    if (anchorCount > 0)
    {
      for (int i = 0; i < anchorCount; i++)
      {
        Read(out anchorPositions[i]);
        Read(out anchorRotations[i]);
      }
    }
  }

  void Read(out Vector3 vec)
  {
    vec.x = m_Stream.ReadSingle();
    vec.y = m_Stream.ReadSingle();
    vec.z = m_Stream.ReadSingle();
  }

  void Read(out Quaternion quat)
  {
    quat.w = m_Stream.ReadSingle();
    quat.x = m_Stream.ReadSingle();
    quat.y = m_Stream.ReadSingle();
    quat.z = m_Stream.ReadSingle();
  }
    
  public void SeekToTime(double timeStamp)
  {
    long targetPosition;
    timePositions.TryGetValue(timeStamp, out targetPosition);
    SeekToPosition(targetPosition);
  }

  public void SeekToPosition(long position, bool readFrame = false)
  {
    m_BaseStream.Seek(position, SeekOrigin.Begin);
    Array.Clear(planePolygons, 0, planePolygons.Length);
    Array.Clear(anchorPositions, 0, anchorPositions.Length);
    Array.Clear(anchorRotations, 0, anchorRotations.Length);

    if(readFrame)
      ReadFrame();
  }

}
