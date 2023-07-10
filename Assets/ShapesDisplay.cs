using System;
using UnityEngine;
using Rnd = UnityEngine.Random;
using PatternWires;

public class ShapesDisplay : MonoBehaviour {

    public patternWires module;

    public Shapes Shape {
        get { return _shape; }
        set {
            GetComponent<MeshRenderer>().material.mainTexture = module.ShapeImages[(int)value];
            _shape = value;
        }
    }

    private Shapes _shape;

    // Use this for initialization
    public void Awake() {
        Shape = (Shapes)Rnd.Range(0, Enum.GetValues(typeof(Shapes)).Length);
    }
}
