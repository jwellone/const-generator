using System.Linq;
using UnityEngine;
using UnityEditorInternal;

namespace jwellone.ConstGenerator.Editor
{
	public class LayersConstGenerator : ConstGenerator
	{
		protected override string FileName
		{
			get
			{
				return "Layers";
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
			AppendLine("using UnityEngine;");
			AppendLine("");
		}

		protected override void OnMakeBody(in ConstGeneratorSettings settings)
		{
			var layers = InternalEditorUtility.layers.ToList();
			foreach (var layer in layers)
			{
				var no = LayerMask.NameToLayer(layer);
				Append("public const int ", true);
				Append(layer.Replace(" ", ""));
				Append(" = ");
				Append(no.ToString());
				AppendLine(";");
			}

			AppendLine("");

			foreach (var layer in layers)
			{
				var no = LayerMask.NameToLayer(layer);
				Append("public const int Mask", true);
				Append(layer.Replace(" ", ""));
				Append(" = ");
				Append((1 << no).ToString());
				AppendLine(";");
			}

			AppendLine("");

			foreach (var layer in layers)
			{
				var no = LayerMask.NameToLayer(layer);
				Append("public static readonly string Label", true);
				Append(layer.Replace(" ", ""));
				Append(" = \"");
				Append(layer);
				AppendLine("\";");
			}

			AppendLine("");

			AppendLine("public static int NameToLayer ( string name ) ", true);
			AppendLine("{", true);
			++Indet;

			AppendLine("return LayerMask.NameToLayer( name );", true);

			--Indet;
			AppendLine("}", true);
		}

		protected override void OnMakeFooter(in ConstGeneratorSettings settings)
		{
		}
	}
}
