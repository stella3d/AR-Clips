using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ARClipReader : MonoBehaviour
{
  // increase this number to slow down playback
  public bool pauseWhenUnfocused = true;
  public int updatesPerDeviceUpdate;
  public ARClip clip;

  [Range(0.0f, 1.0f)]
  public float scrubByPercent;
  float previousScrubPercent;

  // these are just informational / to look at in the inspector
  public double elapsedSeconds;
  public double editorElapsedSeconds;
  public double editorTimeOffset;

  public int updateCount;
  public int pointCount;
  public int planeCount;
  public int anchorCount;

  public Vector3 position;
  public Vector3 previousPosition;
  public Quaternion rotation;
  public Quaternion previousRotation;
  public double timestamp;
  public float lightEstimate;

  public Vector3[] pointCloud 
  { 
    get { return m_Reader.pointCloud; } 
  }

  public Vector3[][] planePolygons 
  {
    get { return m_Reader.planePolygons; }
  }

  public Vector3[] anchorPositions
  {
    get { return m_Reader.anchorPositions; }
  }

  public Quaternion[] anchorRotations
  {
    get { return m_Reader.anchorRotations; }
  }

  int m_FrameSkipIndex;
  int nextTimestampIndex;
  double beginningTime;
  double beginningDeviceTime;
  double lastTimestamp;
  public double nextTimestamp;

  public Stopwatch timer = new Stopwatch();

  // our actual reading all goes on in here
  ARClipFileReader m_Reader;

  void Start () 
  {
    if (clip != null)
      m_Reader = new ARClipFileReader(clip);
    else
      Debug.LogWarning("no AR Clip asset assigned to reader component!");

    beginningDeviceTime = m_Reader.clip.timeStamps[0];
    
    updateCount = -1;  
    previousPosition = gameObject.transform.position;
    timer.Start();
  }

  void FixedUpdate () 
  {
    if (updateCount == -1)
    {      
      timer.Reset();
      timer.Start();
    }

    editorElapsedSeconds = (double)((double)(timer.ElapsedMilliseconds / (double)1000) + editorTimeOffset);
    if (scrubByPercent != previousScrubPercent)
    {
      SeekToRoundedPercent();
    }
    previousScrubPercent = scrubByPercent;

    // TODO - replace this with time-normalized playback.
    if(nextTimestamp - Time.deltaTime <= editorElapsedSeconds)
    {
      previousPosition = position;
      previousRotation = rotation;
      m_Reader.ReadFrame();
      CopyToInspector();
      nextTimestamp = GetNormalizedNextTime();
    }
    
  }

  double GetNormalizedTime(int index)
  {
    var times = m_Reader.clip.timeStamps;
    if (index < times.Length)
      return times[index] - times[0];
    else
      return (double)0;
  }

  double GetNormalizedNextTime()
  {
    return GetNormalizedTime(updateCount + 1);
  }

  void OnApplicationFocus(bool hasFocus)
  {
    if (pauseWhenUnfocused)
    {
      if (!hasFocus)
      {
        timer.Stop();
        Time.timeScale = 0f;
      }
      else if (pauseWhenUnfocused)
      {
        timer.Start();
        Time.timeScale = 1f;
      }
    }
  }

  void CopyToInspector()
  {
      elapsedSeconds = m_Reader.elapsed;
      updateCount = m_Reader.totalFrameCount;
      timestamp = m_Reader.timestamp;
      lightEstimate = m_Reader.lightEstimate;
      position = m_Reader.position;
      rotation = m_Reader.rotation;
      pointCount = m_Reader.pointCount;
      planeCount = m_Reader.planeCount;
      anchorCount = m_Reader.anchorCount;
  }

  void SeekToRoundedPercent()
  {
    var frame = (int)Mathf.Round(clip.frameCount * scrubByPercent);
    var clamped = Mathf.Clamp(frame, 0, clip.timeStampPositions.Length - 1);

    m_Reader.SeekToPosition(clip.timeStampPositions[clamped]);
    m_Reader.totalFrameCount = clamped;
    nextTimestampIndex = m_Reader.totalFrameCount;
    //timestamp = m_Reader.clip.timeStamps[nextTimestampIndex - 1];
    nextTimestamp = GetNormalizedTime(nextTimestampIndex);
    
    timer.Reset();
    timer.Start();
    editorTimeOffset = GetNormalizedTime(clamped);
  }

}
