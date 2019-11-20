using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gate : MonoBehaviour, IDropHandler
{
    public Complex[] input1, input2;
    public Complex[] output1, output2;

    public enum Type { PauliX, Hadamard, CNOT };
    public Type top, bottom;

    public void OnDrop(PointerEventData ped)
    {

    }
}
