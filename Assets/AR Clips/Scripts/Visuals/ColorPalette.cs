using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorPalette : ScriptableObject 
{
	[SerializeField]
	public List<Color> colors = new List<Color>(24);
}
