﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnchorVisuals : ARClipVisual
{
  [SerializeField]
  Mesh m_PlaneMesh;

  public Material anchorMaterial;
  public float scaling = 0.125f;

  public MeshRenderer[] anchorRenderers;
  public GameObject[] anchorObjects;

  void Start()
  {
    anchorObjects = new GameObject[64];
    anchorRenderers = new MeshRenderer[64];

    for (int i = 0; i < 64; i++)
    {
      var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
      obj.transform.localScale *= scaling;
      obj.name = "anchor representation";
      anchorObjects[i] = obj;

      anchorRenderers[i] = obj.GetComponent<MeshRenderer>();
      anchorRenderers[i].enabled = false;

      if (anchorMaterial != null)
        anchorRenderers[i].material = anchorMaterial;
    }
  }

  void Update()
  {
    var positions = m_Reader.anchorPositions;
    var rotations = m_Reader.anchorRotations;

    for (int i = 0; i < positions.Length; i++)
    {
      if (positions[i] != null && rotations[i] != null && anchorRenderers.Length > i)
      {
        anchorRenderers[i].enabled = true;
        var obj = anchorObjects[i];
        obj.transform.SetPositionAndRotation(positions[i], rotations[i]);
      }
    }
  }

}


