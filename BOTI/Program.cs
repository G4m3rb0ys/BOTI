using System.CodeDom.Compiler;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;

namespace BOTI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string map = "";
            List<Player> players = new List<Player>();
            Console.WriteLine("Welcome to BOTI");
            Console.WriteLine("Pleas Enter Player 1 Name:");
            string player1Name = Console.ReadLine();
            Player player1 = new Player(player1Name, "Red");
            players.Add(player1);
            Console.WriteLine("Pleas Enter Player 2 Name:");
            string player2Name = Console.ReadLine();
            Player player2 = new Player(player2Name, "Blue");
            players.Add(player2);
            Game game = new Game(players);
            foreach (Land land in game.Lands)
                Console.Clear();
            Console.WriteLine("BOTI Game");
            foreach (Player player in game.Players)
            {
                Console.WriteLine($"Welcome to The game {player.Name}");
            }
            Console.WriteLine("Game Started");
            while (!game.GameOver)
            {
                Console.Clear();
                bool gameover = true;
                foreach (Land land in game.Lands)
                {
                    if (land.Owner == game.CurrentPlayer)
                    {
                        gameover = false;
                    }
                }
                if (gameover)
                {
                    Console.WriteLine($"Game Over! {game.CurrentPlayer.Name} Lost!");
                    game.GameOver = true;
                    break;
                }
                nextRound(game);
                Console.ReadLine();
            }
            

            void nextRound(Game game)
            {
                placementPhase(game);
                drafPhase(game);
                attackPhase(game);
                game.NextTurn();
            }

            void placementPhase(Game game)
            {
                Random random = new Random();
                int soldiersToPlace = random.Next(1, 9);
                while (soldiersToPlace > 0)
                {
                    Console.WriteLine($"You have {soldiersToPlace} soldiers left to place");
                    printGrid(game);
                    Console.WriteLine("Please enter the location of the land you want to place a soldier on:");
                    Console.WriteLine("Example: A1");
                    string location = Console.ReadLine();
                    Console.WriteLine("Please enter the amount of soldiers you want to place:");
                    int soldiersAmmount = int.Parse(Console.ReadLine());
                    game.PlaceSoldier(game.CurrentPlayer, location, soldiersAmmount);
                    soldiersToPlace -= soldiersToPlace;
                    Console.Clear();
                }
            }

            void drafPhase(Game game)
            {
                printGrid(game);
                Console.WriteLine("1: Relocate soldiers");
                Console.WriteLine("2: Skip Phase");
                string input = Console.ReadLine();
                if (input == "1")
                {
                    Console.WriteLine("Please enter the location of the land you want to relocate soldiers from:");
                    string from = Console.ReadLine();
                    Console.WriteLine("Please enter the location of the land you want to relocate soldiers to:");
                    string to = Console.ReadLine();
                    Console.WriteLine("Please enter the amount of soldiers you want to relocate:");
                    int soldiersAmmount = int.Parse(Console.ReadLine());
                    game.RelocateSoldiers(game.CurrentPlayer, from, to, soldiersAmmount);
                }
                else
                {
                    Console.Clear();
                }
            }

            void attackPhase(Game game)
            {
                printGrid(game);
                Console.WriteLine("Please enter the location of the land you want to attack from:");
                string from = Console.ReadLine();
                Console.WriteLine("Please enter the location of the land you want to attack:");
                string to = Console.ReadLine();
                game.Attack(game.CurrentPlayer, from, to);
            }

            void printGrid(Game game)
            {
                Console.WriteLine($"Round {game.Round}");
                Console.WriteLine($"It's {game.CurrentPlayer.Name}'s turn");
                Console.WriteLine($"Your collor is {game.CurrentPlayer.Color}");
                char[] columns = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
                char[,] grid = new char[10, 10];

                for (int row = 0; row < 10; row++)
                {
                    for (int col = 0; col < 10; col++)
                    {
                        grid[row, col] = ' ';
                    }
                }

                foreach (Land land in game.Lands)
                {
                    int row = int.Parse(land.Location[1].ToString());
                    int col = Array.IndexOf(columns, land.Location[0]);
                    if (land.Soldiers > 0)
                    {
                        if (land.Color == "Red")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                        }
                        grid[row, col] = (char)(land.Soldiers + '0');
                        Console.ForegroundColor = ConsoleColor.Gray; // Reset the console's foreground color
                    }
                    else
                    {
                        grid[row, col] = '#';
                    }
                }

                // Print the grid
                int linenummer = 0;
                Console.WriteLine("--A-B-C-D-E-F-G-H-I-J---");
                for (int row = 0; row < 10; row++)
                {
                    Console.Write(linenummer + "|");
                    for (int col = 0; col < 10; col++)
                    {
                        if (grid[row, col] >= '1' && grid[row, col] <= '9')
                        {
                            Land land = game.Lands.FirstOrDefault(l => l.Location == $"{columns[col]}{row}");
                            if (land != null && land.Color == "Red")
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                        }
                        Console.Write(grid[row, col]);
                        Console.ForegroundColor = ConsoleColor.Gray; // Reset the console's foreground color
                        Console.Write(" ");
                    }
                    Console.Write("| " + linenummer);
                    Console.WriteLine(); // Add this line to move to the next line after each row
                    linenummer++;
                }
                Console.WriteLine("--A-B-C-D-E-F-G-h-I-J---");
            }
        }
    }
}