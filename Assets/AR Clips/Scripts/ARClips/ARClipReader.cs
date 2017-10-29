using UnityEngine;
using UnityEditor;

public class ARClipReader : MonoBehaviour
{
  // increase this number to slow down playback
  public int updatesPerDeviceUpdate;
  public ARClip clip;
  public string fileSource;

  [Range(0.0f, 1.0f)]
  public float scrubByPercent;
  float previousScrubPercent;

 
  // these are just informational / to look at in the inspector
  public double elapsedSeconds;
  public int updateCount;
  public int pointCount;
  public int planeCount;
  public int anchorCount;

  public Vector3 position;
  public Quaternion rotation;
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

  // our actual reading all goes on in here
  ARClipFileReader m_Reader;

  void Start () 
  {
    if (clip != null)
    {
      m_Reader = new ARClipFileReader(clip);
      fileSource = "";
      Debug.Log("reading AR Clip data from asset");
    }
    else
    {
      m_Reader = new ARClipFileReader(fileSource);
      Debug.Log("reading AR Clip data from file");
    }
  }

  void Update () 
  {
    if (scrubByPercent != previousScrubPercent)
    {
      Debug.Log("seeking to: " + scrubByPercent); 
      SeekToRoundedPercent();
    }

    m_FrameSkipIndex++;
    if (m_FrameSkipIndex % updatesPerDeviceUpdate == 0)
    {
      m_Reader.ReadFrame();

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

    previousScrubPercent = scrubByPercent;
  }

  void SeekToRoundedPercent()
  {
    var frame = (int)Mathf.Round(clip.frameCount * scrubByPercent);
    var clamped = Mathf.Clamp(frame, 0, clip.timeStampPositions.Length - 1);
    Debug.Log(clamped);

    m_Reader.SeekToPosition(clip.timeStampPositions[clamped]);
    m_Reader.totalFrameCount = clamped;
  }

}
