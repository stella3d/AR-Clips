using UnityEngine;
using System.Collections;

public class PoseVisuals : ARClipVisual
{
  public GameObject deviceObject;

  int skipIndex = 1;
	
  void Update()
  {
    deviceObject.transform.rotation = m_Reader.rotation;

    var skip = m_Reader.updatesPerDeviceUpdate;
    if (skip > 1)
    {
      var trans = deviceObject.transform;

      trans.position = Vector3.Lerp(trans.position, m_Reader.position, skipIndex / skip);
      trans.rotation = Quaternion.Lerp(trans.rotation, m_Reader.rotation, skipIndex / skip);

      if (skipIndex < skip)
        skipIndex++;
      else if(skipIndex == skip)
        skipIndex = 1;
    }
  }
}

