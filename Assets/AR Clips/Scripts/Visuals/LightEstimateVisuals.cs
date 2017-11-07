using UnityEngine;

namespace ARClips
{
  [RequireComponent(typeof(Light))]
  public class LightEstimateVisuals : ARClipVisual
  {
    Light m_Light;
    public float sensitivity = 2f;

    void Start()
    {
      base.Start();
      m_Light = GetComponent<Light>();
    }

    void Update()
    {
      m_Light.intensity = m_Reader.lightEstimate * sensitivity;
    }
  }
}
