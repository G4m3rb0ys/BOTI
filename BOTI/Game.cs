using System;
using System.Collections.Generic;
using System.Linq;

namespace BOTI
{
    public class Game
    {
        // Variables
        // Constants
        private const int GridSize = 10;
        private const string DefaultLandColor = "Grey";
        public bool GameOver { get; set; }

        // Properties
        public int ID { get; private set; }
        public List<Player> Players { get; set; }
        public List<Land> Lands { get; set; }
        public Player CurrentPlayer { get; private set; }
        public int Round { get; private set; }

        // Constructor
        public Game(List<Player> players)
        {
            Players = players;
            Lands = new List<Land>();
            Round = 0;
            CurrentPlayer = players[0];

            // Create lands
            List<Land> usableLand = new List<Land>();
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    string location = $"{GetColumnLetter(col)}{row}";
                    Land land = new Land(DefaultLandColor, 0, location, null);
                    Lands.Add(land);
                    usableLand.Add(land);
                }
            }

            // Place initial tiles
            List<Land> availableTiles = new List<Land>();
            Random rand = new Random();
            int numTiles = GetInitialTileCount(players.Count);
            for (int i = 0; i < numTiles; i++)
            {
                Land land = PlaceTile(rand, availableTiles, usableLand);
                availableTiles.Add(land);
            }
        }

        // Methods
        private char GetColumnLetter(int col) => (char)('A' + col);

        private int GetInitialTileCount(int playerCount) => playerCount switch
        {
            2 => 16,
            3 => 24,
            4 => 32,
            _ => throw new ArgumentException("Invalid number of players")
        };

        private Land PlaceTile(Random rand, List<Land> availableTiles, List<Land> usableLand)
        {
            int currentplayerindex = Players.IndexOf(CurrentPlayer);
            if (availableTiles.Count == 0)
            {
                // Place first tile randomly
                int index = rand.Next(usableLand.Count);
                Land land = usableLand[index];
                land.AssignOwner(Players[rand.Next(Players.Count)]);
                usableLand.RemoveAt(index);
                return land;
            }
            else
            {
                // Place subsequent tiles adjacent to available tiles
                Land adjacentLand = GetAdjacentLand(rand, availableTiles);
                adjacentLand.AssignOwner(Players[rand.Next(Players.Count)]);
                usableLand.Remove(adjacentLand);
                return adjacentLand;
            }
        }

        private Land GetAdjacentLand(Random rand, List<Land> availableTiles)
        {
            Land adjacentLand = null;
            while (adjacentLand == null)
            {
                Land land = availableTiles[rand.Next(availableTiles.Count)];
                foreach (Land adjacent in GetAdjacentLands(land))
                {
                    if (!availableTiles.Contains(adjacent))
                    {
                        adjacentLand = adjacent;
                        break;
                    }
                }
            }
            return adjacentLand;
        }

        private IEnumerable<Land> GetAdjacentLands(Land land)
        {
            int row = int.Parse(land.Location.Substring(1));
            int col = GetColumnLetterIndex(land.Location[0]);

            foreach (Land adjacent in Lands)
            {
                int adjacentRow = int.Parse(adjacent.Location.Substring(1));
                int adjacentCol = GetColumnLetterIndex(adjacent.Location[0]);

                if (Math.Abs(row - adjacentRow) == 1 && col == adjacentCol)
                {
                    yield return adjacent;
                }
                else if (Math.Abs(col - adjacentCol) == 1 && row == adjacentRow)
                {
                    yield return adjacent;
                }
            }
        }

        private int GetColumnLetterIndex(char letter) => letter - 'A';

        public void NextTurn()
        {
            int currentPlayerIndex = Players.IndexOf(CurrentPlayer);
            CurrentPlayer = Players[(currentPlayerIndex + 1) % Players.Count];
            if (currentPlayerIndex == Players.Count - 1)
            {
                Round++;
            }
        }

        public void Attack(Player player, string from, string to)
        {
            Land fromLand = Lands.FirstOrDefault(land => land.Location == from);
            Land toLand = Lands.FirstOrDefault(land => land.Location == to);
            bool validattack = true;
            string reason = "";
            if (fromLand.Owner != player)
            {
                validattack = false;
                reason = "You do not own the attacking land";
            }
            else if (toLand.Owner == player)
            {
                validattack = false;
                reason = "You cannot attack your own land";
            }
            else if (!GetAdjacentLands(fromLand).Contains(toLand))
            {
                validattack = false;
                reason = "The attacking and defending lands are not adjacent";
            }
            else if (fromLand.Soldiers <= 1)
            {
                validattack = false;
                reason = "You do not have enough soldiers to attack";
            }
            else if (fromLand.Soldiers <= toLand.Soldiers)
            {
                validattack = false;
                reason = "You do not have enough soldiers to attack";
            }

            if (validattack)
            {
                Random rand = new Random();
                int attackerRoll = rand.Next(1, 7);
                int defenderRoll = rand.Next(1, 7);

                if (attackerRoll > defenderRoll)
                {
                    toLand.OwnerAttack(player, fromLand.Soldiers - 1);
                    fromLand.SetSoldiers(1);
                    Console.WriteLine($"{player.Name} has taken {toLand.Location}");
                }
                else
                {
                    Random random = new Random();
                    int soldiers = random.Next(1, fromLand.Soldiers);
                    fromLand.SetSoldiers(soldiers);
                }
                Console.WriteLine($"Attacker rolled {attackerRoll}");
                Console.WriteLine($"Defender rolled {defenderRoll}");
                Console.WriteLine($"Attack was {(attackerRoll > defenderRoll ? "successful" : "unsuccessful")}");
            }
            else
            {
                Console.WriteLine(reason);
            }
        }

        public void PlaceSoldier(Player player, string location, int soldiers)
        {
            Land land = Lands.FirstOrDefault(l => l.Location == location);
            if (land.Owner == player)
            {
                soldiers += land.Soldiers;
                land.SetSoldiers(soldiers);
            }
            else
            {
                Console.WriteLine("You do not own this land");
            }
        }

        public void RelocateSoldiers(Player player, string from, string to, int soldiers)
        {
            Land fromLand = Lands.FirstOrDefault(l => l.Location == from);
            Land toLand = Lands.FirstOrDefault(l => l.Location == to);
            if (fromLand.Owner == player && toLand.Owner == player)
            {
                if (fromLand.Soldiers > soldiers)
                {
                    fromLand.SetSoldiers(fromLand.Soldiers - soldiers);
                    toLand.SetSoldiers(toLand.Soldiers + soldiers);
                }
                else
                {
                    Console.WriteLine("You do not have enough soldiers to relocate");
                }
            }
            else
            {
                Console.WriteLine("You do not own one of the lands");
            }
        }

        public void SetID(int id)
        {
            ID = id;
        }

        public void SetRound(int round)
        {
            Round = round;
        }

        public void SetCurrentPlayer(Player player)
        {
            CurrentPlayer = player;
        }


    }
}
