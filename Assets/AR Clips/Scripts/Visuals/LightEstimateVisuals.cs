using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightEstimateVisuals : ARClipVisual
{
  public Light light;
  public float sensitivity = 2f;

  void Update()
  {
    light.intensity = m_Reader.lightEstimate * sensitivity;
  }
}

