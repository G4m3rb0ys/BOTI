using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTI
{
    internal class Player
    {

        // variables
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Color { get; private set; }

        // functions
        public Player(string name, string color)
        {
            this.Name = name;
            this.Color = color;
        }

    }
}
