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
    planeRenderers = new List<XRLineRenderer>();

    for (int i = 0; i < 20; i++)
    {
      var obj = new GameObject("plane outline");
      var render = obj.AddComponent<XRLineRenderer>();
      render.enabled = false;
      render.material = lineMaterial;
      planeRenderers.Add(render);
    }
  }
	
  void Update()
  {
    var planePoints = m_Reader.planePolygons;

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

