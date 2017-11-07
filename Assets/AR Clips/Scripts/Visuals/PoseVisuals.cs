using UnityEngine;

namespace ARClips
{
  public class PoseVisuals : ARClipVisual
  {
    public GameObject deviceObject;
    float m_Lerp;
    
    void Update()
    {
        var trans = deviceObject.transform;
        var timeDiff = m_Reader.nextTimestamp - m_Reader.elapsedSeconds - m_Reader.editorTimeOffset;
        
        m_Lerp = (float)timeDiff / (float)(m_Reader.nextTimestamp - m_Reader.editorElapsedSeconds);
        m_Lerp = Mathf.Clamp(m_Lerp, 0f, 1f);

        trans.position = Vector3.Slerp(m_Reader.previousPosition, m_Reader.position, m_Lerp);
        trans.rotation = Quaternion.Lerp(m_Reader.previousRotation, m_Reader.rotation, m_Lerp);
    }
  }
}
