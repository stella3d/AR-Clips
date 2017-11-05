using System;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

namespace ARcorder
{
  public class ARClipRecorder : MonoBehaviour 
  {
    public ArRecordingController m_Controller;

    const int k_MaxPoints = 15360;
    const string k_FileExtension = ".arclip";

    FileStream m_File;
    GZipStream m_GzipStream;
    BufferedStream m_Buffer;
    BinaryWriter m_Stream;

    double m_LastCloudTimestamp;

    Vector3[] m_Points = new Vector3[k_MaxPoints];

    List<Vector3> m_PlaneBoundaryCache = new List<Vector3>();

  	void Start () 
    {
      // how many seconds have we been living in the Willenium ?
      var time = (int)(DateTime.UtcNow - new DateTime (2000, 1, 1)).TotalSeconds;
      var fileName = time + k_FileExtension;
      var path = Path.Combine(Application.persistentDataPath, fileName);

      m_File = new FileStream(path, FileMode.OpenOrCreate);
      m_GzipStream = new GZipStream(m_File, CompressionMode.Compress);
      m_Buffer = new BufferedStream(m_GzipStream, 65536);
      m_Stream = new BinaryWriter(m_GzipStream);

      m_Controller = gameObject.GetComponent<ArRecordingController>();
  	}

  	void Update () 
    {
      if (Frame.TrackingState != FrameTrackingState.Tracking)
        return;

      PointCloud cloud = Frame.PointCloud;
      if (cloud.PointCount > 0 && cloud.Timestamp > m_LastCloudTimestamp)
      {
        Array.Clear(m_Points, 0, m_Points.Length);
        for (int i = 0; i < cloud.PointCount; i++)
        {
          m_Points[i] = cloud.GetPoint(i);
        }

        m_LastCloudTimestamp = cloud.Timestamp;

        // write all frame data in here
        RecordFrame();
      }
  	}

    void OnApplicationPause()
    {
      m_Stream.Flush();
      m_Buffer.Flush();
      m_GzipStream.Flush();
      m_File.Flush();
    }

    void OnApplicationSuspend()
    {
      OnApplicationPause();
    }

    /*
     * In the stream protocol, there are only arrays & primitives.
     * Every array is preceded by an int of the array length.

     * 
      Frames are written like this for now, but the data structure
      should be defined by a header at the beginning of the stream 

      in order:

      timestamp ( double )
      light estimate ( float )

      device position ( Vector3 )
      device rotation ( Quaternion )
      
      # of tracked planes ( int )
      for each tracked plane:
        # of points in the boundary polygon ( int )
        all points in the polygon ( Vector3[vertexCount] )

      # of points in the point cloud ( int )
      all points in the cloud  ( Vector3[pointCount] )
      
      frame delimiter ( ; )
    */

    void RecordFrame()
    {
      m_Stream.Write(Frame.Timestamp);
      m_Stream.Write(Frame.LightEstimate.PixelIntensity);

      WritePoseData();
      WritePointCloudData();
      WritePlaneData();
      WriteAnchorData();

      // ; is our frame delimiter, which allows aligning
      // to a frame no matter where you start getting data
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
          WriteVector3(m_PlaneBoundaryCache[n]);
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


