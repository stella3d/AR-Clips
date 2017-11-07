using UnityEngine;

namespace  ARClips
{
  public abstract class ARClipVisual : MonoBehaviour
  {
    protected ARClipReader m_Reader;

    protected virtual void Start()
    {
      if (m_Reader == null)
        m_Reader = GetComponent<ARClipReader>();
    }
  }
}