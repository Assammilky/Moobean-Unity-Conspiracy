using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identifier : MonoBehaviour
{
    public bool inputNode;

    public enum NodeType
    {
        grid,
        marking,
        hexagon
    }

    public NodeType nodeType;
    public bool submitDial;
}
