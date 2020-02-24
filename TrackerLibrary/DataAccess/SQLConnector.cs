using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SQLConnector : IDataConnection
    {
        private const string db = "TournamentTracker";

        public void CompleteTournament(TournamentModel model)
        {
            //throw new NotImplementedException();
        }

        public void CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var person = new DynamicParameters();
                person.Add("@FirstName", model.FirstName);
                person.Add("@LastName", model.LastName);
                person.Add("@EmailAddress", model.EmailAddress);
                person.Add("@CellphoneNumber", model.CellphoneNumber);
                person.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", person, commandType: CommandType.StoredProcedure);

                model.Id = person.Get<int>("@Id");
            }
        }

        public void CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var prize = new DynamicParameters();
                prize.Add("@PlaceNumber", model.PlaceNumber);
                prize.Add("@PlaceName", model.PlaceName);
                prize.Add("@PrizeAmount", model.PrizeAmount);
                prize.Add("@PrizePercentage", model.PrizePercentage);
                prize.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", prize, commandType: CommandType.StoredProcedure);

                model.Id = prize.Get<int>("@Id");
            }
        }

        public void CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                var team = new DynamicParameters();
                team.Add("@TeamName", model.TeamName);
                team.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert", team, commandType: CommandType.StoredProcedure);

                model.Id = team.Get<int>("@Id");

                foreach(PersonModel person in model.TeamMembers)
                {
                    var member = new DynamicParameters();
                    member.Add("@TeamId", model.Id);
                    member.Add("@PersonId", person.Id);

                    connection.Execute("dbo.spTeamMembers_Insert", member, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                SaveTournament(connection, model);
                SaveTournamentPrizes(connection, model);
                SaveTournamentEntries(connection, model);
                SaveTournamentRounds(connection, model);
            }
        }

        private void SaveTournament(IDbConnection connection, TournamentModel model)
        {
            var tournament = new DynamicParameters();
            tournament.Add("@TournamentName", model.TournamentName);
            tournament.Add("@EntryFee", model.EntryFee);
            tournament.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert", tournament, commandType: CommandType.StoredProcedure);

            model.Id = tournament.Get<int>("@Id");
        }
        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            foreach (PrizeModel prize in model.Prizes)
            {
                var pz = new DynamicParameters();
                pz.Add("@TournamentId", model.Id);
                pz.Add("@PrizeId", prize.Id);
                pz.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentPrizes_Insert", pz, commandType: CommandType.StoredProcedure);
            }
        }
        private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
        {
            foreach (TeamModel tm in model.EnteredTeams)
            {
                var team = new DynamicParameters();
                team.Add("@TournamentId", model.Id);
                team.Add("@TeamId", tm.Id);
                team.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentEntries_Insert", team, commandType: CommandType.StoredProcedure);
            }
        }
        private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
        {
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach(MatchupModel matchup in round)
                {
                    var match = new DynamicParameters();
                    match.Add("@TournamentId", model.Id);
                    match.Add("@MatchupRound", matchup.MatchupRound);
                    match.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spMatchups_Insert", match, commandType: CommandType.StoredProcedure);

                    matchup.Id = match.Get<int>("@Id");

                    foreach(MatchupEntryModel entry in matchup.Entries)
                    {
                        var matchEntry = new DynamicParameters();
                        matchEntry.Add("@MatchupId", matchup.Id);

                        if (entry.ParentMatchup == null)
                        {
                            matchEntry.Add("@ParentMatchupId", null);
                        }
                        else
                        {
                            matchEntry.Add("@ParentMatchupId", entry.ParentMatchup.Id);
                        }

                        if(entry.TeamCompeting == null)
                        {
                            matchEntry.Add("@TeamCompetingId", null);
                        }
                        else
                        {
                            matchEntry.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                        }
                        matchEntry.Add("@Id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("dbo.spMatchupEntries_Insert", matchEntry, commandType: CommandType.StoredProcedure);
                    }
                }
            }
        }

        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeams_GetAll").ToList();

                foreach (TeamModel team in output)
                {
                    var teamParams = new DynamicParameters();
                    teamParams.Add("@TeamId", team.Id);
                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", teamParams, commandType: CommandType.StoredProcedure).ToList();
                }
            }

            return output;
        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();

                foreach (TournamentModel tournament in output)
                {
                    //Populate Prizes
                    var param = new DynamicParameters();
                    param.Add("@TournamentId", tournament.Id);

                    tournament.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament", param, commandType: CommandType.StoredProcedure).ToList();

                    //Populate Teams
                    param = new DynamicParameters();
                    param.Add("@TournamentId", tournament.Id);

                    tournament.EnteredTeams = connection.Query<TeamModel>("dbo.spTeams_GetByTournament", param, commandType: CommandType.StoredProcedure).ToList();

                    foreach (TeamModel team in tournament.EnteredTeams)
                    {
                        var teamParams = new DynamicParameters();
                        teamParams.Add("@TeamId", team.Id);
                        team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", teamParams, commandType: CommandType.StoredProcedure).ToList();
                    }

                    //Populate Rounds
                    param = new DynamicParameters();
                    param.Add("@TournamentId", tournament.Id);

                    List<MatchupModel> matchups = connection.Query<MatchupModel>("spMatchups_GetByTournament", param, commandType: CommandType.StoredProcedure).ToList();

                    foreach (MatchupModel matchup in matchups)
                    {
                        param = new DynamicParameters();
                        param.Add("@MatchupId", matchup.Id);

                        matchup.Entries = connection.Query<MatchupEntryModel>("spMatchupEntries_GetByMatchup", param, commandType: CommandType.StoredProcedure).ToList();

                        List<TeamModel> allTeams = GetTeam_All();

                        //Populate each matchup (1 model)
                        if(matchup.WinnerId > 0)
                        {
                            matchup.Winner = allTeams.Where(x => x.Id == matchup.WinnerId).First();
                        }

                        //Populate each entry (2 models)
                        foreach (var entry in matchup.Entries)
                        {
                            if (entry.TeamCompetingId > 0)
                            {
                                entry.TeamCompeting = allTeams.Where(x => x.Id == entry.TeamCompetingId).First();
                            }
                            
                            if(entry.ParentMatchupId > 0)
                            {
                                entry.ParentMatchup = matchups.Where(x => x.Id == entry.ParentMatchupId).First();
                            }
                        }
                    }

                    //Rounds - List<List<MatchupModel>>
                    List<MatchupModel> currentRow = new List<MatchupModel>();
                    int currentRound = 1;

                    foreach(MatchupModel matchup in matchups)
                    {
                        if(matchup.MatchupRound > currentRound)
                        {
                            tournament.Rounds.Add(currentRow);
                            currentRow = new List<MatchupModel>();
                            currentRound++;
                        }

                        currentRow.Add(matchup);
                    }

                    tournament.Rounds.Add(currentRow);
                }
            }

            return output;
        }

        public void UpdateMatchup(MatchupModel model)
        {
            //throw new NotImplementedException();
        }
    }
}
