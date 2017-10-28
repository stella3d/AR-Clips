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

    FileStream m_File;
    BinaryWriter m_Stream;

    int m_FrameIndex;
    double m_LastPointCloudTimestamp;

    Vector3[] m_Points = new Vector3[MAX_POINT_COUNT];

    List<Vector3> m_PlaneBoundaryCache = new List<Vector3>();


  	void Start () 
    {
      m_File = new FileStream(Application.persistentDataPath + k_DefaultFileName, FileMode.OpenOrCreate);
      m_Stream = new BinaryWriter(m_File);
      m_Controller = gameObject.GetComponent<ArRecordingController>();
  	}

  	void Update () 
    {
      m_FrameIndex++;
      if (Frame.TrackingState != FrameTrackingState.Tracking)
        return;

      PointCloud cloud = Frame.PointCloud;
      if (cloud.PointCount > 0 && cloud.Timestamp > m_LastPointCloudTimestamp)
      {
        Array.Clear(m_Points, 0, m_Points.Length);
        for (int i = 0; i < cloud.PointCount; i++)
        {
          m_Points[i] = cloud.GetPoint(i);
        }

        m_LastPointCloudTimestamp = cloud.Timestamp;

        // write all frame data in here
        RecordFrame();
      }
  	}

    /*
      Frames are written like this for now:

      frame index (int)
      timestamp (double)
      light estimate (float)
      pose / phone position (transform)
      
      # of tracked planes (int)
      for each tracked plane:
        # of points in the boundary polygon (int)
        all points in the polygon (Vector3[])

      # of points in the point cloud (int)
      all points in the cloud  (Vector3[])
    */

    void RecordFrame()
    {
      m_Stream.Write(m_FrameIndex);
      m_Stream.Write(Frame.Timestamp);
      m_Stream.Write(Frame.LightEstimate.PixelIntensity);

      WritePoseData();
      WritePointCloudData();
      WritePlaneData();
      WriteAnchorData();

      // not really needed, more for humans / debugging
      m_Stream.Write(';'); 
    }

    void WritePoseData()
    {
      WriteVector3(Frame.Pose.position);
      WriteQuaternion(Frame.Pose.rotation);
    }

    void WritePointCloudData()
    {
      m_Stream.Write(Frame.PointCloud.PointCount);

      for (int i = 0; i < Frame.PointCloud.PointCount; i++)
      {
        WriteVector3(m_Points[i]);
      }
    }

    void WritePlaneData()
    {
      var planes = m_Controller.trackedPlanes;
      var planeCount = planes.Count;

      m_Stream.Write(planeCount);

      for (int i = 0; i < planeCount; i++)
      {
        planes[i].GetBoundaryPolygon(ref m_PlaneBoundaryCache);
        var pointCount = m_PlaneBoundaryCache.Count;

        m_Stream.Write(pointCount);

        for (int n = 0; n < pointCount; n++)
        {
          var vec = m_PlaneBoundaryCache[n];
        }
      }
    }

    void WriteAnchorData()
    {
      var anchors = m_Controller.anchors;

      m_Stream.Write(anchors.Count);

      for (int i = 0; i < anchors.Count; i++)
      {
        var trans = anchors[i].transform;
        WriteVector3(trans.position);
        WriteQuaternion(trans.rotation);
      }
    }

    void WriteVector3(Vector3 vec)
    {
      m_Stream.Write(vec.x);
      m_Stream.Write(vec.y);
      m_Stream.Write(vec.z);
    }

    void WriteQuaternion(Quaternion quat)
    {
      m_Stream.Write(quat.w);
      m_Stream.Write(quat.x);
      m_Stream.Write(quat.y);
      m_Stream.Write(quat.z);
    }
   
  }

}


