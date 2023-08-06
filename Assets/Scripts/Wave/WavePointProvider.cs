using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(VoidWaveMesh))]
public abstract class WavePointProvider : MonoBehaviour
{
    public abstract bool HasPoint(float x, float width);
    public abstract float GetY(float x, float width);
    public int DrawCount { get; set; } = 0;
}

