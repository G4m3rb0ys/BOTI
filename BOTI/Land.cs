using System;
using System.Runtime.InteropServices;

namespace BOTI
{
    public class Land
    {
        // Variables
        public int ID { get; private set; }
        public string Location { get; private set; }
        public Player Owner { get; private set; }
        public string Color { get; private set; }
        public int Soldiers { get; private set; }

        public Land(string color, int soldiers, string location, Player owner)
        {
            Color = color;
            Soldiers = soldiers;
            Location = location;
            Owner = owner;
        }

        public void SetID(int id)
        {
            ID = id;
        }

        // Functions
        public void AssignOwner(Player owner)
        {
            Owner = owner;
            Color = owner.Color;
            Random rand = new Random();
            Soldiers = rand.Next(1, 8);
        }

        public void OwnerAttack(Player player, int soldiers)
        {
            Console.WriteLine($"{player.Name} has taken {Location}");
            Owner = player;
            Color = player.Color;
            Soldiers = soldiers;
        }

        public void SetSoldiers(int soldiers)
        {
            Soldiers = soldiers;
        }
    }
}
