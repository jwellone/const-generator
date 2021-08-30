using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace jwellone.ConstGenerator.Editor
{
	public class ConstGeneratorSettings : ScriptableObject
	{
		[Serializable]
		public class GeneratorInfo
		{
			public bool valid;
			public bool autoWrite;
			public string generetorClassName;
			public string displayName;

			public GeneratorInfo(Type type)
			{
				valid = true;
				autoWrite = true;
				this.displayName = type.Name;
				this.generetorClassName = type.FullName;
			}
		}

		private const string PATH = "ProjectSettings/ConstGeneratorSettings.json";

		private static ConstGeneratorSettings s_instance;

		[SerializeField] private string m_namespace = "Const";
		[SerializeField] private bool m_logEnabled = true;
		[SerializeField] private List<GeneratorInfo> m_infos = new List<GeneratorInfo>(0);

		private bool m_withUpdate;

		public string NameSpace => m_namespace;
		public bool LogEnabled => m_logEnabled;
		public IList<GeneratorInfo> Infos => m_infos;

		public static ConstGeneratorSettings Instance
		{
			get
			{
				if (s_instance != null)
				{
					return s_instance;
				}

				try
				{
					var json = File.ReadAllText(PATH);
					var instance = CreateInstance<ConstGeneratorSettings>();
					JsonUtility.FromJsonOverwrite(json, instance);
					s_instance = instance != null ? instance : CreateInstance<ConstGeneratorSettings>();
				}
				catch
				{
					if (File.Exists(PATH))
					{
						File.Delete(PATH);
					}

					s_instance = CreateInstance<ConstGeneratorSettings>();
					Save();
				}

				return s_instance;
			}
		}

		public static bool Exists()
		{
			return File.Exists(Application.dataPath + "/../" + PATH);
		}

		public static void Create(IList<Type> types)
		{
			var infos = Instance.m_infos;
			foreach (var info in infos)
			{
				info.valid = false;
			}

			foreach (var t in types)
			{
				var target = t.ToString();
				var info = infos.Find(x => x.generetorClassName == target);
				if (info != null)
				{
					info.valid = true;
					continue;
				}

				infos.Add(new GeneratorInfo(t));
			}
		}

		private static void Save()
		{
			if (s_instance != null)
			{
				var json = JsonUtility.ToJson(s_instance);
				File.WriteAllText(PATH, json);
				s_instance.m_withUpdate = false;
			}
		}

		[CustomEditor(typeof(ConstGeneratorSettings))]
		private class CustomInspector : UnityEditor.Editor
		{
			private const string FOLDOUT_PREFES_KEY = "CONST_GENERARTOR_FOLDOUT_KEY";
			private bool m_isFoldout;
			private bool IsFoldout
			{
				get => m_isFoldout;
				set
				{
					if (m_isFoldout != value)
					{
						m_isFoldout = value;
						EditorPrefs.SetBool(FOLDOUT_PREFES_KEY, value);
					}
				}
			}

			private void OnEnable()
			{
				m_isFoldout = EditorPrefs.GetBool(FOLDOUT_PREFES_KEY);
			}

			public override void OnInspectorGUI()
			{
				var instance = (ConstGeneratorSettings)target;
				EditorGUI.BeginChangeCheck();

				instance.m_logEnabled = EditorGUILayout.Toggle("Log enabled", instance.m_logEnabled);
				instance.m_namespace = EditorGUILayout.TextField("namespace", instance.m_namespace);

				var isSave = false;
				var isWrite = false;
				IsFoldout = EditorGUILayout.Foldout(IsFoldout, "Generator enabled setting");
				if (IsFoldout)
				{
					var removeIndex = -1;
					++EditorGUI.indentLevel;
					for (var i =0;i < instance.Infos.Count; ++i)
					{
						var info = instance.Infos[i];
						GUI.color = Color.white;
						if (info.valid)
						{
							info.autoWrite = EditorGUILayout.Toggle(info.displayName, info.autoWrite);
						}
						else
						{
							EditorGUILayout.BeginHorizontal();
							GUI.color = Color.red;
							EditorGUILayout.LabelField(info.displayName);
							GUI.color = Color.white;
							EditorGUILayout.Space();
							if (GUILayout.Button("Remove", GUILayout.Width(64)))
							{
								removeIndex = i;
							}
							EditorGUILayout.EndHorizontal();
						}
					}
					GUI.color = Color.white;
					--EditorGUI.indentLevel;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space();
					if (GUILayout.Button("Write", GUILayout.Width(64)))
					{
						isSave = true;
						isWrite = true;
					}
					EditorGUILayout.EndHorizontal();

					if (removeIndex != -1)
					{
						instance.Infos.RemoveAt(removeIndex);
					}
				}

				if (EditorGUI.EndChangeCheck())
				{
					instance.m_withUpdate = true;
					serializedObject.ApplyModifiedProperties();
				}

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if (GUILayout.Button("Open", GUILayout.Width(64)))
				{
					EditorUtility.RevealInFinder(PATH);
				}

				EditorGUI.BeginDisabledGroup(!instance.m_withUpdate);
				if (GUILayout.Button("Apply", GUILayout.Width(64)))
				{
					isSave = true;
				}
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();

				if (isSave)
				{
					Save();
				}

				if (isWrite)
				{
					ConstGeneratorManager.Write();
				}
			}
		}
	}
}
