using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TrackedPlaneVisuals : ARClipVisual
{
    [SerializeField]
    Mesh m_PlaneMesh;

    public Material lineMaterial;
    public Vector3[][] m_PlanePoints;
    public Vector3[][] m_PreviousPlanePoints;
    public List<XRLineRenderer> planeRenderers;

    new void Start()
    {
        base.Start();
        planeRenderers = new List<XRLineRenderer>();

        m_PreviousPlanePoints = new Vector3[16000][];

        for (int i = 2; i < 20; i++)
        {
            var obj = new GameObject("plane outline");
            var render = obj.AddComponent<XRLineRenderer>();
            
            render.loop = true;
            render.SetTotalWidth(0.25f);
            render.material.SetVector("_lineSettings", new Vector4(0f, 0.05f, 0.1f, 0f));

            var materialCopy = new Material(lineMaterial);
            var color = materialCopy.color;
            color.r -= i / 4;
            color.g += i / 6;
            color.b += i / 4;
            materialCopy.color = new Color(color.r, color.g, color.b);

            render.materials = new [] { materialCopy };
            render.materials[0] = materialCopy;

            render.enabled = false;
            planeRenderers.Add(render);
        }
    }

    float lerp;
	
    void Update()
    {
        var planePoints = m_Reader.planePolygons;

        for (int i = 0; i < planeRenderers.Count; i++)
        {
            if (planePoints[i] != null)
            {
                if (m_PreviousPlanePoints[i] != null)
                {
                    lerp += 0.125f / 2;
                    //Debug.Log("previous found");
                    if(planePoints[i].Length == m_PreviousPlanePoints[i].Length)
                    {
                        //Debug.Log("equal length!");
                        for (int n = 0; n < planePoints[i].Length; n++)
                        {
                            var newPoint = planePoints[i][n];
                            var oldPoint = m_PreviousPlanePoints[i][n];
                            planePoints[i][n] = Vector3.Lerp(oldPoint, newPoint, lerp);
                        }
                    }
                    else
                        Array.Resize<Vector3>(ref m_PreviousPlanePoints[i], planePoints[i].Length);

                    if (lerp >= 0.4f)
                        lerp = 0f;
                }
                planeRenderers[i].enabled = true;
                planeRenderers[i].SetPositions(planePoints[i], true);
            }
            else
            {
                planeRenderers[i].enabled = false;
            }

        }

        planePoints.CopyTo(m_PreviousPlanePoints, 0);        
    }

}

