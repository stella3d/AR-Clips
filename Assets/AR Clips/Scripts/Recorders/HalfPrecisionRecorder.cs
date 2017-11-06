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
			m_Stream.WriteShortFloatOne(quat.w);
			m_Stream.WriteShortFloatOne(quat.x);
			m_Stream.WriteShortFloatOne(quat.y);
			m_Stream.WriteShortFloatOne(quat.z);
		}
	}
}