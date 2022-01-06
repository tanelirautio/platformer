using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    [Serializable]
    public class Achievements 
    {
        public int id;
        public int type;
        public int collectableItem;
        public int count;
        public string title;
        public string desc;

        public Achievements(int id, int type, int collectableItem, int count, string title, string desc)
        {
            this.id = id;
            this.type = type;
            this.collectableItem = collectableItem;
            this.count = count;
            this.title = title;
            this.desc = desc;
        }
    }
}
