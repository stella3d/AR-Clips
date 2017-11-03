using UnityEngine;
using System.Collections;

public class PoseVisuals : ARClipVisual
{
  public GameObject deviceObject;
  Vector3 m_Velocity;

  int skip;
  int skipIndex;
  int lerp = 0;

  new void Start()
  {
    skipIndex = m_Reader.updatesPerDeviceUpdate;
  }
	
  void Update()
  {
    var trans = deviceObject.transform;

    skip = m_Reader.updatesPerDeviceUpdate;
    if (skip > 1)
    {
      trans.position = Vector3.SmoothDamp(trans.position, m_Reader.position, ref m_Velocity, lerp);
      trans.rotation = Quaternion.Lerp(trans.rotation, m_Reader.rotation, lerp + 0.1f);

      if (skipIndex >= 1)
        skipIndex--;
      else if(skipIndex == 0)
        skipIndex = m_Reader.updatesPerDeviceUpdate;
    }
    else if (skip == 1)
    {
      trans.position = m_Reader.position;
      trans.rotation = m_Reader.rotation;
    }

    if (lerp < 1)
      lerp += 1 / skip;
  }
}

