import { useAuth } from "../../provider/Authentication";
import {BearerAuth, ButtonStyle, FormMdContainerStyle, FormMemberStyle, FormWiderContainerStyle, LinkStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle, FormHelperStyle} from '../../components/Helpers';
import { Link, useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function LeagueGet(props) {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [isLoading, setIsLoading] = useState(true);
  const [isTemplLoading, setIsTemplLoading] = useState(true);
  const [leagueRoles, setLeagueRoles] = useState([]);
  const [leagueRolesUnavailable, setLeagueRolesUnavailable] = useState([]);
  const [league, setLeague] = useState([]);
  const [participants, setParticipants] = useState([]);

  const navigate = useNavigate();

  useEffect(() => {
    const loadData = async () => {
      const response = (await axios.get(APIEndpoint + '/Fantasy/Leagues/' + params.leagueId));
      console.log(response.data);
      setLeague(response.data);
      setParticipants(response.data.participants);
    }

    loadData().then(() => setIsLoading(false));
    }, [isLoading]);
    
    return (isLoading ? <div className='mt-5 max-w-xl mx-auto px-2 bg-white border-2 rounded-3xl'><p>Loading...</p></div> :
      <>
        <div className='mt-5 max-w-xl mx-auto px-2 flex flex-col items-center bg-white border-2 rounded-3xl'>
          <p className='py-2 font-bold text-xl text-center'>{league.name}</p>
          <p>Created by: {league.leagueHostName}</p>
          <p>Entry fee: {league.entryfee}</p>
          <p>Start date: {new Date(league.startDate+'z').toISOString().substring(0,10)}</p>
          <p>End date: {new Date(league.endDate+'z').toISOString().substring(0,10)}</p>
          {league.userParticipantId != null ?
            <button className={ButtonStyle + " w-1/4 mb-2"} type="button" onClick={() => navigate('/fantasy/leagues/'+params.leagueId+'/participants/'+league.userParticipantId)}>View Current Team</button>
            : league.notStarted &&
            <button className={ButtonStyle + " w-1/4 mb-2"} type="button" onClick={() => navigate('/fantasy/leagues/'+league.id+'/participate')}>Join League</button>
          }
          <Link to={'/fantasy/templates/'+2} target="_blank" rel="noopener noreferrer" ><button type="button" className={ButtonStyle + ' mb-2'}>View Template</button></Link>
        </div>
        <div className='flex flex-col flex-wrap justify-center items-center mt-5 max-w-6xl mx-auto px-2 py-10 bg-white border-2 rounded-3xl'>
          {participants == null || participants.length == 0 ? <div>There are no participants in this league</div>
          :  <>
              <table className="my-2">
                <thead>
                  <tr>
                    <th className='p-2 border-2 text-left'>Team</th>
                    <th className='p-2 border-2 text-left'>User</th>
                    <th className='p-2 border-2 text-left'>Points</th>
                  </tr>
                </thead>
                <tbody>
                  {participants.map((participant) => (
                    <tr key={participant.id} className='w-72 mt-10'>
                      <td className='p-2 border-2 text-left font-semibold hover:font-bold'>
                        <a href={'/fantasy/leagues/'+params.leagueId+'/participants/'+participant.id}>{participant.teamName}</a>
                      </td>
                      <td className='p-2 border-2 text-left'>{participant.userName}</td>
                      <td className='p-2 border-2 text-left'>{participant.points.toFixed(1)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </>
          }
        </div>
      </>
    );
}

export default LeagueGet;