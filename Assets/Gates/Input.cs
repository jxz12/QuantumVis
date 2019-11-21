using System;
using UnityEngine;

public class Input : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI digit0, digit1;
    public event Action OnSet;
    public bool state { get; private set; } = false;
    public void SetValue(bool newState)
    {
        print(state+" "+newState);
        if (state && !newState)
        {
            digit0.text = "1";
            digit1.text = "0";
        }
        else if (!state && newState)
        {
            digit0.text = "0";
            digit1.text = "1";
        }
        state = newState;
        OnSet?.Invoke();
    }
}
