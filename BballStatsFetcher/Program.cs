using BBallStats.Shared;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Xml;
using System.Xml.Serialization;

namespace BballStatsFetcher
{
    internal class Program
    {
        static int intervalInMs = 7000;
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            var stat = new Stat();
            string seasonCode;
            int gameCode;
            //Thread.Sleep(intervalInMs);
            while (true)
            {
                //Console.WriteLine("Enter season year");
                //season = "E" + Console.ReadLine();
                
                // fetcherio reikalavimai:
                // rasti API neapdorotus matchus
                // siusti pasibaigusius matchus i API
                // rasti nezaistus matchus

                seasonCode = "E2023";
                Console.WriteLine("Enter game code");

                //gameCode = int.Parse(Console.ReadLine());
                gameCode = 239;

                Console.WriteLine($"\nSending request (season {seasonCode} | game {gameCode})");
                Console.WriteLine("Request result:");
                var game = FetchGameData(seasonCode, gameCode).Result;
                Console.WriteLine(game);
                Console.WriteLine("------");
                var sendResult = SendGameData(game).Result;

            }
            // https://localhost:7140/api/Statistics

            //while (true)
            //{
            //    Console.WriteLine($"{DateTime.Now:H:mm:ss} | {GetGames().Result.Substring(0, 10)}");
            //    Thread.Sleep(intervalInMs);
            //}
        }



