using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.IO;
using System.IO.Compression;
using System.Collections;

public class StreamSizeTests 
{
  
  FileStream m_File;
  FileStream m_GzipFile;
  BufferedStream m_Buffer;
  GZipStream m_GzipStream;
  BinaryWriter m_Writer;
  BinaryWriter m_GzipWriter;

  [OneTimeSetUp]
  public void Setup()
  {
    var fileName = Application.dataPath + "/uncompressed.txt";
    var gzipFileName = Application.dataPath + "/compressed.txt";

    m_File = new FileStream(fileName, FileMode.CreateNew);
    m_GzipFile = new FileStream(gzipFileName, FileMode.CreateNew);
    m_GzipStream = new GZipStream(m_GzipFile, CompressionMode.Compress);
    m_Buffer = new BufferedStream(m_GzipStream, 65536);

    m_Writer = new BinaryWriter(m_File);
    m_GzipWriter = new BinaryWriter(m_Buffer);
  }

	[Test]
	public void WriteBothStreams() 
  {
    // our real data is mostly lots of floats puncuated by int32s
    for (float i = 0; i < 1000000; i+= 0.5f)
    {
      var x = i / 2f;
      var y = i / 4f;
      var z = i / 8f;
      m_Writer.Write(x);
      m_Writer.Write(y);
      m_Writer.Write(z);
      m_GzipWriter.Write(x);
      m_GzipWriter.Write(y);
      m_GzipWriter.Write(z);

      if (i % 100 == 0)
      {
        m_Writer.Write((int) i / 100);
        m_Writer.Write(';');
        m_GzipWriter.Write((int) i / 100);
        m_GzipWriter.Write(';');
      }
    }

    m_Writer.Flush();
    m_GzipWriter.Flush();
    m_Writer.Close();
    m_GzipWriter.Close();
    m_File.Close();
    m_GzipFile.Close();
	}

  [Test]
  public void WriteRealStreamCopy() 
  {
    var fs = new FileStream("Assets/demo2.arclip.gzip", FileMode.OpenOrCreate);
    m_GzipStream = new GZipStream(fs, CompressionMode.Compress);
    m_Buffer = new BufferedStream(m_GzipStream, 65536);
    m_GzipWriter = new BinaryWriter(m_Buffer);

    var bytes = File.ReadAllBytes("Assets/demo2.arclip");

    //for (var i = 0; i < bytes.Length; i++)
    //{
    m_GzipWriter.Write(bytes);
    //}

    m_GzipWriter.Flush();
    m_GzipWriter.Close();
    fs.Close();
  }
    
}
