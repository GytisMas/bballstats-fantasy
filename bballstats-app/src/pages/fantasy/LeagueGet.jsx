import { useAuth } from "../../provider/Authentication";
import {BearerAuth, ButtonStyle, FormMdContainerStyle, FormMemberStyle, FormTableStyle, FormWiderContainerStyle, LinkStyle} from '../../components/Helpers';
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
  const [templateId, setTemplateId] = useState(1);
  const [leagueRoles, setLeagueRoles] = useState([]);
  const [leagueRolesUnavailable, setLeagueRolesUnavailable] = useState([]);
  const [league, setLeague] = useState([]);
  const [leaguePayments, setLeaguePayments] = useState([]);
  const [participants, setParticipants] = useState([]);

  const navigate = useNavigate();

  const leagueStatus = (l) => {
    const utcNow = new Date().toISOString();
    return l.endDate > utcNow && !league.notStarted ? 
      "Active" : 
      league.notStarted ? "Upcoming" : "Ended";
  }

  useEffect(() => {
    const loadData = async () => {
      const response = (await axios.get(APIEndpoint + '/Fantasy/Leagues/' + params.leagueId));
      console.log(response.data);
      setLeague(response.data);
      setLeaguePayments(response.data.payments);
      setTemplateId(response.data.templateId);
      setParticipants(response.data.participants);
    }

    loadData().then(() => setIsLoading(false));
    }, [isLoading]);
    
    return (isLoading ? <div className='mt-5 max-w-xl mx-auto px-2 bg-white border-2 rounded-3xl'><p>Loading...</p></div> :
      <>
        <div className='mt-5 max-w-xl mx-auto px-2 flex flex-col items-center bg-white border-2 rounded-3xl'>
          <p className='py-2 font-bold text-xl text-center'>{league.name}</p>
          {/* <p>Created by: {league.leagueHostName}</p> */}
          <p>Entry fee: {league.entryfee}</p>
          <p>Start date: {new Date(league.startDate+'z').toISOString().substring(0,10)}</p>
          <p>End date: {new Date(league.endDate+'z').toISOString().substring(0,10)}</p>
          <p>Status: {leagueStatus(league)}</p>
          {league.userParticipantId != null ?
            <button className={ButtonStyle + " w-1/4 mb-2"} type="button" onClick={() => navigate('/fantasy/leagues/'+params.leagueId+'/participants/'+league.userParticipantId)}>View Current Team</button>
            : league.notStarted &&
            <button className={ButtonStyle + " w-1/4 mb-2"} type="button" onClick={() => navigate('/fantasy/leagues/'+league.id+'/participate')}>Join League</button>
          }
          <Link to={'/fantasy/templates/'+templateId} target="_blank" rel="noopener noreferrer" ><button type="button" className={ButtonStyle + ' mb-2'}>View Template</button></Link>
        </div>
        <div className='flex flex-col flex-wrap justify-center items-center mt-5 max-w-xl mx-auto px-2 py-10 bg-white border-2 rounded-3xl'>
          {participants == null || participants.length == 0 ? <div>There are no participants in this league</div>
          :  <>
              <p className='py-2 font-bold text-md text-center'>League Standings</p>
              <table className="my-2">
                <thead>
                  <tr>
                    <th className='p-2 border-b text-left'>Team</th>
                    <th className='p-2 border-b text-center'>User</th>
                    <th className='p-2 border-b text-center'>Points</th>
                  </tr>
                </thead>
                <tbody>
                  {participants.map((participant) => (
                    <tr key={participant.id} className='w-72 mt-10'>
                      <td className='p-1 border-b text-left'>
                        <button type="button" className=" p-1 hover:rounded-lg hover:bg-slate-100" onClick={() => navigate('/fantasy/leagues/'+params.leagueId+'/participants/'+participant.id)}>{participant.teamName}</button>
                      </td>
                      <td className='p-2 border-b text-center'>{participant.userName}</td>
                      <td className='p-2 border-b text-center'>{participant.points.toFixed(1)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </>
          }
        </div>
        <div className='mt-5 max-w-xl mx-auto px-2 flex flex-col items-center bg-white border-2 rounded-3xl'>
          <div className="bg-white rounded px-4 py-2 my-4">
            <div className="text-xl text-center p-1 ">Prizes</div>
            <table className={FormTableStyle}>
              <thead>
                <tr>
                  <th className='p-2 border-2 border-black text-left'>Placement</th>
                  <th className='p-2 border-2 border-black text-left'>Prize</th>
                </tr>
              </thead>
              <tbody>

              {leaguePayments.map((it) => 
              <tr key={it.placing}>
                <td className='p-2 border-2 border-black text-left'>#{it.placing}</td>
                <td className='p-2 border-2 border-black text-right'>{it.amount}</td>
              </tr>
              )}
              </tbody>
            </table>
          </div>
        </div>
      </>
    );
}

export default LeagueGet;