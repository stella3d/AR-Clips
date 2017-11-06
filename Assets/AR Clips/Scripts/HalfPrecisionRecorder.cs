using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

namespace ARcorder
{
	public class HalfPrecisionRecorder : ARClipRecorder
	{
		protected override void WriteVector3(Vector3 vec)
		{
			m_Stream.WriteShortFloat(vec.x);
			m_Stream.WriteShortFloat(vec.y);
			m_Stream.WriteShortFloat(vec.z);
		}

		protected override void WriteQuaternion(Quaternion quat)
		{
			m_Stream.WriteShortFloatOneClamped(quat.w);
			m_Stream.WriteShortFloatOneClamped(quat.x);
			m_Stream.WriteShortFloatOneClamped(quat.y);
			m_Stream.WriteShortFloatOneClamped(quat.z);
		}
	}

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

		public static short ShortFloat1Clamped(float f)
		{
			return (short)(Mathf.Clamp(f, 0f, 1f) * 32767f);
		}

		public static short Short1ClampedToFloat32(float f)
		{
			return (short)(Mathf.Clamp(f, 0f, 1f) * 32767f);
		}

	}

	public static class EncodingExtensions
	{
		static short m_ShortFloatCache;

		public static void WriteShortFloat(this BinaryWriter writer, float f)
		{
			writer.Write(Encode.Float32ToShortFloat(f));
		}

		public static void WriteShortFloatOneClamped(this BinaryWriter writer, float f)
		{
			writer.Write(Encode.ShortFloat1Clamped(f));
		}

		public static float ReadShortFloat(this BinaryReader reader)
		{
			return Encode.ShortFloatToFloat32(reader.ReadInt16());
		}

		public static void ReadShortFloatOneClamped(this BinaryReader reader)
		{
			return Encode.Short1ClampedToFloat32(reader.ReadInt16());
		}
	}

}