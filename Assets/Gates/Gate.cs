using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gate : MonoBehaviour, IDropHandler
{
    public Complex[] input { get; set; }
    public Complex[] output { get; private set; }
    public Complex[,] matrix { get; private set; }

    public enum Type { Identity, PauliX, Hadamard, CNOT };
    public Type top=Type.Identity, bottom=Type.Identity;

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

        if (top == Type.CNOT || bottom == Type.CNOT)
        {
            matrix = new Complex[,] {
                { 1,0,0,0 },
                { 0,1,0,0 },
                { 0,0,0,1 },
                { 0,0,1,0 }
            };
        }
        else
        {
            matrix = TensorProduct(matrices[top], matrices[bottom]);
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

    // [SerializeField] GameObject topSquare, botSquare;
    [SerializeField] TMPro.TextMeshProUGUI topText, botText;
    public void OnDrop(PointerEventData ped)
    {
        var pickup = ped.pointerDrag.GetComponent<Pickup>();
        if (pickup != null)
        {
            if (pickup.type == Type.CNOT)
            {
                top = bottom = Type.CNOT;
                topText.text = pickup.symbol[0].ToString();
                botText.text = pickup.symbol[1].ToString(); // TODO: polymorphism instead
            }
            else
            {
                if (transform.InverseTransformPoint(ped.position).y > 0)
                {
                    top = pickup.type;
                    topText.text = pickup.symbol;
                }
                else
                {
                    bottom = pickup.type;
                    botText.text = pickup.symbol;
                }
            }
        }
    }
}
