using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        public const string PrizesFile = "PrizeModels.csv";
        public const string PeopleFile = "PersonModels.csv";
        public const string TeamsFile = "TeamModels.csv";
        public const string TournamentsFile = "TournamentModels.csv";
        public const string MatchupsFile = "MatchupModels.csv";
        public const string MatchupEntriesFile = "MatchupEntryModels.csv";

        public static IDataConnection Connection { get; private set; }
        public static void InitializeConnection(DatabaseType db)
        {
            if (db == DatabaseType.Sql)
            {
                //TODO - Create SQL Connector properly
                SQLConnector sql = new SQLConnector();
                Connection = sql;
            }
            else if (db == DatabaseType.TextFile)
            {
                //TODO - Create Text Connection
                TextConnector text = new TextConnector();
                Connection = text;
            }
        }

        public static string AppKeyLookup(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
