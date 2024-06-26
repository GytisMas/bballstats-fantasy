﻿using BBallStats.Shared;
using BBallStats.Shared.Entities;
using System.Net.Http.Json;
using System.Xml;
using System.Xml.Serialization;
using static BBallStats.Shared.Utils.DTOs;

namespace BballStatsFetcher
{
    internal class Program
    {
        const int pauseIntervalInMs = 2 * 1000;
        const int noConnectionIntervalInMs = 5 * 1000;
        const int longIntervalInMs = 10 * 1000;
        const string baseUrl = "https://urchin-app-97ttl.ondigitalocean.app/api";
        //const string baseUrl = "https://localhost:7140/api";
        static HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            string seasonCode;
            List<int> GameCodes;
            bool ignoreExisting = false;
            bool checkTeamsAndPlayers = true;
            while (true)
            {
                bool oneGameProcessed = false;
                seasonCode = "E" + (DateTime.UtcNow.Month < 8 ? DateTime.UtcNow.Year - 1 : DateTime.UtcNow.Year);

                Console.WriteLine($"Current date and time (UTC): {DateTime.UtcNow}");
                Console.WriteLine($"Ending active leagues that are over.");

                var url = $"{baseUrl}/fantasy/leagues/endLeagues";
                try
                {
                    var readResponse = await client.PatchAsync(url, null);
                    Console.WriteLine(readResponse.StatusCode);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine($"Checking Teams.");
                if (checkTeamsAndPlayers && !await CheckTeams(seasonCode))
                {
                    Console.WriteLine($"Error Checking Teams. Retrying in {noConnectionIntervalInMs}ms");
                    Thread.Sleep(noConnectionIntervalInMs);
                    continue;
                }
                checkTeamsAndPlayers = false;

                Console.WriteLine($"Getting unused games (season: {seasonCode} | listed games only: {!ignoreExisting})");
                GameCodes = await GetUnusedGames(seasonCode, ignoreExisting);

                if (ignoreExisting)
                {
                    Console.WriteLine($"Got game codes: {string.Join(", ", GameCodes)}");
                    int oldMaxGameCode = GameCodes.Max();
                    for (int i = oldMaxGameCode + 1; i <= oldMaxGameCode + 20; i++)
                    {
                        GameCodes.Add(i);
                    }
                }
                Console.WriteLine($"Processing game codes: {string.Join(", ", GameCodes)}");
                //Thread.Sleep(pauseIntervalInMs);


                foreach (int gameCode in GameCodes)
                {

                    Console.WriteLine($"Fetching game stats (season: {seasonCode} | game: {gameCode})");
                    var game = await FetchGameData(seasonCode, gameCode);

                    if (game == null)
                        continue;

                    if (game.Played)
                    {
                        if (!oneGameProcessed)
                            oneGameProcessed = !game.AlreadyExistedInDB || game.Played;

                        //Console.WriteLine($"\n Sending game stats (season {seasonCode} | game {gameCode})");
                        var sendResult = await SendGameData(game);
                    }
                    Console.WriteLine($"\nGame {gameCode} processed successfully (game was played: {game.Played} | game was already listed: {game.AlreadyExistedInDB})");
                }


                if (!oneGameProcessed)
                    ignoreExisting = !ignoreExisting;


                if (!oneGameProcessed && ignoreExisting)
                {
                    Thread.Sleep(longIntervalInMs);
                    checkTeamsAndPlayers = true;
                }
            }
        }

        private static async Task<List<int>> GetUnusedGames(string seasonCode, bool ignoreExisting)
        {
            var getGameResponse = await client.GetAsync($"{baseUrl}/Matches/unused/{seasonCode.Substring(1)}?ignoreExisting={ignoreExisting}");
            if (!getGameResponse.IsSuccessStatusCode)
            {
                throw new Exception($"{getGameResponse.StatusCode} - failed to fetch game");
            }
            return (await getGameResponse.Content.ReadFromJsonAsync<List<int>>()) ?? new List<int>();
        }

