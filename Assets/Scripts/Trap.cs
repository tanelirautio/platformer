using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    public enum Type
    {
        Spike,
        Saw,
        KillZone
    }

    public Type type;
}
