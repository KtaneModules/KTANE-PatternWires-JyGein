using System;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;
using PatternWires;

public class Wires : MonoBehaviour {

    public MeshRenderer[] Wirecasings;
    public GameObject CutWire;
    public GameObject UncutWire;
    public patternWires module;
    public KMSelectable SelectableWire;
    public TextMesh ColorBlindText;

    public WireColors Color {
        get { return _color; }
        set {
            foreach(MeshRenderer i in Wirecasings) {
                i.material.mainTexture = module.ColorTextures[(int)value];
            }
            _color = value;
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

    public bool Valid { get; set; }

    private WireColors _color;
    private bool _cut;

	// Use this for initialization
	public void Awake () {
        Valid = false;
        Color = (WireColors)Rnd.Range(0, Enum.GetValues(typeof(WireColors)).Length);
        SelectableWire.OnInteract = delegate () { wirecut(true); return false; };
        foreach(char i in module.ColorTextures[(int)Color].name) {
            if("ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(i)) {
                ColorBlindText.text = i.ToString();
            }
        }
    }

    void wirecut(bool cut) {
        if(module.ModuleSolved) { return; }
        Cut = cut;
        module.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSnip, SelectableWire.transform);
        SelectableWire.AddInteractionPunch();
        if(!Valid) {
            module.Strike();
        }
        foreach(Wires Wire in module.Wires) {
            if(Wire.Valid && !Wire.Cut) { return; }
        }
        module.Solve();
    }
}