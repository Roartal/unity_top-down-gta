using UnityEngine;
using System.Collections.Generic;

public class GlowObjectCmd : MonoBehaviour
{
	public Color GlowColor;

	public Renderer Renderers
	{
		get;
		private set;
	}

	public Color CurrentColor
	{
		get { return GlowColor; }
	}

	void Start()
	{
		Renderers = GetComponent<Renderer>();
		GlowController.RegisterObject(this);
	}
}