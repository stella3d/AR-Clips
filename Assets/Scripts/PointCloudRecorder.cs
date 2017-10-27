using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.HelloAR;

namespace ARcorder
{
  public class PointCloudRecorder : MonoBehaviour 
  {
    public ArRecordingController m_Controller;

    const int MAX_POINT_COUNT = 15360;
    const string k_DefaultFileName = "/ar-record-new.arvcr";

    Mesh m_Mesh;

    FileStream file;
    BinaryWriter m_BinaryStream;

    int m_FrameIndex;
    double m_LastPointCloudTimestamp;

    Vector3[] m_Points = new Vector3[MAX_POINT_COUNT];

    List<Vector3> m_PlaneBoundaryCache = new List<Vector3>();


  	void Start () 
    {
      file = new FileStream(Application.persistentDataPath + k_DefaultFileName, FileMode.OpenOrCreate);
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

        // write all frame data in here
        WriteFrameDirect();
      }

      m_FrameIndex++;
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
      WriteAnchorData();

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

    void WriteAnchorData()
    {
      var anchors = m_Controller.anchors;

      m_BinaryStream.Write(anchors.Count);

      for (int i = 0; i < anchors.Count; i++)
      {
        var trans = anchors[i].transform;

        m_BinaryStream.Write(trans.position.x);
        m_BinaryStream.Write(trans.position.y);
        m_BinaryStream.Write(trans.position.z);

        m_BinaryStream.Write(trans.rotation.w);
        m_BinaryStream.Write(trans.rotation.x);
        m_BinaryStream.Write(trans.rotation.y);
        m_BinaryStream.Write(trans.rotation.z);
      }
    }
   
  }

}