        private static async Task<Game?> FetchGameData(string seasonCode, int gameCode)
        {
            try
            {
                // TODO: db saugomi atloadinti games
                var readResponse = await client.GetAsync($"https://api-live.euroleague.net/v1/games?seasonCode={seasonCode}&gameCode={gameCode}");
                if (!readResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"{readResponse.StatusCode} - failed to fetch game");
                }

                string readContent = await readResponse.Content.ReadAsStringAsync();
                var xdoc = new XmlDocument();
                xdoc.LoadXml(readContent);
                xdoc.Save("game_details.xml");

                XmlSerializer serializer = new XmlSerializer(typeof(Game));
                Game gameData;
                using (Stream reader = new FileStream("game_details.xml", FileMode.Open))
                {
                    gameData = (Game)serializer.Deserialize(reader);
                }
                if (gameData == null || !gameData.Played)
                    throw new Exception(message: "Game data of not played game was fetched");

                return gameData;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
                return null;
            }
        }
        private static async Task<bool> SendGameData(Game gameData)
        {
            try
            {
                var dataStatSheet = MakeStatSheet(gameData);

                //await Console.Out.WriteLineAsync("Sending to update stats.");
                //await UpdateStats(client, dataStatSheet);

                await Console.Out.WriteLineAsync("Sending to update FL.");
                await UpdateFantasy(client, dataStatSheet);
            } catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
                return false;
            }
            return true;
        }

        private static async Task UpdateStats(HttpClient client, StatSheet dataStatSheet)
        {
            var updateStatsContent = JsonContent.Create(dataStatSheet);
            var updateStatsResponse = await client.PostAsync("https://localhost:7140/api/Statistics/UpdateStats", updateStatsContent);
            Console.WriteLine(updateStatsResponse);
        }

        private static async Task UpdateFantasy(HttpClient client, StatSheet dataStatSheet)
        {
            var updateStatsContent = JsonContent.Create(dataStatSheet);
            var updateStatsResponse = await client.PostAsync("https://localhost:7140/api/fantasy/leagues/results", updateStatsContent);
            Console.WriteLine(updateStatsResponse);
        }

        private static StatSheet MakeStatSheet(Game game) 
        { 
            StatSheet statSheet = new StatSheet();

            statSheet.LocalClubId = game.Localclub.Totals.Total.ClubCode;
            statSheet.LocalClubName = game.Localclub.Totals.Total.ClubName;

            statSheet.RoadClubId = game.Roadclub.Totals.Total.ClubCode;
            statSheet.RoadClubName = game.Roadclub.Totals.Total.ClubName;
            statSheet.WinnerClubId = game.Localclub.Score > game.Roadclub.Score ? statSheet.LocalClubId : statSheet.RoadClubId;

            foreach (var stat in game.Localclub.Playerstats.Stat)
            {
                if (stat.PlayerCode == "0")
                    continue;

                statSheet.PlayerInfo = AddPlayerStat(statSheet.PlayerInfo, stat.PlayerCode, "TeamCode", statSheet.LocalClubId);
                statSheet.PlayerInfo = AddPlayerStat(statSheet.PlayerInfo, stat.PlayerCode, nameof(stat.PlayerName), stat.PlayerName);
                statSheet.PlayerInfo = AddPlayerStat(statSheet.PlayerInfo, stat.PlayerCode, nameof(stat.Dorsal), stat.Dorsal);

                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.TimePlayedSeconds), stat.TimePlayedSeconds);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.StartFive), stat.StartFive);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.Score), new[] { stat.Score, stat.AccuracyAttempted });
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, "FieldGoals", new[] { stat.FieldGoalsMadeTotal, stat.FieldGoalsAttemptedTotal});
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, "FieldGoals2", new[] { stat.FieldGoalsMade2, stat.FieldGoalsAttempted2 });
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, "FieldGoals3", new[] { stat.FieldGoalsMade3, stat.FieldGoalsAttempted3 });
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, "FreeThrows", new[] { stat.FreeThrowsMade, stat.FreeThrowsAttempted });
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.OffensiveRebounds), stat.OffensiveRebounds);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.DefensiveRebounds), stat.DefensiveRebounds);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.Steals), stat.Steals);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.BlocksFavour), stat.BlocksFavour);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.BlocksAgainst), stat.BlocksAgainst);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.Turnovers), stat.Turnovers);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.Assistances), stat.Assistances);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.PlusMinus), stat.PlusMinus);
            }

            foreach (var stat in game.Roadclub.Playerstats.Stat)
            {
                if (stat.PlayerCode == "0")
                    continue;

                statSheet.PlayerInfo = AddPlayerStat(statSheet.PlayerInfo, stat.PlayerCode, "TeamCode", statSheet.RoadClubId);
                statSheet.PlayerInfo = AddPlayerStat(statSheet.PlayerInfo, stat.PlayerCode, nameof(stat.PlayerName), stat.PlayerName);
                statSheet.PlayerInfo = AddPlayerStat(statSheet.PlayerInfo, stat.PlayerCode, nameof(stat.Dorsal), stat.Dorsal);

                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.TimePlayedSeconds), stat.TimePlayedSeconds);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.StartFive), stat.StartFive);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.Score), new[] { stat.Score, stat.AccuracyAttempted });
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, "FieldGoals", new[] { stat.FieldGoalsMadeTotal, stat.FieldGoalsAttemptedTotal });
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, "FieldGoals2", new[] { stat.FieldGoalsMade2, stat.FieldGoalsAttempted2 });
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, "FieldGoals3", new[] { stat.FieldGoalsMade3, stat.FieldGoalsAttempted3 });
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, "FreeThrows", new[] { stat.FreeThrowsMade, stat.FreeThrowsAttempted });
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.OffensiveRebounds), stat.OffensiveRebounds);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.DefensiveRebounds), stat.DefensiveRebounds);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.Steals), stat.Steals);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.BlocksFavour), stat.BlocksFavour);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.BlocksAgainst), stat.BlocksAgainst);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.Turnovers), stat.Turnovers);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.Assistances), stat.Assistances);
                statSheet.GameStats = AddPlayerStat(statSheet.GameStats, stat.PlayerCode, nameof(stat.PlusMinus), stat.PlusMinus);
            }

            return statSheet;
        }

        private static List<PlayerStat> AddPlayerStat(List<PlayerStat> StatsOrInfo, string playerCode, string statName, string value)
        {
            StatsOrInfo.Add(new PlayerStat()
            {
                PlayerCode = playerCode,
                StatName = statName,
                StatPropertyType = "string",
                StringVal = value
            });
            return StatsOrInfo;
        }

        private static List<PlayerStat> AddPlayerStat(List<PlayerStat> StatsOrInfo, string playerCode, string statName, bool value)
        {
            StatsOrInfo.Add(new PlayerStat()
            {
                PlayerCode = playerCode,
                StatName = statName,
                StatPropertyType = "bool",
                Boolval = value
            });
            return StatsOrInfo;
        }

        private static List<PlayerStat> AddPlayerStat(List<PlayerStat> StatsOrInfo, string playerCode, string statName, int value)
        {
            StatsOrInfo.Add(new PlayerStat()
            {
                PlayerCode = playerCode,
                StatName = statName,
                StatPropertyType = "int",
                IntVal = value
            });
            return StatsOrInfo;
        }

        private static List<PlayerStat> AddPlayerStat(List<PlayerStat> StatsOrInfo, string playerCode, string statName, int[] Values)
        {
            StatsOrInfo.Add(new PlayerStat()
            {
                PlayerCode = playerCode,
                StatName = statName,
                StatPropertyType = "int[]",
                IntArrVal = Values
            });
            return StatsOrInfo;
        }
    }
}
