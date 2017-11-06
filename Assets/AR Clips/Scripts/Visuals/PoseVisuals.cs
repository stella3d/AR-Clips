using UnityEngine;
using System.Collections;

namespace ARClips
{
  public class PoseVisuals : ARClipVisual
  {
    public GameObject deviceObject;
    Vector3 m_Velocity;

    public int skip;
    int skipIndex;
    public float lerp = 0;

    public double timeDiff;

    new void Start()
    {
      skipIndex = m_Reader.updatesPerDeviceUpdate;
    }
    
    Vector3 previousPos;

    void Update()
    {
        var trans = deviceObject.transform;

        timeDiff = m_Reader.nextTimestamp - m_Reader.elapsedSeconds - m_Reader.editorTimeOffset;
        var timeDiffElapsed = (float)timeDiff / (float)(m_Reader.nextTimestamp - m_Reader.editorElapsedSeconds);
        lerp = timeDiffElapsed;
        lerp = Mathf.Clamp(lerp, 0f, 1f);

        trans.position = Vector3.Slerp(m_Reader.previousPosition, m_Reader.position, lerp);
        trans.rotation = Quaternion.Lerp(m_Reader.previousRotation, m_Reader.rotation, lerp);
    }
  }
}
