using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTI
{
    public class Player
    {
        // Variables
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Color { get; private set; }

        // Functions
        public Player(string name, string color)
        {
            Name = name;
            Color = color;
        }

        public void SetID(int id)
        {
            ID = id;
        }
    }
}

