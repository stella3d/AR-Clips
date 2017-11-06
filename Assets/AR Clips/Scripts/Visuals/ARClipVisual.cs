using UnityEngine;

namespace  ARClips
{
  public class ARClipVisual : MonoBehaviour
  {
    protected ARClipReader m_Reader;

    protected void Start()
    {
      if (m_Reader == null)
        m_Reader = GetComponent<ARClipReader>();
    }
  }
}