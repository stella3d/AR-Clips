using System.Collections.Generic;

public interface IARClipReader
{
  Dictionary<double, long> timePositions { get; }

  void ReadFrame();

  void SeekToTime(double timeStamp);
}
