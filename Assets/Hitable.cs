using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//:D
public class Hitable : MonoBehaviour {

    public UniversalHealth hp;
    public Renderer damageMesh;

    // Use this for initialization
    public void Hit () {
		
	}

    public Color GetColor()
    {
        Color c = damageMesh.material.GetColor("_Color");
        return c;
    }
}
