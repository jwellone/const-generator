using System.Reflection;
using UnityEditorInternal;

namespace jwellone.ConstGenerator.Editor
{
	public class SortingLayersConstGenerator : ConstGenerator
	{
		protected override string FileName
		{
			get
			{
				return "SortingLayers";
			}
		}

		protected override bool CanOnWillSaveAssetsWrite(in string[] saveAssetsPaths)
		{
			foreach (var path in saveAssetsPaths)
			{
				if (path.Contains("TagManager.asset"))
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
			var property = typeof(InternalEditorUtility).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			var names = property.GetValue(null, new object[0]) as string[];
			foreach (var name in names)
			{
				Append("public static readonly string ", true);
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
