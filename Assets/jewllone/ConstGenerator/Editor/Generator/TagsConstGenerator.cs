using System.Linq;
using UnityEditorInternal;

namespace jwellone.ConstGenerator.Editor
{
	public class TagsConstGenerator : ConstGenerator
	{
		protected override string FileName
		{
			get
			{
				return "Tags";
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
			var tags = InternalEditorUtility.tags.ToList();
			foreach (var tag in tags)
			{
				Append("public static readonly string ", true);
				Append(tag.Replace(" ", ""));
				Append(" = \"");
				Append(tag);
				AppendLine("\";");
			}
		}

		protected override void OnMakeFooter(in ConstGeneratorSettings settings)
		{
		}
	}
}