        private static async Task<bool> CheckTeams(string seasonCode)
        {
            var url = $"https://api-live.euroleague.net/v1/teams?seasonCode={seasonCode}";
            var readResponse = await client.GetAsync(url);
            if (!readResponse.IsSuccessStatusCode)
            {
                throw new Exception($"{readResponse.StatusCode} - failed to fetch teams");
            }

            string readContent = await readResponse.Content.ReadAsStringAsync();
            var xdoc = new XmlDocument();
            xdoc.LoadXml(readContent);
            xdoc.Save("teams.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(Clubs));
            Clubs teamData;
            using (Stream reader = new FileStream("teams.xml", FileMode.Open))
            {
                teamData = (Clubs)serializer.Deserialize(reader);
            }

            var teamsContent = JsonContent.Create(
                teamData.ClubsList
                .Select(c => new TeamWithPlayersDto(
                    c.Code, c.Clubname,
                    c.Roster.Players.Select(p => new PlayerNoTeamDto(p.Code, MakePlayerName(p.Name), p.Position)).ToArray()
                ))
                );
            try
            {
                var createTeamsResponse = await client.PutAsync($"{baseUrl}/Teams/", teamsContent);
                createTeamsResponse.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        private static async Task<Game?> FetchGameData(string seasonCode, int gameCode)
        {
            try
            {
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

                if (gameData == null)
                    throw new Exception(message: $"Game {seasonCode}-{gameCode} not found");

                gameData.Date = gameData.Cetdate.AddHours(-2);

                var matchContent = JsonContent.Create(
                        new CreateMatchDto(gameData.Code,
                        int.Parse(seasonCode.Substring(1)),
                        gameData.Localclub.Code,
                        gameData.Roadclub.Code,
                        gameData.Date)
                        );

                var getGameResponse = await client.PostAsync($"{baseUrl}/Matches/", matchContent);
                getGameResponse.EnsureSuccessStatusCode();
                gameData.AlreadyExistedInDB = getGameResponse.StatusCode == System.Net.HttpStatusCode.OK;

                return gameData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        private static async Task<bool> SendGameData(Game gameData)
        {
            try
            {
                var dataStatSheet = MakeStatSheet(gameData);

                await UpdateStats(client, dataStatSheet);

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
            var updateStatsResponse = await client.PostAsync($"{baseUrl}/Statistics/UpdateStats", updateStatsContent);
        }

        private static async Task UpdateFantasy(HttpClient client, StatSheet dataStatSheet)
        {
            var updateStatsContent = JsonContent.Create(dataStatSheet);
            var updateStatsResponse = await client.PostAsync($"{baseUrl}/fantasy/leagues/results", updateStatsContent);
        }



        private static StatSheet MakeStatSheet(Game game) 
        { 
            StatSheet statSheet = new StatSheet();

            statSheet.GameId = game.Code;
            statSheet.SeasonId = int.Parse(game.Seasoncode.Substring(1));

            statSheet.LocalClubId = game.Localclub.Code;
            statSheet.LocalClubName = game.Localclub.Name;

            statSheet.RoadClubId = game.Roadclub.Code;
            statSheet.RoadClubName = game.Roadclub.Name;
            statSheet.WinnerClubId = game.Localclub.Score > game.Roadclub.Score ? statSheet.LocalClubId : statSheet.RoadClubId;

            foreach (var stat in game.Localclub.Playerstats.Stat)
            {
                if (stat.PlayerCode == "0")
                    continue;

                statSheet.PlayerInfo = AddPlayerStat(statSheet.PlayerInfo, stat.PlayerCode, "TeamCode", statSheet.LocalClubId);
                statSheet.PlayerInfo = AddPlayerStat(statSheet.PlayerInfo, stat.PlayerCode, nameof(stat.PlayerName), MakePlayerName(stat.PlayerName));
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

        private static string MakePlayerName(string name)
        {
            string[] arr = name.Split(", ");
            return arr[1] + " " + arr[0];
        }
    }
}
