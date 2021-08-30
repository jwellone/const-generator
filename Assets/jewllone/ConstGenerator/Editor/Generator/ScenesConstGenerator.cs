using System.IO;
using System.Collections.Generic;
using UnityEditor;

namespace jwellone.ConstGenerator.Editor
{
	public class ScenesConstGenerator : ConstGenerator
	{
		protected override string FileName
		{
			get
			{
				return "Scenes";
			}
		}

		protected override bool CanOnWillSaveAssetsWrite(in string[] saveAssetsPaths)
		{
			foreach (var path in saveAssetsPaths)
			{
				if (path.Contains("EditorBuildSettings.asset"))
				{
					return true;
				}
			}

			return false;
		}

		protected override void OnMakeHeader(in ConstGeneratorSettings settings)
		{
		}

		protected override void OnMakeBody(in ConstGeneratorSettings settings)
		{
			var sceneNameList = new List<string>();
			foreach (var scene in EditorBuildSettings.scenes)
			{
				if (!scene.enabled)
				{
					continue;
				}

				sceneNameList.Add(Path.GetFileNameWithoutExtension(scene.path));
			}

			for (var i = 0; i < sceneNameList.Count; ++i)
			{
				var name = sceneNameList[i].Replace(" ", "");
				if (string.IsNullOrEmpty(name))
				{
					continue;
				}
				Append("public const int ", true);
				Append(name);
				Append(" = ");
				Append(i.ToString());
				AppendLine(";");
			}

			AppendLine("");

			foreach (var name in sceneNameList)
			{
				if (string.IsNullOrEmpty(name))
				{
					continue;
				}

				Append("public static readonly string Label", true);
				Append(name.Replace(" ", ""));
				Append(" = \"");
				Append(name);
				AppendLine("\";");
			}
		}

		protected override void OnMakeFooter(in ConstGeneratorSettings settings)
		{
		}
	}
}
