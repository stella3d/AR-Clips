using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackedPlaneVisuals : ARClipVisual
{
  [SerializeField]
  Mesh m_PlaneMesh;

  public Material lineMaterial;
  public Vector3[][] m_PlanePoints;
  public List<XRLineRenderer> planeRenderers;

  void Start()
  {
    base.Start();
    for (int i = 0; i < 20; i++)
    {
      var obj = new GameObject("plane outline");
      planeRenderers[i] = obj.AddComponent<XRLineRenderer>();
      planeRenderers[i].enabled = false;
      planeRenderers[i].material = lineMaterial;
    }
  }
	
  void Update()
  {
    var planePoints = m_Reader.trackedPlanePolygons;

    for (int i = 0; i < planeRenderers.Count; i++)
    {
      //Debug.Log(planePoints[i]);

      if (planePoints[i] != null)
      {
        planeRenderers[i].enabled = true;
        planeRenderers[i].SetPositions(planePoints[i]);
      }
      else
      {
        planeRenderers[i].enabled = false;
      }
    }
  }

}

