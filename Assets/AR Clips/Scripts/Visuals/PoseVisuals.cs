using UnityEngine;
using System.Collections;

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

    timeDiff = m_Reader.nextTimestamp - m_Reader.elapsedSeconds;
    var timeDiffElapsed = (float)timeDiff / (float)(m_Reader.nextTimestamp - m_Reader.editorElapsedSeconds);
    //var rate = 1f / timeDiffElapsed;
    lerp = timeDiffElapsed;
    //lerp = (float)rate * Time.deltaTime;
    lerp = Mathf.Clamp(lerp, 0f, 1f);
    //if (skip > 1)
    //if (lerp < 1f)
    //{
      trans.position = Vector3.Slerp(m_Reader.previousPosition, m_Reader.position, lerp);
      trans.rotation = Quaternion.Lerp(trans.rotation, m_Reader.rotation, lerp);
    //}

    if (lerp >= 1f)
      lerp = 0;

/* 
      if (skipIndex >= 1)
        skipIndex--;
      else if(skipIndex == 0)
        skipIndex = m_Reader.updatesPerDeviceUpdate;
        */
    //}
    /*else if (skip == 1)
    {
      trans.position = m_Reader.position;
      trans.rotation = m_Reader.rotation;
    }
    

    if (lerp < 1)
      lerp += 1 / skip;
    else
      lerp = 0;
    */
  }
}

