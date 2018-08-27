using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class CustomizationHandler : MonoBehaviour {
	[Tooltip("Defines whether to use the vehicle customization or the character customization")]
	public bool isVehicle = true;
	[Header("Vehicle Customization")]
	public ParticleSystem crashParticles;
	public Color vehicleColor;
	public MeshRenderer[] allMeshes;
	public bool useRandomColor = true;
	[Space()]
	[Header("Character Customization")]
	public Texture2D charTex;
	public SkinnedMeshRenderer[] allCharMeshes;
	public Color[] charColors;

	// Use this for initialization
	void Start () {
		if (useRandomColor) {
			vehicleColor = Random.ColorHSV ();
			if (!isVehicle) {
				charColors [3] = vehicleColor;
				charColors [4] = vehicleColor;
			}
		}
		Init ();
	}

	public void Init()
	{
		if (isVehicle)
		{
			var main = crashParticles.main;
			main.startColor = vehicleColor;
			foreach (MeshRenderer m in allMeshes) {
				m.material.SetColor ("_Color", vehicleColor);
				m.material.SetColor ("_Color2", vehicleColor);
			}
		} 
		else
		{
			charTex = new Texture2D (3, 3, TextureFormat.ARGB32, false);
			charTex.filterMode = FilterMode.Point;
			GenerateImage (charColors);
		}
	}
	public void GenerateImage (Color[] colors)
	{
		charTex.SetPixels(colors);

		charTex.Apply ();
		foreach (SkinnedMeshRenderer m in allCharMeshes) {
			m.material.mainTexture = charTex;
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
