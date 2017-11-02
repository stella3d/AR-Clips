using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnchorVisuals : ARClipVisual
{
    [SerializeField]
    Mesh m_PlaneMesh;

    [SerializeField]
    PrimitiveType m_PrimitiveType = PrimitiveType.Cube;

    [SerializeField]
    public Material anchorMaterial;

    [SerializeField]
    public float scaling = 0.125f;

    public List<MeshRenderer> anchorRenderers;
    public List<GameObject> anchorObjects;

    new void Start()
    {
        anchorObjects = new List<GameObject>();
        anchorRenderers = new List<MeshRenderer>();
    }

    void Update()
    {
        var positions = m_Reader.anchorPositions;
        var rotations = m_Reader.anchorRotations;

        for (int i = 0; i < positions.Length; i++)
        {
            if (m_Reader.anchorCount > anchorRenderers.Count)
            {
                Debug.Log("new anchor visual");
                for (int n = anchorRenderers.Count; n < positions.Length; n++)
                {
                    PlaceAnchorObject(positions[n], rotations[n]);
                }
            }

            if (anchorRenderers.Count > i)
            {
                anchorRenderers[i].enabled = true;
                var obj = anchorObjects[i];
                obj.transform.SetPositionAndRotation(positions[i], rotations[i]);
            }
        }
    }

    void PlaceAnchorObject(Vector3 position, Quaternion rotation)
    {
        var obj = GameObject.CreatePrimitive(m_PrimitiveType);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.localScale *= scaling;
        obj.name = "anchor representation";

        var renderer = obj.GetComponent<MeshRenderer>();
        if (anchorMaterial != null)
            renderer.material = anchorMaterial;

        anchorObjects.Add(obj);
        anchorRenderers.Add(renderer);
    }

}



