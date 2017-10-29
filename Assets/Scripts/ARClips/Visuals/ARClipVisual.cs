using UnityEngine;

public class ARClipVisual : MonoBehaviour
{
  [SerializeField]
  protected ARClipReader m_Reader;

  protected void Start()
  {
    if (m_Reader == null)
      m_Reader = GetComponent<ARClipReader>();
  }
}

