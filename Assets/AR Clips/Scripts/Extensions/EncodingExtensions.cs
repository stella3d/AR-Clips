using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EncodingExtensions
{
	public static void WriteShortFloat(this BinaryWriter writer, float f)
	{
		writer.Write(f.ToShortFloat());
	}

	public static void WriteShortFloatOne(this BinaryWriter writer, float f)
	{
		writer.Write(f.ToShortFloatOne());
	}

	public static float ReadShortFloat(this BinaryReader reader)
	{
		return reader.ReadInt16().ShortFloatToSingle();
	}

	public static float ReadShortFloatOne(this BinaryReader reader)
	{
		return reader.ReadInt16().ShortFloatOneToSingle();
	}

	public static short ToShortFloat(this float f)
	{
		return (short)(f * 1000f);
	}

	public static short ToShortFloatOne(this float f)
	{
		return (short)(Mathf.Clamp(f, 0f, 1f) * 32767f);
	}

	public static float ShortFloatToSingle(this short s)
	{
		return (float)((float)s / 1000f);
	}

	public static float ShortFloatOneToSingle(this short s)
	{
		return (float)(Mathf.Clamp(s, 0, 1) * 32767);
	}
}

