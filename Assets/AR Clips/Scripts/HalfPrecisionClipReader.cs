using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class HalfPrecisionClipReader : ARClipFileReader
{
	protected override void Read(out Vector3 vec)
	{
		vec.x = m_Stream.ReadShortFloat();
		vec.y = m_Stream.ReadShortFloat();
		vec.z = m_Stream.ReadShortFloat();
	}

	protected override void Read(out Quaternion quat)
	{
		quat.w = m_Stream.ReadShortFloatOneClamped();
		quat.x = m_Stream.ReadShortFloatOneClamped();
		quat.y = m_Stream.ReadShortFloatOneClamped();
		quat.z = m_Stream.ReadShortFloatOneClamped();
	}
}
