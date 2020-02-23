using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SQLConnector : IDataConnection
    {
        public void CompleteTournament(TournamentModel model)
        {
            throw new NotImplementedException();
        }

        public void CreatePerson(PersonModel model)
        {
            throw new NotImplementedException();
        }

        public void CreatePrize(PrizeModel model)
        {
            throw new NotImplementedException();
        }

        public void CreateTeam(TeamModel model)
        {
            throw new NotImplementedException();
        }

        public void CreateTournament(TournamentModel model)
        {
            throw new NotImplementedException();

            //TournamentLogic.UpdateTournamentResults(tm);
        }

        public List<PersonModel> GetPerson_All()
        {
            throw new NotImplementedException();
        }

        public List<TeamModel> GetTeam_All()
        {
            throw new NotImplementedException();
        }

        public List<TournamentModel> GetTournament_All()
        {
            throw new NotImplementedException();
        }

        public void UpdateMatchup(MatchupModel model)
        {
            throw new NotImplementedException();
        }
    }
}
