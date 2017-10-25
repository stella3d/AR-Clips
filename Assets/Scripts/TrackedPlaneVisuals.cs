using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackedPlaneVisuals : MonoBehaviour
{
  [SerializeField]
  Mesh m_PlaneMesh;

  PointCloudPlayer m_Player;
  public Vector3[][] m_PlanePoints;
  public List<XRLineRenderer> planeRenderers;

  void Start()
  {
    m_Player = gameObject.GetComponent<PointCloudPlayer>();
    for (int i = 0; i < 20; i++)
    {
      var obj = new GameObject("plane outline");
      planeRenderers[i] = obj.AddComponent<XRLineRenderer>();
      planeRenderers[i].enabled = false;
    }

  }
	
  void Update()
  {
    var planePoints = m_Player.trackedPlanePolygons;

    for (int i = 0; i < planeRenderers.Count; i++)
    {
      if (planePoints[i][0] != Vector3.zero)
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

  void AddRenderer()
  {
    
  }

}

