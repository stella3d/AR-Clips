using System.IO;
using UnityEngine;

namespace ARClips
{
	public class HalfPrecisionClipReader : ARClipFileReader
	{
		public HalfPrecisionClipReader(ARClip clip): base(clip) {}

		protected override void Read(out Vector3 vec)
		{
			vec.x = m_Stream.ReadShortFloat();
			vec.y = m_Stream.ReadShortFloat();
			vec.z = m_Stream.ReadShortFloat();
		}

		protected override void Read(out Quaternion quat)
		{
			quat.w = m_Stream.ReadShortFloatOne();
			quat.x = m_Stream.ReadShortFloatOne();
			quat.y = m_Stream.ReadShortFloatOne();
			quat.z = m_Stream.ReadShortFloatOne();
		}
	}
}
