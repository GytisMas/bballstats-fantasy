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
      setTeam(participantResponse.data.team.sort((a, b) => a.roleId > b.roleId ? 1 : -1));
      const matchesResponse = (await axios.get(APIEndpoint + '/Participants/' + params.participantId +'/Matches/'));
      console.log(participantResponse.data)
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
        <p>Created by: {participant.userName}</p>
        <p>Points in League: {participant.points.toFixed(1)}</p>
        {participant.participantIsUser &&
          <button className={ButtonStyle + " w-1/4 mb-2"} type="button" onClick={() => navigate('/fantasy/leagues/'+params.leagueId+'/participate/'+participant.id)}>Manage Team</button>
        }
      </div>
      <div className='flex flex-row flex-wrap justify-center items-stretch mt-5 max-w-6xl mx-auto px-2 pb-10 bg-white border-2 rounded-3xl'>
        {team.map((player) => (
          <div key={player.id} className='w-64 mt-10 mx-1 pt-1 pb-2 rounded-xl flex flex-col justify-start items-center bg-gradient-to-b bg-white'>
            {/* <p className="rounded-t-xl text-xl text-white text-center w-64 py-2 bg-gradient-to-b from-slate-500 to-black">{player.roleName}</p> */}
            <div className='w-64 h-24 flex border-2 border-slate-400 rounded-t-xl flex-col p-1 items-center justify-center bg-gradient-to-b from-slate-300 to-slate-100'>
              <p className="px-1 text-lg w-full text-left">{player.playerName}</p>
              <p className="px-1 text-slate-600 w-full text-sm text-left">{player.teamName}</p>
            </div>
            <div className='w-64 flex border-x-2 border-slate-400  flex-col p-1 py-5 items-center bg-slate-100'>
              <div className=" w-full flex flex-row justify-between">
                <label className="px-1 text-left">Points earned</label>
                <label className="px-1 text-right">{player.points.toFixed(1)} {player.pointsLastGame != 0 && "("+(player.pointsLastGame > 0 ? "+"+player.pointsLastGame.toFixed(1) : player.pointsLastGame.toFixed(1))+")"}</label>
              </div>
              <div className=" w-full flex flex-row justify-between">
                <label className="px-1 text-left">Next game date</label>
                {
                  nextGameInfo(player.playerId)
                }
              </div>
            </div>
            <div className='w-64 flex border-2 border-slate-400  flex-col rounded-b-xl p-1 items-center bg-slate-200 bg-gradient-to-b from-slate-100 to-slate-300'>
              <p className="px-1 w-full text-sm text-left">Fantasy role</p>
              <p className="px-1 w-full text-lg text-left">{player.roleName}</p>
            </div>
          </div>
        ))}
      </div>
    </>
  );
}

export default LeagueParticipantGet;