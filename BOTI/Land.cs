using System;
using System.Runtime.InteropServices;

namespace BOTI
{
    internal class Land
    {
        // Variables

        public int ID { get; private set; }
        public string Location { get; private set; }
        public Player Owner { get; private set; }
        public string Color { get; private set; }
        public int Soldiers { get; private set; }

        public Land(string color, int soldiers, string location, Player? owner)
        {
            Owner = owner;
            Color = color;
            Soldiers = soldiers;
            Location = location;
        }

        //functions
        public void assignOwner(Player owner)
        {
            this.Owner = owner;
            this.Color = owner.Color;
            Random rand = new Random();
            this.Soldiers = rand.Next(1, 8);
        }

        public void OwnerAttack (Player player, int soldiers)
        {
            Console.WriteLine($"{player.Name} has taken {this.Location}");
            this.Owner = player;
            this.Color = player.Color;
            this.Soldiers = soldiers;
        }

        public void SetSoldiers(int soldiers)
        {
            this.Soldiers = soldiers;
        }
    }
}