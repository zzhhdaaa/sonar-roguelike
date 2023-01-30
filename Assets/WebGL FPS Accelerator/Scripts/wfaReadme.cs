using System;
using UnityEngine;

public class wfaReadme : ScriptableObject
{
	public Texture2D icon;
	public string title;
	public Section[] sections;
	public bool loadedLayout;

	[Serializable]
	public class Section
	{
		public string heading, text, linkText, url;
	}
}
