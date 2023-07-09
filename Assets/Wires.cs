using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wires : MonoBehaviour {

    public MeshRenderer[] Wirecasing;
    public GameObject CutWire;
    public GameObject UncutWire;

    public Texture Color {
        get { return _texture; }
        set {
            foreach(MeshRenderer i in Wirecasing) {
                i.material.mainTexture = value;
            }
            _texture = value;
        }
    }
    public bool Cut {
        get { return _cut; }
        set {
            CutWire.SetActive(value);
            UncutWire.SetActive(!value);
            _cut = value;
        }
    }

    private Texture _texture;
    private bool _cut;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
