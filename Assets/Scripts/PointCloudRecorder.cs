using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class PointCloudRecorder : MonoBehaviour 
{
  const int MAX_POINT_COUNT = 61440;

  Mesh m_Mesh;

  Vector3[] m_Points = new Vector3[MAX_POINT_COUNT];

  double m_LastPointCloudTimestamp;

  int m_FrameIndex;

  FileStream file;
  StreamWriter stream;

  BinaryWriter m_BinaryWriter;

  ArFrameRecord m_FrameRecord;


	void Start () 
  {
    file = new FileStream(Application.persistentDataPath + "/pointcloud.json", FileMode.OpenOrCreate);
    //stream = new StreamWriter(file);
    m_BinaryWriter = new BinaryWriter(file);
	}
	
	void Update () 
  {
    if (Frame.TrackingState != FrameTrackingState.Tracking)
      return;

    PointCloud pointcloud = Frame.PointCloud;
    if (pointcloud.PointCount > 0 && pointcloud.Timestamp > m_LastPointCloudTimestamp)
    {
      // Copy the point cloud points for mesh verticies.
      for (int i = 0; i < pointcloud.PointCount; i++)
      {
        m_Points[i] = pointcloud.GetPoint(i);
      }

      for (int i = pointcloud.PointCount; i < MAX_POINT_COUNT; i++)
      {
        m_Points[i] = m_Points[pointcloud.PointCount - 1];
      }


      m_LastPointCloudTimestamp = pointcloud.Timestamp;

      m_FrameRecord = new ArFrameRecord(Frame.Pose, m_Points);

      var start = new ThreadStart(() => { WriteFrame(m_FrameRecord); });
      new Thread(start).Start();

    }

    m_FrameIndex++;
	}

  void WriteFrame(ArFrameRecord record)
  {
    m_BinaryWriter.Write(m_FrameIndex);
    m_BinaryWriter.Write(' ');

    WriteVector3Binary(record.position);
    m_BinaryWriter.Write(' ');

    for (int i = 0; i < record.points.Length; i++)
    {
      WriteVector3Binary(record.points[i]);
      m_BinaryWriter.Write(',');
    }
    m_BinaryWriter.Write(' ');

    m_BinaryWriter.Write(';');
  }

  void WriteVector3Binary(Vector3 vec)
  {
    m_BinaryWriter.Write('x');
    m_BinaryWriter.Write(vec.x);

    m_BinaryWriter.Write('y');
    m_BinaryWriter.Write(vec.y);

    m_BinaryWriter.Write('z');
    m_BinaryWriter.Write(vec.z);
  }
}

[Serializable]
public struct ArFrameRecord
{
  public Vector3 position;
  public Quaternion rotation;
  public Vector3[] points;

  public ArFrameRecord(Pose trans, Vector3[] pointCloud)
  {
    this.position = trans.position;
    this.rotation = trans.rotation;
    points = pointCloud;
  }
}


