using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARClips
{
  [Serializable]
  [PreferBinarySerialization]
  public class ARClip : ScriptableObject
  {
    public byte[] data;
    public double sizeInKilobytes;
    public double lengthInSeconds;
    public int frameCount;

    // supports scrubbing - maps device timestamp to stream position
    public double[] timeStamps;
    public long[] timeStampPositions;

    public Dictionary<double, long> timeLookup;

    public ARClip () 
    {
      if (timeStamps != null)
      {
        timeLookup = new Dictionary<double,long>();
        for (int i = 0; i < timeStamps.Length; i++)
          timeLookup.Add(timeStamps[i], timeStampPositions[i]);
      }
    }

  }
}