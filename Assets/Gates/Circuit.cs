using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Circuit : MonoBehaviour
{
    List<Gate> gates;

    [SerializeField] Toggle qubit1, qubit2;
    [SerializeField] TMPro.TextMeshProUGUI result;

    void Start()
    {
        qubit1.onValueChanged.AddListener(b=>Run());
        qubit2.onValueChanged.AddListener(b=>Run());

        gates = new List<Gate>(transform.GetComponentsInChildren<Gate>());
        foreach (var gate in gates)
        {
            gate.OnDropped += Run;
        }

        Run();
    }
    public void Run()
    {
        // initialise first gate
        int input = (qubit1.isOn?2:0) + (qubit2.isOn?1:0);

        gates[0].input = new Complex[] { 0,0,0,0 };
        gates[0].input[input] = 1;

        // run to get outputs
        gates[0].Run();
        gates[0].ColourInState();
        for (int i=1; i<gates.Count; i++)
        {
            gates[i].input = gates[i-1].output; // potential value reference problems?
            gates[i].Run();
            gates[i].ColourInState();
        }

        // print results
        var sb = new System.Text.StringBuilder();
        foreach (Complex c in gates[gates.Count-1].output)
        {
            sb.Append((c.Magnitude * c.Magnitude).ToString("0.00")).Append('\n');
        }
        sb.Length -= 1;
        result.text = sb.ToString();
    }
}
