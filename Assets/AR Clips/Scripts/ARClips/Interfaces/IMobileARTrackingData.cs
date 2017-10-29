using System;
using UnityEngine;

// based on ARCore data right now, but should cover Kit & Core
public interface IMobileARTrackingData
{
  double timestamp { get; set; }

  float lightEstimate { get; set; }

  Vector3 devicePosition { get; set; }
  Quaternion deviceRotation { get; set; }

  Vector3[] pointCloud { get; set; }

  Vector3[][] trackedPlanePolygons { get; set; }

  Vector3[] anchorPositions { get; set; }
  Quaternion[] anchorRotations { get; set; }
}

