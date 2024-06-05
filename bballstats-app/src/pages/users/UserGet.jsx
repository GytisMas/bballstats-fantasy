import { useAuth } from "../../provider/Authentication";
import { useParams } from "react-router-dom";
import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from "react-router-dom";
import { jwtDecode } from 'jwt-decode'
import axios from 'axios';
import { APIEndpoint, ButtonStyle } from "../../components/Helpers";
import AlgorithmGet from "./AlgorithmGet";
import { BearerAuth } from "../../components/Helpers";
export default function UserGet(props) {
  let params = useParams();
  const { accessToken } = useAuth();
  const [user, setUser] = useState([]);
  const [balance, setBalance] = useState(0);
  const [algorithms, setAlgorithms] = useState([]);
  const [teams, setTeams] = useState([]);
  const [statTypes, setStatTypes] = useState([]);
  const [userId, setUserId] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();


  const handleRemoveItem = (id) => {
    setAlgorithms(algorithms.filter(item => item.id !== id));
  };

  useEffect(() => {
    const loadUserData = async () => {
      if (params.userId && accessToken && params.userId == jwtDecode(accessToken).sub) {
        navigate("/profile");
      }
      let userIdLocal = "";
      if (props.userId)
        userIdLocal = props.userId;
      else if (params.userId)
        userIdLocal = params.userId;
      setUserId(userIdLocal);
      const userResponse = await axios.get(APIEndpoint + '/users/' + userIdLocal);
      setUser(userResponse.data);
      if (props.noAlgorithms)
        return;
      if (props.personal) {
        const balanceResponse = await axios.get(APIEndpoint + '/users/' + userIdLocal + '/balance');
        setBalance(balanceResponse.data);
      }
      const algoResponse = await axios.get(APIEndpoint + '/users/' + userIdLocal + '/customStatistics/');
      setAlgorithms(algoResponse.data.sort(function(a, b){return a.id - b.id}));
      const statResponse = 
        Object.fromEntries((await axios.get(APIEndpoint + '/statistics/'))
        .data.sort(function(a, b){return a.id - b.id})
        .map((stat) => [stat.id, stat.displayName ?? stat.name]));
      setStatTypes(statResponse);

      const leagueTeamResponse = await axios.get(APIEndpoint + '/participants/byUser/' + userIdLocal);
      setTeams(leagueTeamResponse.data);

    }
    loadUserData().then(() => setIsLoading(false));
  }, []);


  if (props.noAlgorithms)
    return user.username;
  else
  return isLoading ? <div className='mt-5 max-w-xl mx-auto px-2 bg-white border-2 rounded-3xl'><p>Loading...</p></div> : (
    <div>
      <div className="flex flex-row mt-5 max-w-xl mx-auto px-2 py-2 items-center justify-evenly bg-white border-2 rounded-3xl">
        <div className="p-5 my-auto">
          <img className="w-48" src={process.env.PUBLIC_URL + '/icons/user-avatar-svgrepo-com.svg'}/>
        </div>
        <div className='flex flex-col justify-start items-start'>
          <p className="text-xs">User name</p>
          <p className="text-xl mb-5">{user.username}</p>
          {props.personal && 
          <div className="flex flex-col">
            <p className="text-xs">Email</p>
            <p className="text-xl mb-5">{user.email}</p>
            <p className="text-xs">Current Balance</p>
            <p className="text-xl mb-5">{balance}</p>
            <a href='/changePassword' className={ButtonStyle}>Change password</a>
            <a href='/algorithms/create' className={ButtonStyle}>Create new algorithm</a>
          </div>}
        </div>
      </div>
      {teams.length > 0 && <>
        <div className='mt-5 max-w-xl mx-auto px-2 bg-white border-2 rounded-3xl'>
          <p className='py-2 font-bold text-xl text-center'>User FL teams in active / upcoming leagues</p>
        </div>
        <div className='flex flex-col flex-wrap justify-center items-center my-5 max-w-3xl mx-auto px-2 py-10 bg-white border-2 rounded-3xl'>
          <table className="my-2">
            <thead>
              <tr>
                <th className='p-2 border-b text-left'>Team Name</th>
                <th className='p-2 border-b text-center'>League</th>
                <th className='p-2 border-b text-center'>Placing</th>
              </tr>
            </thead>
            <tbody>
              {teams.map((team) => (
                <tr key={team.id} className='w-72 mt-10'>
                  <td className='p-1 border-b text-left'>
                    <a className=" p-1 hover:rounded-lg hover:bg-slate-100" href={'/fantasy/leagues/'+team.leagueId+'/participants/'+team.id}>{team.teamName}</a>
                  </td>
                  <td className='p-2 border-b text-center'>
                    <a className=" p-1 hover:rounded-lg hover:bg-slate-100" href={'/fantasy/leagues/'+team.leagueId}>{team.leagueName}</a>
                  </td>
                  <td className='p-2 border-b text-center'>
                    {team.placement != -1 ? "#" + team.placement : "-"}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        </>
      }
      
      {algorithms.length > 0 && <>
        <div className='mt-5 max-w-xl mx-auto px-2 bg-white border-2 rounded-3xl'>
          <p className='py-2 font-bold text-xl text-center'>User custom statistics</p>
        </div>
        <div className="flex flex-row flex-wrap mt-5 max-w-6xl mx-auto px-2 pb-10 bg-white border-2 rounded-3xl">
          {algorithms.map((algo, i) => (
            <div key={algo.id} className='w-72 mx-10 pt-7'>
              <AlgorithmGet personal={props.userId ? true : false} onChange={handleRemoveItem} userId={user.id} algoId={algo.id} sTypes={statTypes}/>
            </div>
          ))} 
        </div>  
        </>
      }
    </div>
  );
}