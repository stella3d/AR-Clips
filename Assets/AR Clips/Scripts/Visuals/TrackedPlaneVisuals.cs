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

    new void Start()
    {
        base.Start();
        planeRenderers = new List<XRLineRenderer>();

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
	
    void Update()
    {
        var planePoints = m_Reader.planePolygons;

        for (int i = 0; i < planeRenderers.Count; i++)
        {
            if (planePoints[i] != null)
            {
                planeRenderers[i].enabled = true;
                planeRenderers[i].SetPositions(planePoints[i], true);
            }
            else
            {
                planeRenderers[i].enabled = false;
            }
        }
    }

}

