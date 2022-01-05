using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    [Serializable]
    public class CompletedObjectives
    {
        public bool CompletedNoHits { get; set; } // 1st
        public bool CompletedPoints { get; set; } //2nd
        public bool CompletedTime { get; set; } //3rd
        public float BestTime { get; set; }
    }
}
