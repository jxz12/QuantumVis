using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Circuit : MonoBehaviour
{
    [SerializeField] List<Input> inputs;
    [SerializeField] List<Pickup> pickups;
    [SerializeField] TMPro.TextMeshProUGUI result;
    public bool qubit0 { get; set; }
    public bool qubit1 { get; set; }

    List<Gate> gates;
    [SerializeField] HorizontalLayoutGroup gatesLayout;
    void Start()
    {
        foreach (var input in inputs) {
            input.OnSet += Run;
        }
        foreach (var pickup in pickups) {
            pickup.OnDragged += PickupDragCallback;
            pickup.OnDropped += PickupDropCallback;
        }
        gates = new List<Gate>();
        Run();
    }

    int snappedSectionX, snappedSectionY;
    float xRange;
    void PickupDragCallback(PointerEventData ped)
    {
        var gatesRT = gatesLayout.GetComponent<RectTransform>();
        var corners = new Vector3[4];
        gatesRT.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float yMin = corners[0].y;
        float xMax = corners[2].x;
        float yMax = corners[2].y;
        float xViewport = (ped.position.x-xMin) / (xMax-xMin);
        float yViewport = (ped.position.y-yMin) / (yMax-yMin);

        var pickup = ped.pointerDrag.GetComponent<Pickup>();

        // check if hovered
        if (xViewport<0 || xViewport>1 || yViewport<0 || yViewport>1) {
            snappedSectionX = snappedSectionY = -1;
            pickup.transform.position = ped.position;
            pickup.Alpha = .7f;
            return;
        }
        // snap
        int nSlotsX = 2*gates.Count + 3;
        snappedSectionX = (int)Mathf.Round(xViewport * (nSlotsX-1));
        // sections at end can be snapped to one-from-end
        snappedSectionX = Math.Min(nSlotsX-2, Math.Max(1, snappedSectionX));

        // hard coded for two qubits only
        int nSlotsY = (pickup.type==Gate.Type.C || pickup.type==Gate.Type.NOT)? 1 : 2;
        snappedSectionY = (int)Mathf.Round(yViewport * (nSlotsY-1));

        float snappedPosX = nSlotsX<=1? .5f : (float)snappedSectionX / (nSlotsX-1);
        float snappedPosY = nSlotsY==1? .5f : .25f + .5f*snappedSectionY / (nSlotsY-1);
        snappedPosX = xMin + snappedPosX*(xMax-xMin);
        snappedPosY = yMin + snappedPosY*(yMax-yMin);

        pickup.transform.position = new Vector2(snappedPosX, snappedPosY);
        pickup.Alpha = .9f;
        xRange = xMax-xMin;
    }
    [SerializeField] Gate gatePrefab;
    void PickupDropCallback(PointerEventData ped)
    {
        var pickup = ped.pointerDrag.GetComponent<Pickup>();
        pickup.Alpha = 1;
        // check if hovered
        if (snappedSectionX == -1) {
            return;
        }
        bool topHalf = snappedSectionY==1;
        if (snappedSectionX%2 == 1) // odd numbers mean new gate
        {
            if (pickup.type == Gate.Type.Identity) {
                return;
            }
            var newGate = Instantiate(gatePrefab, gatesLayout.transform);
            int idx = (snappedSectionX-1) / 2;
            gates.Insert(idx, newGate);
            newGate.SetType(pickup.type, topHalf);
            if (pickup.type!=Gate.Type.C && pickup.type!=Gate.Type.NOT) {
                newGate.SetType(Gate.Type.Identity, !topHalf); // initialise other half
            }
        }   
        else // hovered over existing
        {
            var hoveredGate = gates[snappedSectionX/2 - 1];
            bool isIdentity = hoveredGate.SetType(pickup.type, topHalf);
            if (isIdentity)
            {
                gates.Remove(hoveredGate);
                Destroy(hoveredGate.gameObject);
            }
        }
        foreach (Gate gate in gates)
        {
            gate.transform.SetAsLastSibling();
        }
        float n = (float)gates.Count;
        float layoutBorder = n<=1? 0 : (1/(n+1) - 1/(2*n)) / (1 - 1/n);
        gatesLayout.padding.left = gatesLayout.padding.right = (int)(layoutBorder * xRange);
        Run();
    }
    public void Run()
    {
        // initialise first gate
        int input = (inputs[0].state?2:0) + (inputs[1].state?1:0);

        var inState = new System.Numerics.Complex[] { 0,0,0,0 };
        inState[input] = 1;
        System.Numerics.Complex[] outState = null;
        if (gates.Count > 0)
        {
            gates[0].input = new System.Numerics.Complex[] { 0,0,0,0 };
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
            outState = gates[gates.Count-1].output;
        }
        else
        {
            outState = inState;
        }

        // print results
        var sb = new System.Text.StringBuilder();
        foreach (System.Numerics.Complex c in outState)
        {
            sb.Append((c.Magnitude * c.Magnitude).ToString("0.00")).Append('\n');
        }
        sb.Length -= 1;
        result.text = sb.ToString();
    }
}
