using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;

public class EncodingTests 
{
	const int arraySize = 10000;

	FileStream f32File;
	FileStream f16File;

	BinaryWriter f32Writer;
	BinaryWriter f16Writer;

	BinaryReader f32Reader;
	BinaryReader f16Reader;

	const string float32File = "float32s.txt";
	const string float16File = "float16s.txt";

	Vector3[] m_Vectors = new Vector3[arraySize];
	Quaternion[] m_Quaternions = new Quaternion[arraySize];


	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		for (int i = 0; i < arraySize; i++)
		{
			var x = i / 2000;
			var y = i / 3000;
			var z = i / 4000;
			m_Vectors[i] = new Vector3(x, y, z);
			m_Quaternions[i] = new Quaternion(i / 5000, x, y, z);
		}

		f32File = new FileStream(float32File, FileMode.OpenOrCreate);
		f16File = new FileStream(float16File, FileMode.OpenOrCreate);
		f32Writer = new BinaryWriter(f32File);
		f16Writer = new BinaryWriter(f16File);
	}

	[Test]
	public void ShortFloatSizeComparison() 
	{
		for (int i = 0; i < arraySize; i++)
		{
			var vec = m_Vectors[i];
			var quat = m_Quaternions[i];

			f16Writer.Write(Encode.Float32ToShortFloat(vec.x));
			f16Writer.Write(Encode.Float32ToShortFloat(vec.y));
			f16Writer.Write(Encode.Float32ToShortFloat(vec.z));
			f16Writer.Write(Encode.Float32ToShortFloat(quat.w));
			f16Writer.Write(Encode.Float32ToShortFloat(quat.x));
			f16Writer.Write(Encode.Float32ToShortFloat(quat.y));
			f16Writer.Write(Encode.Float32ToShortFloat(quat.z));

			f32Writer.Write(vec.x);
			f32Writer.Write(vec.y);
			f32Writer.Write(vec.z);
			f32Writer.Write(quat.w);
			f32Writer.Write(quat.z);
			f32Writer.Write(quat.y);
			f32Writer.Write(quat.z);
		}

		f16Writer.Flush();
		f16File.Flush();
		f32Writer.Flush();
		f32File.Flush();
	}

	[Test]
	public void ShortFloatReadTest() 
	{
		f16Reader = new BinaryReader(f16File);

		for (int i = 0; i < arraySize; i++)
		{
			Vector3 vec;
			vec.x = f16Reader.ReadShortFloat();
			vec.y = f16Reader.ReadShortFloat();
			vec.z = f16Reader.ReadShortFloat();
			Debug.Log(vec.ToString("F8"));
			Quaternion quat;
			quat.w = f16Reader.ReadShortFloat();
			quat.x = f16Reader.ReadShortFloat();
			quat.y = f16Reader.ReadShortFloat();
			quat.z = f16Reader.ReadShortFloat();
			//Debug.Log(quat.ToString("F3"));
		}
	}

	[Test]
	public void BasicShortToFloatConversionTest()
	{
		const short value = 24201;
		float f = Encode.ShortFloatToFloat32(value);
		Debug.Log(f);
		Assert.AreEqual(24.201f, f);
	}

	[Test]
	public void BasicQuaternionFloatToShortTest()
	{
		const float value = 0.666f;
		short s = Encode.ShortFloat1Clamped(value);
		Debug.Log(s);
		Assert.AreEqual((short)21822, s);
	}

}

/* 
public struct ShortVector3()
{
	public short x;
	public short y;
	public short z;

	public ShortVector3(Vector3 v)
	{
		x = (short)(v.x * 1000);
		y = (short)(v.y * 1000);
		z = (short)(v.z * 1000);
	}
	
}

public struct ShortQuaternion()
{
	public short w;
	public short x;
	public short y;
	public short z;
 
	public ShortQuaternion(Quaternion q)
	{
		w = (short)(q.w * 1000);
		x = (short)(q.x * 1000);
		y = (short)(q.y * 1000);
		z = (short)(q.z * 1000);
	}
}
*/

public static class Encode
{
	public static short Float32ToShortFloat(float f)
	{
		return (short)(f * 1000f);
	}

	public static float ShortFloatToFloat32(short s)
	{
		return (float)((float)s / 1000f);
	}

	const double floatToShort1Ratio = 0.0000305185;

	public static short ShortFloat1Clamped(float f)
	{
		return (short)(f * 32767f);
	}

}

public static class EncodingExtensions
{
	static short m_ShortFloatCache;

	public static void WriteShortFloat(this BinaryWriter writer, float f)
	{
		short s = Encode.Float32ToShortFloat(f);
		//writer.Write(); 
	}

	public static float ReadShortFloat(this BinaryReader reader)
	{
		return Encode.ShortFloatToFloat32(reader.ReadInt16());
	}
}
