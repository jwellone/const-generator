// =================================================
// This is an automatically generated file.
// Unable to edit.
// Create by class LayersConstGenerator.
// =================================================
using UnityEngine;

namespace Const
{
	public static class Layers
	{
		public const int Default = 0;
		public const int TransparentFX = 1;
		public const int IgnoreRaycast = 2;
		public const int Water = 4;
		public const int UI = 5;

		public const int MaskDefault = 1;
		public const int MaskTransparentFX = 2;
		public const int MaskIgnoreRaycast = 4;
		public const int MaskWater = 16;
		public const int MaskUI = 32;

		public static readonly string LabelDefault = "Default";
		public static readonly string LabelTransparentFX = "TransparentFX";
		public static readonly string LabelIgnoreRaycast = "Ignore Raycast";
		public static readonly string LabelWater = "Water";
		public static readonly string LabelUI = "UI";

		public static int NameToLayer ( string name ) 
		{
			return LayerMask.NameToLayer( name );
		}
	}
}
