using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BBallStats.Shared
{

    [XmlRoot(ElementName = "referee")]
    public class Referee
    {

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "countrycode")]
        public string Countrycode { get; set; }
    }

    [XmlRoot(ElementName = "referees")]
    public class Referees
    {

        [XmlElement(ElementName = "referee")]
        public List<Referee> Referee { get; set; }
    }

    [XmlRoot(ElementName = "partials")]
    public class Partials
    {

        [XmlAttribute(AttributeName = "Partial1")]
        public int Partial1 { get; set; }

        [XmlAttribute(AttributeName = "Partial2")]
        public int Partial2 { get; set; }

        [XmlAttribute(AttributeName = "Partial3")]
        public int Partial3 { get; set; }

        [XmlAttribute(AttributeName = "Partial4")]
        public int Partial4 { get; set; }

        [XmlAttribute(AttributeName = "ExtraPeriod1")]
        public int ExtraPeriod1 { get; set; }

        [XmlAttribute(AttributeName = "ExtraPeriod2")]
        public int ExtraPeriod2 { get; set; }

        [XmlAttribute(AttributeName = "ExtraPeriod3")]
        public int ExtraPeriod3 { get; set; }

        [XmlAttribute(AttributeName = "ExtraPeriod4")]
        public int ExtraPeriod4 { get; set; }

        [XmlAttribute(AttributeName = "ExtraPeriod5")]
        public int ExtraPeriod5 { get; set; }
    }

    public class StatSheet
    {
        public List<PlayerStat> PlayerInfo { get; set; } = new List<PlayerStat>();
        public List<PlayerStat> GameStats { get; set; } = new List<PlayerStat>();
        public string WinnerClubId { get; set; } = null!;
        public string LocalClubId { get; set; } = null!;
        public string RoadClubId { get; set; } = null!;
        public string LocalClubName { get; set; } = null!;
        public string RoadClubName { get; set; } = null!;
        public int SeasonId { get; set; }
        public int GameId { get; set; }
    }

    public class PlayerStat
    {
        public string PlayerCode { get; set; } = null!;
        public string StatName { get; set; } = null!;
        public string StatPropertyType { get; set; } = null!;
        public string? StringVal { get; set; }
        public bool? Boolval { get; set; }
        public int? IntVal { get; set; }
        public int[]? IntArrVal { get; set; }
    }

    [XmlRoot(ElementName = "stat")]
    public class Stat
    {

        [XmlElement(ElementName = "TimePlayed")]
        public string TimePlayed { get; set; }

        [XmlElement(ElementName = "TimePlayedSeconds")]
        public int TimePlayedSeconds { get; set; }

        [XmlElement(ElementName = "Dorsal")]
        public string Dorsal { get; set; }

        [XmlElement(ElementName = "PlayerCode")]
        public string PlayerCode { get; set; }

        [XmlElement(ElementName = "PlayerName")]
        public string PlayerName { get; set; }

        [XmlElement(ElementName = "PlayerAlias")]
        public string PlayerAlias { get; set; }

        [XmlElement(ElementName = "StartFive")]
        public bool StartFive { get; set; }

        [XmlElement(ElementName = "StartFive2")]
        public bool StartFive2 { get; set; }

        [XmlElement(ElementName = "Valuation")]
        public int Valuation { get; set; }

        [XmlElement(ElementName = "Score")]
        public int Score { get; set; }

        [XmlElement(ElementName = "FieldGoalsMade2")]
        public int FieldGoalsMade2 { get; set; }

        [XmlElement(ElementName = "FieldGoalsAttempted2")]
        public int FieldGoalsAttempted2 { get; set; }

        [XmlElement(ElementName = "FieldGoalsMade3")]
        public int FieldGoalsMade3 { get; set; }

        [XmlElement(ElementName = "FieldGoalsAttempted3")]
        public int FieldGoalsAttempted3 { get; set; }

        [XmlElement(ElementName = "FreeThrowsMade")]
        public int FreeThrowsMade { get; set; }

        [XmlElement(ElementName = "FreeThrowsAttempted")]
        public int FreeThrowsAttempted { get; set; }

        [XmlElement(ElementName = "FieldGoalsMadeTotal")]
        public int FieldGoalsMadeTotal { get; set; }

        [XmlElement(ElementName = "FieldGoalsAttemptedTotal")]
        public int FieldGoalsAttemptedTotal { get; set; }

        [XmlElement(ElementName = "AccuracyMade")]
        public int AccuracyMade { get; set; }

        [XmlElement(ElementName = "AccuracyAttempted")]
        public int AccuracyAttempted { get; set; }

        [XmlElement(ElementName = "TotalRebounds")]
        public int TotalRebounds { get; set; }

        [XmlElement(ElementName = "DefensiveRebounds")]
        public int DefensiveRebounds { get; set; }

        [XmlElement(ElementName = "OffensiveRebounds")]
        public int OffensiveRebounds { get; set; }

        [XmlElement(ElementName = "Assistances")]
        public int Assistances { get; set; }

        [XmlElement(ElementName = "Steals")]
        public int Steals { get; set; }

        [XmlElement(ElementName = "Turnovers")]
        public int Turnovers { get; set; }

        [XmlElement(ElementName = "Contras")]
        public int Contras { get; set; }

        [XmlElement(ElementName = "Dunks")]
        public int Dunks { get; set; }

        [XmlElement(ElementName = "BlocksFavour")]
        public int BlocksFavour { get; set; }

        [XmlElement(ElementName = "BlocksAgainst")]
        public int BlocksAgainst { get; set; }

        [XmlElement(ElementName = "FoulsCommited")]
        public int FoulsCommited { get; set; }

        [XmlElement(ElementName = "FoulsReceived")]
        public int FoulsReceived { get; set; }

        [XmlElement(ElementName = "PlusMinus")]
        public int PlusMinus { get; set; }

        [XmlElement(ElementName = "FieldGoals2")]
        public string FieldGoals2 { get; set; }

        [XmlElement(ElementName = "FieldGoals3")]
        public string FieldGoals3 { get; set; }

        [XmlElement(ElementName = "FreeThrows")]
        public string FreeThrows { get; set; }

        [XmlElement(ElementName = "FieldGoals2Percent")]
        public string FieldGoals2Percent { get; set; }

        [XmlElement(ElementName = "FieldGoals3Percent")]
        public string FieldGoals3Percent { get; set; }

        [XmlElement(ElementName = "FreeThrowsPercent")]
        public string FreeThrowsPercent { get; set; }

        [XmlElement(ElementName = "FieldGoalsPercent")]
        public double FieldGoalsPercent { get; set; }

        [XmlElement(ElementName = "AccuracyPercent")]
        public double AccuracyPercent { get; set; }

        [XmlElement(ElementName = "AssistancesTurnoversRatio")]
        public double AssistancesTurnoversRatio { get; set; }
    }

    [XmlRoot(ElementName = "playerstats")]
    public class Playerstats
    {

        [XmlElement(ElementName = "stat")]
        public List<Stat> Stat { get; set; }
    }

    [XmlRoot(ElementName = "total")]
    public class Total
    {

        [XmlElement(ElementName = "ClubCode")]
        public string ClubCode { get; set; }

        [XmlElement(ElementName = "GamesPlayed")]
        public int GamesPlayed { get; set; }

        [XmlElement(ElementName = "TimePlayed")]
        public string TimePlayed { get; set; }

        [XmlElement(ElementName = "TimePlayedSeconds")]
        public int TimePlayedSeconds { get; set; }

        [XmlElement(ElementName = "Valuation")]
        public int Valuation { get; set; }

        [XmlElement(ElementName = "Score")]
        public int Score { get; set; }

        [XmlElement(ElementName = "FieldGoalsMade2")]
        public int FieldGoalsMade2 { get; set; }

        [XmlElement(ElementName = "FieldGoalsAttempted2")]
        public int FieldGoalsAttempted2 { get; set; }

        [XmlElement(ElementName = "FieldGoalsMade3")]
        public int FieldGoalsMade3 { get; set; }

        [XmlElement(ElementName = "FieldGoalsAttempted3")]
        public int FieldGoalsAttempted3 { get; set; }

        [XmlElement(ElementName = "FreeThrowsMade")]
        public int FreeThrowsMade { get; set; }

        [XmlElement(ElementName = "FreeThrowsAttempted")]
        public int FreeThrowsAttempted { get; set; }

        [XmlElement(ElementName = "FieldGoalsMadeTotal")]
        public int FieldGoalsMadeTotal { get; set; }

        [XmlElement(ElementName = "FieldGoalsAttemptedTotal")]
        public int FieldGoalsAttemptedTotal { get; set; }

        [XmlElement(ElementName = "AccuracyMade")]
        public int AccuracyMade { get; set; }

        [XmlElement(ElementName = "AccuracyAttempted")]
        public int AccuracyAttempted { get; set; }

        [XmlElement(ElementName = "TotalRebounds")]
        public int TotalRebounds { get; set; }

        [XmlElement(ElementName = "DefensiveRebounds")]
        public int DefensiveRebounds { get; set; }

        [XmlElement(ElementName = "OffensiveRebounds")]
        public int OffensiveRebounds { get; set; }

        [XmlElement(ElementName = "Assistances")]
        public int Assistances { get; set; }

        [XmlElement(ElementName = "Steals")]
        public int Steals { get; set; }

        [XmlElement(ElementName = "Turnovers")]
        public int Turnovers { get; set; }

        [XmlElement(ElementName = "Contras")]
        public int Contras { get; set; }

        [XmlElement(ElementName = "Dunks")]
        public int Dunks { get; set; }

        [XmlElement(ElementName = "BlocksFavour")]
        public int BlocksFavour { get; set; }

        [XmlElement(ElementName = "BlocksAgainst")]
        public int BlocksAgainst { get; set; }

        [XmlElement(ElementName = "FoulsCommited")]
        public int FoulsCommited { get; set; }

        [XmlElement(ElementName = "FoulsReceived")]
        public int FoulsReceived { get; set; }

        [XmlElement(ElementName = "PlusMinus")]
        public int PlusMinus { get; set; }

        [XmlElement(ElementName = "FieldGoals2")]
        public string FieldGoals2 { get; set; }

        [XmlElement(ElementName = "FieldGoals3")]
        public string FieldGoals3 { get; set; }

        [XmlElement(ElementName = "FreeThrows")]
        public string FreeThrows { get; set; }

        [XmlElement(ElementName = "FieldGoals2Percent")]
        public double FieldGoals2Percent { get; set; }

        [XmlElement(ElementName = "FieldGoals3Percent")]
        public double FieldGoals3Percent { get; set; }

        [XmlElement(ElementName = "FreeThrowsPercent")]
        public double FreeThrowsPercent { get; set; }

        [XmlElement(ElementName = "FieldGoalsPercent")]
        public double FieldGoalsPercent { get; set; }

        [XmlElement(ElementName = "AccuracyPercent")]
        public double AccuracyPercent { get; set; }

        [XmlElement(ElementName = "AssistancesTurnoversRatio")]
        public int AssistancesTurnoversRatio { get; set; }

        [XmlElement(ElementName = "ClubName")]
        public string ClubName { get; set; }

        [XmlElement(ElementName = "ClubSeasonCode")]
        public string ClubSeasonCode { get; set; }
    }

    [XmlRoot(ElementName = "totals")]
    public class Totals
    {

        [XmlElement(ElementName = "total")]
        public Total Total { get; set; }
    }

    [XmlRoot(ElementName = "localclub")]
    public class Localclub
    {

        [XmlElement(ElementName = "partials")]
        public Partials Partials { get; set; }

        [XmlElement(ElementName = "pregamequotes")]
        public object Pregamequotes { get; set; }

        [XmlElement(ElementName = "playerstats")]
        public Playerstats Playerstats { get; set; }

        [XmlElement(ElementName = "totals")]
        public Totals Totals { get; set; }

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "tvcode")]
        public string Tvcode { get; set; }

        [XmlAttribute(AttributeName = "score")]
        public int Score { get; set; }

        [XmlAttribute(AttributeName = "coachcode")]
        public string Coachcode { get; set; }

        [XmlAttribute(AttributeName = "coachname")]
        public string Coachname { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "roadclub")]
    public class Roadclub
    {

        [XmlElement(ElementName = "partials")]
        public Partials Partials { get; set; }

        [XmlElement(ElementName = "pregamequotes")]
        public object Pregamequotes { get; set; }

        [XmlElement(ElementName = "playerstats")]
        public Playerstats Playerstats { get; set; }

        [XmlElement(ElementName = "totals")]
        public Totals Totals { get; set; }

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "tvcode")]
        public string Tvcode { get; set; }

        [XmlAttribute(AttributeName = "score")]
        public int Score { get; set; }

        [XmlAttribute(AttributeName = "coachcode")]
        public string Coachcode { get; set; }

        [XmlAttribute(AttributeName = "coachname")]
        public string Coachname { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "tv")]
    public class Tv
    {

        [XmlElement(ElementName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "url")]
        public string Url { get; set; }

        [XmlElement(ElementName = "countrycode")]
        public string Countrycode { get; set; }

        [XmlElement(ElementName = "countryname")]
        public string Countryname { get; set; }

        [XmlElement(ElementName = "broadcastdatetime")]
        public DateTime Broadcastdatetime { get; set; }

        [XmlElement(ElementName = "deferred")]
        public bool Deferred { get; set; }
    }

    [XmlRoot(ElementName = "tvschedules")]
    public class Tvschedules
    {

        [XmlElement(ElementName = "tv")]
        public List<Tv> Tv { get; set; }
    }

    [XmlRoot(ElementName = "records")]
    public class Records
    {

        [XmlAttribute(AttributeName = "localwins")]
        public int Localwins { get; set; }

        [XmlAttribute(AttributeName = "roadwins")]
        public int Roadwins { get; set; }

        [XmlAttribute(AttributeName = "localhomewins")]
        public int Localhomewins { get; set; }

        [XmlAttribute(AttributeName = "localhomelooses")]
        public int Localhomelooses { get; set; }

        [XmlAttribute(AttributeName = "localawaywins")]
        public int Localawaywins { get; set; }

        [XmlAttribute(AttributeName = "localawaylooses")]
        public int Localawaylooses { get; set; }

        [XmlAttribute(AttributeName = "roadhomewins")]
        public int Roadhomewins { get; set; }

        [XmlAttribute(AttributeName = "roadhomelooses")]
        public int Roadhomelooses { get; set; }

        [XmlAttribute(AttributeName = "roadawaywins")]
        public int Roadawaywins { get; set; }

        [XmlAttribute(AttributeName = "roadawaylooses")]
        public int Roadawaylooses { get; set; }
    }

    [XmlRoot(ElementName = "game")]
    public class Game
    {
        public bool AlreadyExistedInDB { get; set; }

        [XmlAttribute(AttributeName = "year")]
        public int Year { get; set; }

        [XmlAttribute(AttributeName = "seasoncode")]
        public string Seasoncode { get; set; }

        [XmlAttribute(AttributeName = "date")]
        public DateTime Date { get; set; }

        [XmlAttribute(AttributeName = "phasetypecode")]
        public string Phasetypecode { get; set; }

        [XmlAttribute(AttributeName = "phasetypename")]
        public string Phasetypename { get; set; }

        [XmlAttribute(AttributeName = "local")]
        public string Local { get; set; }

        [XmlAttribute(AttributeName = "road")]
        public string Road { get; set; }

        [XmlAttribute(AttributeName = "localscore")]
        public int Localscore { get; set; }

        [XmlAttribute(AttributeName = "roadscore")]
        public int Roadscore { get; set; }

        [XmlElement(ElementName = "audience")]
        public int Audience { get; set; }

        [XmlElement(ElementName = "audienceconfirmed")]
        public bool Audienceconfirmed { get; set; }

        [XmlElement(ElementName = "referees")]
        public Referees Referees { get; set; }

        [XmlElement(ElementName = "localclub")]
        public Localclub Localclub { get; set; }

        [XmlElement(ElementName = "roadclub")]
        public Roadclub Roadclub { get; set; }

        [XmlElement(ElementName = "tvschedules")]
        public Tvschedules Tvschedules { get; set; }

        [XmlElement(ElementName = "history")]
        public History History { get; set; }

        [XmlAttribute(AttributeName = "xsi")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "xsd")]
        public string Xsd { get; set; }

        [XmlAttribute(AttributeName = "code")]
        public int Code { get; set; }

        [XmlAttribute(AttributeName = "played")]
        public bool Played { get; set; }

        [XmlAttribute(AttributeName = "number")]
        public int Number { get; set; }

        [XmlAttribute(AttributeName = "cetdate")]
        public DateTime Cetdate { get; set; }

        [XmlAttribute(AttributeName = "localtimezone")]
        public int Localtimezone { get; set; }

        [XmlAttribute(AttributeName = "stadium")]
        public string Stadium { get; set; }

        [XmlAttribute(AttributeName = "stadiumname")]
        public string Stadiumname { get; set; }

        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }

        [XmlAttribute(AttributeName = "ticketsurl")]
        public string Ticketsurl { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "games")]
    public class Games
    {

        [XmlElement(ElementName = "game")]
        public List<Game> Game { get; set; }
    }

    [XmlRoot(ElementName = "history")]
    public class History
    {

        [XmlElement(ElementName = "records")]
        public Records Records { get; set; }

        [XmlElement(ElementName = "games")]
        public Games Games { get; set; }
    }


}
