import { useEffect, useState } from 'react';
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";
import PlayerStatsGet from '../playerStats/PlayerStatsGet';
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import { roles } from '../../components/PlayerRoles';
import { useNavigate } from "react-router-dom";
export default function TeamPlayersGet(props) {
  const [isLoading, setIsLoading] = useState(true);
  const [players, setPlayers] = useState([]);
  const [showStats, setShowStats] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const loadPlayer = async () => {
        const response = (await axios.get(APIEndpoint + '/teams/' 
            + props.teamId + '/players/'
        ));
        setPlayers(response.data.sort(function(a, b){return a.id - b.id}));
        const statResponse = 
          Object.fromEntries(response.data.sort(function(a, b){return a.id - b.id})
          .map((player) => [player.id, false]));
        setShowStats(statResponse)
        setIsLoading(false)
    }
    loadPlayer();
  }, []);

  const handleInfo = (playerId, val) => {
    setShowStats(showStats => ({...showStats, [playerId]: val}))
  }

  const handleInfoNoVal = (playerId) => {
    let val = showStats[playerId]
    setShowStats(showStats => ({...showStats, [playerId]: !val}))
  }

  const handleUpdate = (playerId) => {
    navigate('/players/update/'+props.teamId+'/'+playerId); 
  }

  const handleDelete = (playerId) => {
    navigate('/players/delete/'+props.teamId+'/'+playerId); 
  }
  
  if (isLoading) return "Loading..."

  return (
    <div className='flex flex-col justify-start mt-1 items-stretch'>
        {players.map((player) => (
          <div key={player.id} className='flex flex-col min-w-max'>
            <div className='px-2 flex flex-row flex-nowrap justify-between min-w-max items-center border-b-2'>
              <button onClick={() => handleInfoNoVal(player.id)} className='group inline-flex items-end h-3/4 w-4/5'>
                <p className='text-md text-wrap w-32 wrap group-hover:underline'>{player.name}</p>
                <p className='text-sm group-hover:underline ml-1 mr-3'>{roles[player.role]}</p>
              </button>
              {/* <PlayerStatsGet teamId={props.teamId} playerId={player.id}/> */}
              {props.isModerator && props.isModerator == true &&
                <div className='flex flex-row items-center'>
                  <button onClick={() => handleUpdate(player.id)}><img className='sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-pencil-24.png'}/></button>
                  <button onClick={() => handleDelete(player.id)}><img className='sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-trash-24.png'}/></button>
                </div>
              }
              
            </div> 
            <div>
              {!showStats[player.id] ? null
                : 
                <div className=' flex flex-col items-start border-b-4'>
                  <PlayerStatsGet isModerator={props.isModerator} teamId={props.teamId} playerId={player.id}/>                    
                </div>
              } 
            </div>
          </div>
                       
        ))}
    </div>
  );
}