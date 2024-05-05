import { useAuth } from "../../provider/Authentication";
import {BearerAuth, ButtonStyle, FormMdContainerStyle, FormMemberStyle, FormWiderContainerStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle, FormHelperStyle} from '../../components/Helpers';
import { useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function LeagueParticipantGet(props) {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [isLoading, setIsLoading] = useState(true);
  const [isTemplLoading, setIsTemplLoading] = useState(true);
  const [leagueRoles, setLeagueRoles] = useState([]);
  const [leagueRolesUnavailable, setLeagueRolesUnavailable] = useState([]);
  const [participant, setParticipant] = useState([]);
  const [nextMatches, setNextMatches] = useState([]);
  const [team, setTeam] = useState([]);

  const navigate = useNavigate();

  useEffect(() => {
    const loadData = async () => {
      const participantResponse = (await axios.get(APIEndpoint + '/Fantasy/Leagues/' + params.leagueId +'/Participants/' + params.participantId));
      setParticipant(participantResponse.data);
      setTeam(participantResponse.data.team);
      const matchesResponse = (await axios.get(APIEndpoint + '/Participants/' + params.participantId +'/Matches/'));
      setNextMatches(matchesResponse.data)
    }

    loadData().then(() => setIsLoading(false));
  }, [isLoading]);
    
  function nextGameInfo (playerId) {
    const nextGame = nextMatches == null ? null : nextMatches.find(m => m.playerIds.includes(playerId))
    return nextGame != null ? 
    <label className="px-1 text-right">{new Date(nextGame.match.matchDate).toISOString().substring(0,10)}</label>
    :
      <label className="px-1 text-right">Unknown</label>
  }

  return (isLoading ? <div className='mt-5 max-w-xl mx-auto px-2 bg-white border-2 rounded-3xl'><p>Loading...</p></div> :
    <>
      <div className='mt-5 max-w-xl mx-auto px-2 flex flex-col items-center bg-white border-2 rounded-3xl'>
        <p className='py-2 font-bold text-xl text-center'>{participant.teamName}</p> 
        {participant.participantIsUser &&
          <button className={ButtonStyle + " w-1/4 mb-2"} type="button" onClick={() => navigate('/fantasy/leagues/'+params.leagueId+'/participate/'+participant.id)}>Edit Team</button>
        }
      </div>
      <div className='flex flex-col flex-wrap justify-center items-center mt-5 max-w-max mx-auto px-2 pb-10 bg-white border-2 rounded-3xl'>
        <p>User: {participant.userName}</p>
        <p>Points in League: {participant.points}</p>
        {team.map((player) => (
          <div key={player.id} className='w-72 mt-10 flex flex-col items-center bg-slate-200'>
            <p className="text-xl">{player.playerName}</p>
            <div className=" w-full flex flex-row justify-between">
              <label className="px-1 text-left">Role</label>
              <label className="px-1 text-right">{player.roleName}</label>
            </div>
            <div className=" w-full flex flex-row justify-between">
              <label className="px-1 text-left">Team</label>
              <label className="px-1 text-right">{player.teamName}</label>
            </div>
            <div className=" w-full flex flex-row justify-between">
              <label className="px-1 text-left">Points</label>
              <label className="px-1 text-right">{player.points} {player.pointsLastGame != 0 && "("+(player.pointsLastGame > 0 ? "+"+player.pointsLastGame : "-"+player.pointsLastGame)+")"}</label>
            </div>
            <div className=" w-full flex flex-row justify-between">
              <label className="px-1 text-left">Next game</label>
              {
                nextGameInfo(player.playerId)
              }
            </div>
          </div>
        ))}
      </div>
    </>
  );
}

export default LeagueParticipantGet;