using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace jwellone.ConstGenerator.Editor
{
	public interface IConstGenerator
	{
		void OnSave(in ConstGeneratorSettings settings, ref string outputFilePath);
		bool OnWillSaveAssets(in string[] saveAssetsPaths, in ConstGeneratorSettings settings, ref string outputFilePath);
	}

	public abstract class ConstGenerator : IConstGenerator
	{
		private int m_indent;
		private StringBuilder m_stringBuilder;

		protected int Indet
		{
			get
			{
				return m_indent;
			}
			set
			{
				m_indent = Math.Max(0, value);
			}
		}

		protected abstract string FileName
		{
			get;
		}

		protected string FileExtension
		{
			get { return "cs"; }
		}

		public void OnSave(in ConstGeneratorSettings settings, ref string outputFilePath)
		{
			Write(settings, ref outputFilePath);
		}

		public bool OnWillSaveAssets(in string[] saveAssetsPaths, in ConstGeneratorSettings settings, ref string outputFilePath)
		{
			if (CanOnWillSaveAssetsWrite(saveAssetsPaths))
			{
				Write(settings, ref outputFilePath);
				return true;
			}
			return false;
		}

		private void Write(in ConstGeneratorSettings settings, ref string outputFilePath)
		{
			var fileName = FileName + "." + ConstGeneratorManager.FILE_NAME_SUFFIX;
			var guids = AssetDatabase.FindAssets(string.Format("t:script {0}", fileName));

			fileName += "." + FileExtension;

			var relativePath = string.Empty;
			if (guids.Length > 0)
			{
				relativePath = AssetDatabase.GUIDToAssetPath(guids[0]).Replace("Assets/", string.Empty);
			}
			else
			{
				relativePath = Path.Combine(ConstGeneratorManager.DEFAULT_SCRIPTS_PATH, fileName);
				CreateDirectory(relativePath);
			}

			outputFilePath = relativePath;
			m_indent = 0;
			m_stringBuilder = new StringBuilder();

			AppendLine( "// =================================================");
			AppendLine( "// This is an automatically generated file.");
			AppendLine( "// Unable to edit.");
			AppendLine($"// Create by class {GetType().Name}.");
			AppendLine( "// =================================================");

			OnMakeHeader(settings);

			var useNameSpace = !string.IsNullOrEmpty(settings.NameSpace);
			if (useNameSpace)
			{
				Append("namespace ");
				AppendLine(settings.NameSpace);
				AppendLine("{");
				++Indet;
			}

			Append("public static class ", true);
			AppendLine(FileName);
			AppendLine("{", true);
			++Indet;

			OnMakeBody(settings);

			--Indet;
			Append("}", true);

			if (useNameSpace)
			{
				--Indet;
				AppendLine("");
				Append("}");
			}

			OnMakeFooter(settings);

			AppendLine("");

			var fullPath = Path.Combine(Application.dataPath, relativePath);
			using (var stream = new StreamWriter(fullPath, false, Encoding.UTF8))
			{
				stream.NewLine = "\r\n";
				stream.Write(m_stringBuilder.ToString().Replace(Environment.NewLine, stream.NewLine));
				ConstGeneratorManager.Log("automatically generated " + fullPath);
			}
		}

		protected void Append(string text, bool enableIndent = false)
		{
			if (enableIndent)
			{
				for (var i = 0; i < m_indent; ++i)
				{
					m_stringBuilder.Append("\t");
				}
			}

			m_stringBuilder.Append(text);
		}

		protected void AppendLine(string text, bool enableIndent = false)
		{
			Append(text, enableIndent);
			m_stringBuilder.AppendLine("");
		}

		private void CreateDirectory(string relativePath)
		{
			var path = Path.GetDirectoryName(Path.Combine(Application.dataPath, relativePath));
			if (Directory.Exists(path))
			{
				return;
			}

			Directory.CreateDirectory(path);
		}

		protected abstract bool CanOnWillSaveAssetsWrite(in string[] saveAssetsPaths);
		protected abstract void OnMakeHeader(in ConstGeneratorSettings settings);
		protected abstract void OnMakeBody(in ConstGeneratorSettings settings);
		protected abstract void OnMakeFooter(in ConstGeneratorSettings settings);
	}
}
