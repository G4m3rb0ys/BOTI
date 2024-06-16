using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BOTI.DataAccessLayer
{
    public class DAL
    {
        readonly string connectionString = "Data Source=.;Initial Catalog=boti_game;Integrated Security=true";

        // Methode om een spel op te slaan
        public void SaveGame(Game game)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Sla het spel op
                string gameQuery = "INSERT INTO Game (Round, GameOver, CurrentPlayerID) OUTPUT INSERTED.ID VALUES (@Round, @GameOver, @CurrentPlayerID)";
                using (SqlCommand command = new SqlCommand(gameQuery, connection))
                {
                    command.Parameters.AddWithValue("@Round", game.Round);
                    command.Parameters.AddWithValue("@GameOver", game.GameOver);
                    command.Parameters.AddWithValue("@CurrentPlayerID", game.CurrentPlayer.ID);
                    int gameId = (int)command.ExecuteScalar();
                    game.SetID(gameId);
                }

                // Sla de spelers op
                foreach (var player in game.Players)
                {
                    string playerQuery = "INSERT INTO Player (Name, Color) OUTPUT INSERTED.ID VALUES (@Name, @Color)";
                    using (SqlCommand command = new SqlCommand(playerQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", player.Name);
                        command.Parameters.AddWithValue("@Color", player.Color);
                        int playerId = (int)command.ExecuteScalar();
                        player.SetID(playerId);
                    }

                    // Sla de GamePlayer-relatie op
                    string gamePlayerQuery = "INSERT INTO GamePlayer (PlayerID, GameID) VALUES (@PlayerID, @GameID)";
                    using (SqlCommand command = new SqlCommand(gamePlayerQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PlayerID", player.ID);
                        command.Parameters.AddWithValue("@GameID", game.ID);
                        command.ExecuteNonQuery();
                    }
                }

                // Sla de landen op
                foreach (var land in game.Lands)
                {
                    string landQuery = "INSERT INTO Land (Location, Color, Soldiers) OUTPUT INSERTED.ID VALUES (@Location, @Color, @Soldiers)";
                    using (SqlCommand command = new SqlCommand(landQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Location", land.Location);
                        command.Parameters.AddWithValue("@Color", land.Color);
                        command.Parameters.AddWithValue("@Soldiers", land.Soldiers);
                        int landId = (int)command.ExecuteScalar();
                        land.SetID(landId);
                    }

                    // Sla de GameLand-relatie op
                    string gameLandQuery = "INSERT INTO GameLand (LandID, GameID, OwnerID, Soldiers) VALUES (@LandID, @GameID, @OwnerID, @Soldiers)";
                    using (SqlCommand command = new SqlCommand(gameLandQuery, connection))
                    {
                        command.Parameters.AddWithValue("@LandID", land.ID);
                        command.Parameters.AddWithValue("@GameID", game.ID);
                        command.Parameters.AddWithValue("@OwnerID", (object)land.Owner?.ID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Soldiers", land.Soldiers);
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }

        // Methode om een spel te laden
        public Game LoadGame(int gameId)
        {
            Game game;
            List<Player> players = new List<Player>();
            List<Land> lands = new List<Land>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Laad het spel
                string gameQuery = "SELECT * FROM Game WHERE ID = @GameID";
                using (SqlCommand command = new SqlCommand(gameQuery, connection))
                {
                    command.Parameters.AddWithValue("@GameID", gameId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        game = new Game(new List<Player>())
                        {
                            GameOver = reader.GetBoolean(2)
                        };
                        game.SetID(reader.GetInt32(0));
                        game.SetRound(reader.GetInt32(1));
                    }
                }

                // Laad de spelers
                string playerQuery = "SELECT p.* FROM Player p INNER JOIN GamePlayer gp ON p.ID = gp.PlayerID WHERE gp.GameID = @GameID";
                using (SqlCommand command = new SqlCommand(playerQuery, connection))
                {
                    command.Parameters.AddWithValue("@GameID", gameId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Player player = new Player(reader.GetString(1), reader.GetString(2));
                            player.SetID(reader.GetInt32(0));
                            players.Add(player);
                        }
                    }
                }
                game.Players = players;

                // Laad de landen
                string landQuery = "SELECT l.*, gl.GameID, gl.OwnerID FROM Land l INNER JOIN GameLand gl ON l.ID = gl.LandID WHERE gl.GameID = @GameID";
                using (SqlCommand command = new SqlCommand(landQuery, connection))
                {
                    command.Parameters.AddWithValue("@GameID", gameId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Player owner = players.FirstOrDefault(p => p.ID == (reader.IsDBNull(5) ? -1 : reader.GetInt32(5)));
                            Land land = new Land(reader.GetString(2), reader.GetInt32(3), reader.GetString(1), owner);
                            land.SetID(reader.GetInt32(0));
                            lands.Add(land);
                        }
                    }
                }
                game.Lands = lands;

                // Controleer of spelers zijn geladen voordat CurrentPlayer wordt ingesteld
                if (players.Count > 0)
                {
                    game.SetCurrentPlayer(players[0]);
                }
                else
                {
                    throw new InvalidOperationException("No players loaded from the database.");
                }

                connection.Close();
            }
            Console.WriteLine("Press any key to continue...");
            string alwt = Console.ReadLine();
            // Print de geladen gegevens voor debuggen
            PrintLoadedGameInfo(game);
            Console.WriteLine("Press any key to continue...");
            string alt = Console.ReadLine();    
            return game;
        }

        private void PrintLoadedGameInfo(Game game)
        {
            Console.WriteLine("Game Loaded:");
            Console.WriteLine($"Game ID: {game.ID}");
            Console.WriteLine($"Round: {game.Round}");
            Console.WriteLine($"Game Over: {game.GameOver}");
            Console.WriteLine($"Current Player: {game.CurrentPlayer?.Name}");

            Console.WriteLine("Players:");
            foreach (var player in game.Players)
            {
                Console.WriteLine($"Player ID: {player.ID}, Name: {player.Name}, Color: {player.Color}");
            }

            Console.WriteLine("Lands:");
            foreach (var land in game.Lands)
            {
                Console.WriteLine($"Land ID: {land.ID}, Location: {land.Location}, Color: {land.Color}, Soldiers: {land.Soldiers}, Owner: {land.Owner?.Name}");
            }
        }


    }
}
