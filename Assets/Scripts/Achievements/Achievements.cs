using System;

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
        public string img;

        public Achievements(int id, int type, int collectableItem, int count, string title, string desc, string img)
        {
            this.id = id;
            this.type = type;
            this.collectableItem = collectableItem;
            this.count = count;
            this.title = title;
            this.desc = desc;
            this.img = img;
        }
    }
}
