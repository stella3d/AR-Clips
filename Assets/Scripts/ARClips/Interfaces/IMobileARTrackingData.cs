using System;
using UnityEngine;


// based on ARCore data right now, but should cover Kit & Core
public interface IMobileARTrackingData
{
  double timestamp { get; protected set; }

  float lightEstimate { get; protected set; }

  Vector3 devicePosition { get; protected set; }
  Quaternion deviceRotation { get; protected set; }

  Vector3[] pointCloud { get; protected set; }

  Vector3[][] trackedPlanePolygons { get; protected set; }

  Vector3[] anchorPositions { get; protected set; }
  Quaternion[] anchorRotations { get; protected set; }
}

