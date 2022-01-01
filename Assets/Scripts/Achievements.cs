using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    [Serializable]
    public class Achievements 
    {
        //public string meta;
        public int type;
        public int code;
        public string item;
        public int count;
        public string title;
        public string desc;

        public bool Completed { get; set; } = false;

        public Achievements(int type, int code, string item, int count, string title, string desc)
        {
            this.type = type;
            this.code = code;
            this.item = item;
            this.count = count;
            this.title = title;
            this.desc = desc;
        }
    }
}
