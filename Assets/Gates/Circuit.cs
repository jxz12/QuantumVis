using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Circuit : MonoBehaviour
{
    List<Gate> gates;

    public bool qubit1, qubit2;
    public void Set1(bool val) { qubit1 = val; }
    public void Set2(bool val) { qubit2 = val; }

    void Start()
    {
        gates = new List<Gate>(transform.GetComponentsInChildren<Gate>());
    }
    public void Run()
    {
        Run((qubit1?0:2) + (qubit2?0:1));
    }
    void Run(int input)
    {
        // initialise input
        if (input<0 || input>3)
            throw new Exception("bad input");

        gates[0].input = new Complex[] { 0,0,0,0 };
        gates[0].input[input] = 1;
        
        // run to get outputs
        gates[0].Run();
        for (int i=1; i<gates.Count; i++)
        {
            gates[i].input = gates[i-1].output; // potential value reference problems?
            gates[i].Run();
        }

        // print results
        foreach (Complex c in gates[gates.Count-1].output)
        {
            print(c);
        }
    }
}
