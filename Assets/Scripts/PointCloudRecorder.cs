using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.HelloAR;

public class PointCloudRecorder : MonoBehaviour 
{
  public ArRecordingController m_Controller;

  const int k_StreamBufferSize = 1024 * 16;
  const double k_StreamFlushPercent = .75;

  const int MAX_POINT_COUNT = 15360;
  const string k_DefaultFileName = "/ar-record-new.arvcr";

  Mesh m_Mesh;

  FileStream file;
  MemoryStream buffer;
  BinaryWriter m_BinaryStream;

  int m_FrameIndex;
  double m_LastPointCloudTimestamp;

  ArFrameRecord m_FrameRecord;
  Vector3[] m_Points = new Vector3[MAX_POINT_COUNT];

  List<Vector3> m_PlaneBoundaryCache = new List<Vector3>();


	void Start () 
  {
    file = new FileStream(Application.persistentDataPath + k_DefaultFileName, FileMode.OpenOrCreate);
    buffer = new MemoryStream(k_StreamBufferSize);
    m_BinaryStream = new BinaryWriter(file);

    m_Controller = gameObject.GetComponent<ArRecordingController>();
	}
	
	void Update () 
  {
    if (Frame.TrackingState != FrameTrackingState.Tracking)
      return;

    PointCloud pointcloud = Frame.PointCloud;
    if (pointcloud.PointCount > 0 && pointcloud.Timestamp > m_LastPointCloudTimestamp)
    {
      // Copy the point cloud points for mesh verticies.
      Array.Clear(m_Points, 0, m_Points.Length);
      for (int i = 0; i < pointcloud.PointCount; i++)
      {
        m_Points[i] = pointcloud.GetPoint(i);
      }

      m_LastPointCloudTimestamp = pointcloud.Timestamp;

      //var start = new ThreadStart(() => { WriteFrame(m_FrameRecord); });
      //new Thread(start).Start();
      WriteFrameDirect();

      /* is the buffer more than %75 full ?
      if(buffer.Position > k_StreamBufferSize * k_StreamFlushPercent)
      {
        buffer.WriteTo(file);
        buffer.Flush();
        buffer.Position = 0;
      }
      */
    }

    m_FrameIndex++;
	}

  void WriteFrame(ArFrameRecord record)
  {
    m_BinaryStream.Write(m_FrameIndex);

    m_BinaryStream.Write(record.position.x);
    m_BinaryStream.Write(record.position.y);
    m_BinaryStream.Write(record.position.z);

    for (int i = 0; i < record.points.Length; i++)
    {
      var vec = record.points[i];
      m_BinaryStream.Write(vec.x);
      m_BinaryStream.Write(vec.y);
      m_BinaryStream.Write(vec.z);
    }

    m_BinaryStream.Write(';'); // probably not needed
  }

  /*
    Frames are written like this:

    frame index

    pose / phone position

    # of tracked planes
    
    for each tracked plane:
      # of points in the boundary polygon
      all points in the polygon

    # of points in the point cloud
    
    all points in the cloud
  */
  void WriteFrameDirect()
  {
    m_BinaryStream.Write(m_FrameIndex);

    WritePoseData();
    WritePlaneData();
    WritePointCloudData();
    // not really needed, more for humans / debugging
    m_BinaryStream.Write(';'); 
  }

  void WritePointCloudData()
  {
    var cloud = Frame.PointCloud;
    m_BinaryStream.Write(cloud.PointCount);

    for (int i = 0; i < cloud.PointCount; i++)
    {
      var vec = m_Points[i];
      m_BinaryStream.Write(vec.x);
      m_BinaryStream.Write(vec.y);
      m_BinaryStream.Write(vec.z);
    }
  }

  void WritePoseData()
  {
    var pose = Frame.Pose;

    m_BinaryStream.Write(pose.position.x);
    m_BinaryStream.Write(pose.position.y);
    m_BinaryStream.Write(pose.position.z);

    m_BinaryStream.Write(pose.rotation.w);
    m_BinaryStream.Write(pose.rotation.x);
    m_BinaryStream.Write(pose.rotation.y);
    m_BinaryStream.Write(pose.rotation.z);
  }

  void WritePlaneData()
  {
    var planes = m_Controller.trackedPlanes;
    var planeCount = planes.Count;

    m_BinaryStream.Write(planeCount);

    for (int i = 0; i < planeCount; i++)
    {
      planes[i].GetBoundaryPolygon(ref m_PlaneBoundaryCache);
      var pointCount = m_PlaneBoundaryCache.Count;

      m_BinaryStream.Write(pointCount);

      for (int n = 0; n < pointCount; n++)
      {
        var vec = m_PlaneBoundaryCache[n];
        m_BinaryStream.Write(vec.x);
        m_BinaryStream.Write(vec.y);
        m_BinaryStream.Write(vec.z);
      }
    }
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


