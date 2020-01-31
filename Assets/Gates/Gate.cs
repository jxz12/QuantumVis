using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    // TODO: make this any size input
    public Complex[] input { get; set; } = new Complex[4];
    public Complex[] output { get; private set; } = new Complex[4];
    public Complex[,] matrix { get; private set; }

    public enum Type { Identity, PauliX, Hadamard, C, NOT };
    public Type topType=Type.Identity, botType=Type.Identity;

    static Dictionary<Type, Complex[,]> matrices = new Dictionary<Type, Complex[,]>()
    {
        { Type.Identity, new Complex[,] {
            {1,0},
            {0,1}
        }},
        { Type.PauliX, new Complex[,] {
            {0,1},
            {1,0}
        }},
        { Type.Hadamard, new Complex[,] {
            {1/Math.Sqrt(2),1/Math.Sqrt(2)},
            {1/Math.Sqrt(2),-1/Math.Sqrt(2)}
        }},
    };
    Complex[,] TensorProduct(Complex[,] a, Complex[,] b)
    {
        int na=a.GetLength(0), ma=a.GetLength(1);
        int nb=b.GetLength(0), mb=b.GetLength(1);
        Complex[,] result = new Complex[na*nb, ma*mb];
        for (int ia=0; ia<na; ia++)
        {
            for (int ja=0; ja<ma; ja++)
            {
                for (int ib=0; ib<nb; ib++)
                {
                    for (int jb=0; jb<mb; jb++)
                    {
                        result[ia*nb + ib, ja*nb + jb] = a[ia,ja] * b[ib,jb];
                    }
                }
            }
        }
        return result;
    }
    public void Run()
    {
        if (input==null || input.Length != 4)
            throw new Exception("malformed input");

        if (topType == Type.C && botType == Type.NOT)
        {
            matrix = new Complex[,] {
                { 1,0,0,0 },
                { 0,1,0,0 },
                { 0,0,0,1 },
                { 0,0,1,0 }
            };
        }
        else if (topType == Type.NOT && botType == Type.C)
        {
            matrix = new Complex[,] {
                { 1,0,0,0 },
                { 0,0,0,1 },
                { 0,0,1,0 },
                { 0,1,0,0 }
            };
        }
        else
        {
            matrix = TensorProduct(matrices[topType], matrices[botType]);
        }
        if (matrix.GetLength(0)!=4 || matrix.GetLength(1)!=4)
            throw new Exception("malformed matrix");
        
        output = new Complex[] { 0, 0, 0, 0 };
        for (int i=0; i<4; i++)
        {
            for (int j=0; j<4; j++)
            {
                output[i] += matrix[i,j] * input[j];
            }
        }
    }

    static Dictionary<Type, string> symbols = new Dictionary<Type, string>()
    {
        { Type.Identity, "I" },
        { Type.PauliX, "X" },
        { Type.Hadamard, "H" },
        { Type.C, "C" },
        { Type.NOT, "X" },
    };
    public bool SetType(Type newType, bool topHalf)
    {
        if (newType == Type.C)
        {
            topType = Type.C;
            botType = Type.NOT;
            // topText.text = symbols[Type.C];
            // botText.text = symbols[Type.NOT];
            bridge.enabled = true;
        }
        else if (newType == Type.NOT)
        {
            topType = Type.NOT;
            botType = Type.C;
            // topText.text = symbols[Type.NOT];
            // botText.text = symbols[Type.C];
            bridge.enabled = true;
        }
        else
        {
            if (topHalf)
            {
                topType = newType;
                if (botType == Type.C || botType == Type.NOT) {
                    // topText.text = symbols[Type.Identity];
                    botType = Type.Identity;
                }
            }
            else
            {
                botType = newType;
                // botText.text = symbols[newType];
                if (topType == Type.C || topType == Type.NOT) {
                    // topText.text = symbols[Type.Identity];
                    topType = Type.Identity;
                }
            }
            bridge.enabled = false;
        }

        // refresh appearance
        if (botType == Type.Identity) {
            botText.text = "";
            botText.transform.parent.GetComponent<Image>().enabled = false;
        } else {
            botText.text = symbols[botType];
            botText.transform.parent.GetComponent<Image>().enabled = true;
        }
        if (topType == Type.Identity) {
            topText.text = "";
            topText.transform.parent.GetComponent<Image>().enabled = false;
        } else {
            topText.text = symbols[topType];
            topText.transform.parent.GetComponent<Image>().enabled = true;
        }
        return topType==Type.Identity && botType==Type.Identity;
    }
    [SerializeField] TMPro.TextMeshProUGUI topText, botText;
    [SerializeField] UnityEngine.UI.Image bridge;
    [SerializeField] UnityEngine.UI.Image[] inputState;
    [SerializeField] UnityEngine.UI.Image[] outputState;
    public void ColourInState()
    {
        if (inputState.Length != input.Length || outputState.Length != output.Length) {
            throw new Exception("wrong number of squares");
        }

        for (int i=0; i<4; i++)
        {
            // TODO: complex numbers will look different
            float inProb = (float)input[i].Real;
            if (inProb > 0) {
                inputState[i].color = new Color(0, inProb, 0);
            } else if (inProb < 0) {
                inputState[i].color = new Color(-inProb, 0, 0);
            } else {
                inputState[i].color = Color.black;
            }
            float outProb = (float)output[i].Real;
            if (outProb > 0) {
                outputState[i].color = new Color(0, outProb, 0);
            } else if (outProb < 0) {
                outputState[i].color = new Color(-outProb, 0, 0);
            } else {
                outputState[i].color = Color.black;
            }
        }
    }
}
