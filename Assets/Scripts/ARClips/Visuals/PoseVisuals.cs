using UnityEngine;
using System.Collections;

public class PoseVisuals : ARClipVisual
{
  public GameObject deviceObject;
	
  void Update()
  {
    deviceObject.transform.position = m_Reader.position;
    deviceObject.transform.rotation = m_Reader.rotation;
  }
}

