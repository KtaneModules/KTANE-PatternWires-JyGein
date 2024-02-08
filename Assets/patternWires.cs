using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using PatternWires;

public class patternWires : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;
    public Texture[] ColorTextures;
    public Texture[] ShapeImages;
    public Wires[] Wires;
    public ShapesDisplay[] ShapeDisplays;
    public KMSelectable ColorblindStatusLight;

    string ModuleName;
    static int ModuleIdCounter = 1;
    int ModuleId;
    [HideInInspector] public bool ModuleSolved;
    private Shapes[,] Patterns = new Shapes[,] {
        { Shapes.Square, Shapes.Star, Shapes.Triangle, Shapes.Circle, Shapes.Triangle },
        { Shapes.Triangle, Shapes.Circle, Shapes.Square, Shapes.Star, Shapes.Triangle },
        { Shapes.Star, Shapes.Circle, Shapes.Triangle, Shapes.Triangle, Shapes.Square },
        { Shapes.Square, Shapes.Triangle, Shapes.Circle, Shapes.Triangle, Shapes.Star },
        { Shapes.Star, Shapes.Square, Shapes.Circle, Shapes.Circle, Shapes.Square },
        { Shapes.Circle, Shapes.Square, Shapes.Triangle, Shapes.Circle, Shapes.Star },
        { Shapes.Triangle, Shapes.Triangle, Shapes.Square, Shapes.Star, Shapes.Star },
    };
    private bool Reversed;

    void Awake () { //Shit that happens before Start
        ModuleName = Module.ModuleDisplayName;
        ModuleId = ModuleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Activate;
        ColorblindStatusLight.OnInteract += delegate () { StatusLightPress(); return false; };
        /*
        foreach (KMSelectable object in keypad) {
            object.OnInteract += delegate () { keypadPress(object); return false; };
        }
        */

        //button.OnInteract += delegate () { buttonPress(); return false; };

        //keypadPress() and buttonPress() you have to make yourself and should just be what happens when you press a button. (deaf goes through it probably)
    }

    void Activate () { //Shit that should happen when the bomb arrives (factory)/Lights turn on

    }

    void Start () { //Shit
        int count = 0;
        int count2 = 0;
        List<WireColors> temp = new List<WireColors>();
        List<WireColors> temp2 = new List<WireColors>();
        if(int.Parse(Bomb.GetSerialNumber().Last().ToString())%2 == 1) {
            Reversed = true;
        }
        count = 0;
        count2 = 0;
        foreach(Wires Wire in Wires) {
            if(Wire.Color == WireColors.Blue) { count++; }
            if(Wire.Color == WireColors.Yellow) { count2++; }
        }
        if(count == 2 && count2 == 1) {
            ValidateWires(0);
            return;
        }

        count = 0;
        temp = new List<WireColors>();
        foreach(Wires Wire in Wires) {
            if(!temp.Contains(Wire.Color)) {
                count++;
            }
            temp.Add(Wire.Color);
        }
        if(count == 5) {
            ValidateWires(1);
            return;
        }

        count = 0;
        foreach(Wires Wire in Wires) {
            if(Wire.Color == WireColors.Red) { count++; }
        }
        if(count == 3) {
            ValidateWires(2);
            return;
        }

        for(int i = 0; i < 5; i++) {
            if(Wires[i].Color == WireColors.Yellow && Wires[(i+1)%5].Color == WireColors.White) {
                ValidateWires(3);
                return;
            }
        }

        temp = new List<WireColors>();
        temp2 = new List<WireColors>();
        foreach(Wires Wire in Wires) {
            if(!temp.Contains(Wire.Color)) {
                temp.Add(Wire.Color);
            } else {
                if(!temp2.Contains(Wire.Color)) {
                    temp2.Add(Wire.Color);
                }
            }
        }
        if(temp2.Count == 2) {
            ValidateWires(4);
            return;
        }

        count = 0;
        count2 = 0;
        foreach(Wires Wire in Wires) {
            if(Wire.Color == WireColors.Green) { count++; }
            if(Wire.Color == WireColors.Black) { count2++; }
        }
        if(count < 2 && count2 == 1) {
            ValidateWires(5);
            return;
        }

        ValidateWires(6);
        return;
    }

    void ValidateWires(int _number) {
        for(int i = 0; i < 5; i++) {
            if(ShapeDisplays[i].Shape == Patterns[_number, Math.Abs((6 * Convert.ToInt32(Reversed)) - (i + 1)) - 1]) {
                Wires[i].Valid = true;
            }
        }
        foreach(Wires Wire in Wires) {
            if(Wire.Valid) {
                Log($"Last digit of the Serial Number is {(Reversed ? "odd so the pattern is reversed." : "even so the pattern is not reversed.")}");
                List<string> correctPattern = new List<string>();
                for(int i = 0; i<5; i++) { correctPattern.Add(Patterns[_number, i].ToString()); }
                if(Reversed) { correctPattern.Reverse(); }
                Log($"Rule number {_number + 1} is correct with the pattern {string.Join(", ", correctPattern.ToArray())}");
                for(int i = 0; i < 5; i++) {
                    Log($"Wire {i + 1} is colored {Wires[i].Color.ToString()}, is with the shape {ShapeDisplays[i].Shape.ToString()}, and is {(Wires[i].Valid ? "Valid" : "Invalid")}");
                }
                return;
            }
        }
        for(int i = 0; i < 5; i++) {
            Wires[i].Awake();
            ShapeDisplays[i].Awake();
        }
        Start();
    }

    void StatusLightPress() {
        foreach(Wires Wire in Wires) {
            Wire.ToggleColorblind();
        }
    }

    void Update () { //Shit that happens at any point after initialization

    }

    public void Solve () { //Call this method when you want the module to solve
        Module.HandlePass();
        Log("Correct! Module Solved.");
        ModuleSolved = true;
    }

    public void Strike () { //Call this method when you want the module to strike
        Module.HandleStrike();
        Log("Incorrect! Strike Issued.");
    }

    public void Log (string message) {
        Debug.Log($"[{ModuleName} #{ModuleId}] {message}");
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} 12345 to cut the wires in their respective positions. !{0} Colorblind to toggle colorblind mode.";
#pragma warning restore 414

    // Twitch Plays (TP) documentation: https://github.com/samfundev/KtaneTwitchPlays/wiki/External-Mod-Module-Support

    KMSelectable[] ProcessTwitchCommand (string Command) {
        if(Command.ToLower() == "colorblind" || Command.ToLower() == "coworbwind") {
            return new KMSelectable[] { ColorblindStatusLight };
        }
        List<KMSelectable> output = new List<KMSelectable>();
        foreach(char i in Command) {
            int f = 0;
            if(int.TryParse(i.ToString(), out f)) {
                if(!Wires[f-1].Cut && !ModuleSolved) { output.Add(Wires[f-1].SelectableWire); }
            } else {
                return null;
            }
        }
        return output.ToArray();
    }

    void TwitchHandleForcedSolve () {
        foreach(Wires Wire in Wires) {
            if(Wire.Valid && !ModuleSolved && !Wire.Cut) {
                Wire.SelectableWire.OnInteract();
            }
        }
    }
}
