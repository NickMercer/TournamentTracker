using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        public void CompleteTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentsFile.FullFilePath().LoadFile().ConvertToTournamentModels();

            model.TournamentName = $"COMPLETE: { model.TournamentName }";

            tournaments.SaveToTournamentsFile();

            TournamentLogic.UpdateTournamentResults(model);
        }

        public void CreatePerson(PersonModel model)
        {
            List<PersonModel> people = GetPerson_All();

            int currentId = 1;

            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            people.Add(model);

            people.SaveToPeopleFile();
        }
        public void CreatePrize(PrizeModel model)
        {
            //Load text file and convert to list of PrizeModel
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            //Find max ID
            int currentId = 1;

            if(prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            //Add new record with the new Id
            prizes.Add(model);

            //Convert prizes to list<string> and save to the file.
            prizes.SaveToPrizeFile();
        }
        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GetTeam_All();

            int currentId = 1;

            if (teams.Count > 0)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamsFile();
        }
        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentsFile.FullFilePath().LoadFile().ConvertToTournamentModels();

            int currentId = 1;

            if(tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            model.SaveRoundsToFile();

            tournaments.Add(model);

            tournaments.SaveToTournamentsFile();

            TournamentLogic.UpdateTournamentResults(model);
        }

        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }
        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
        }
        public List<TournamentModel> GetTournament_All()
        {
            return GlobalConfig.TournamentsFile.FullFilePath().LoadFile().ConvertToTournamentModels();
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }
    }
}
