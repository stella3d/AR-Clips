using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace ARClips
{
  public static class CreateAssetUtility
  {
    [MenuItem("Assets/Create/Mobile AR Clip")]
    public static void CreateMobileARClip()
    {
      string path = AssetDatabase.GetAssetPath (Selection.activeObject);

      var clip = ScriptableObject.CreateInstance<ARClip> ();

      // parse all data & ignore end-of-stream error for now
      var reader = new ARClipFileReader(path);
      reader.ReadToEnd();

      clip.data = File.ReadAllBytes(path);
      clip.sizeInKilobytes = clip.data.Length / 1000;
      clip.lengthInSeconds = Math.Round(reader.elapsed, 3);
      clip.frameCount = reader.totalFrameCount;

      clip.timeStamps = reader.timePositions.Keys.ToArray();
      clip.timeStampPositions = reader.timePositions.Values.ToArray();

      if (path == "") 
        path = "Assets";
      else if (Path.GetExtension (path) != "") 
        path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");

      string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New ARClip.asset");

      AssetDatabase.CreateAsset (clip, assetPathAndName);
      AssetDatabase.SaveAssets ();
      EditorUtility.FocusProjectWindow ();
      Selection.activeObject = clip;
    }

    [MenuItem("Assets/Create/Color Palette")]
    public static void CreateColorPalette()
    {
      var palette = ScriptableObject.CreateInstance<ColorPalette> ();

      var path = "Assets/AR Clips/New Color Palette.asset";
      string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path);

      AssetDatabase.CreateAsset (palette, assetPathAndName);
      AssetDatabase.SaveAssets ();
      EditorUtility.FocusProjectWindow ();
    }

  }
}