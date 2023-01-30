using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

[CustomEditor(typeof(wfaReadme))]
[InitializeOnLoad]
public class wfaReadmeEditor : Editor
{

	static float kSpace = 16f;

	static wfaReadmeEditor()
	{
	}

	[MenuItem("WFA Help/Show")]
	static wfaReadme SelectwfaReadme()
	{
		wfaReadme wfaReadmeObject = (wfaReadme)AssetDatabase.LoadAssetAtPath("Assets/WebGL FPS Accelerator/README_wfa.asset", typeof(wfaReadme));
		Selection.objects = new UnityEngine.Object[] { wfaReadmeObject };
		return wfaReadmeObject;
	}

	protected override void OnHeaderGUI()
	{
		var wfaReadme = (wfaReadme)target;
		Init();

		var iconWidth = Mathf.Min((EditorGUIUtility.currentViewWidth / 100) * 50, 300f);

		GUILayout.BeginHorizontal("In BigTitle");
		{

			GUILayout.FlexibleSpace();
			GUILayout.Label(wfaReadme.icon, GUILayout.Width(iconWidth), GUILayout.Height((iconWidth / 1950) * 1300));
			//GUILayout.Label(wfaReadme.title, TitleStyle);
			GUILayout.FlexibleSpace();


		}
		GUILayout.EndHorizontal();
	}

	public override void OnInspectorGUI()
	{
		var wfaReadme = (wfaReadme)target;
		Init();

		foreach (var section in wfaReadme.sections)
		{
			if (!string.IsNullOrEmpty(section.heading))
			{
				GUILayout.Label(section.heading, HeadingStyle);
			}
			if (!string.IsNullOrEmpty(section.text))
			{
				GUILayout.Label(section.text, BodyStyle);
			}
			if (!string.IsNullOrEmpty(section.linkText))
			{
				if (LinkLabel(new GUIContent(section.linkText)))
				{
					Application.OpenURL(section.url);
				}
			}
			GUILayout.Space(kSpace);
		}
	}


	bool m_Initialized;

	GUIStyle LinkStyle { get { return m_LinkStyle; } }
	[SerializeField] GUIStyle m_LinkStyle;

	GUIStyle TitleStyle { get { return m_TitleStyle; } }
	[SerializeField] GUIStyle m_TitleStyle;

	GUIStyle HeadingStyle { get { return m_HeadingStyle; } }
	[SerializeField] GUIStyle m_HeadingStyle;

	GUIStyle BodyStyle { get { return m_BodyStyle; } }
	[SerializeField] GUIStyle m_BodyStyle;

	void Init()
	{
		if (m_Initialized)
			return;
		m_BodyStyle = new GUIStyle(EditorStyles.label);
		m_BodyStyle.wordWrap = true;
		m_BodyStyle.fontSize = 14;

		m_TitleStyle = new GUIStyle(m_BodyStyle);
		m_TitleStyle.fontSize = 26;

		m_HeadingStyle = new GUIStyle(m_BodyStyle);
		m_HeadingStyle.fontSize = 18;

		m_LinkStyle = new GUIStyle(m_BodyStyle);
		m_LinkStyle.wordWrap = false;
		// Match selection color which works nicely for both light and dark skins
		m_LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
		m_LinkStyle.stretchWidth = false;

		m_Initialized = true;
	}

	bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
	{
		var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

		Handles.BeginGUI();
		Handles.color = LinkStyle.normal.textColor;
		Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
		Handles.color = Color.white;
		Handles.EndGUI();

		EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

		return GUI.Button(position, label, LinkStyle);
	}
}

