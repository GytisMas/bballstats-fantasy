import { useEffect, useState } from 'react';
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";
import { statTypes } from '../players/PlayersList';
import { useNavigate } from "react-router-dom";
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
export default function PlayerStatsGet(props) {
  const [playerStats, setPlayerStats] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const loadPlayer = async () => {
        const response = (await axios.get(APIEndpoint + '/teams/' 
            + props.teamId + '/players/' + props.playerId + '/playerStatistics'
        ));
        setPlayerStats(response.data.sort(function(a, b){return a.statType - b.statType}));
    }
    loadPlayer();
  }, []);

  const StatName = (id) => {
    if (!localStorage.getItem('statTypes')) return id;
    const stat = (JSON.parse(localStorage.getItem('statTypes'))).find((el) => el.id == id);
    return stat.displayName ?? stat.name;
  }

  let StatVisibility = (id) => {
    console.log((JSON.parse(localStorage.getItem('statTypes'))).find((el) => el.id == id));
    return localStorage.getItem('statTypes') ? 
      (JSON.parse(localStorage.getItem('statTypes'))).find((el) => el.id == id).status
      : id;
  }

  const handleUpdate = (statId) => {
    navigate('/players/statUpdate/'+props.teamId+'/'+props.playerId+'/'+statId); 
  }

  const handleDelete = (statId) => {
    navigate('/players/statDelete/'+props.teamId+'/'+props.playerId+'/'+statId); 
  }

  return (
    <>
        {props.isModerator && props.isModerator == true &&
          <div className='border-t-4 border-b-2 p-2 w-full'>
            <a href={'/players/statCreate/'+props.teamId+'/'+props.playerId} className='bth p-1 my-1 duration-200 bg-slate-100 hover:bg-blue-100 border-2 '>Create player statistic</a>
          </div>
        }
        {playerStats.map((playerStat) => (
          <div className=' w-full  border-b-2' key={playerStat.id}>
            {(!props.isModerator && StatVisibility(playerStat.statType) != 1) ?
              null
              : 
              <div className='w-full flex flex-row flex-nowrap items-center justify-between'>
                <div className='flex flew-row items-center justify-between w-4/5'>
                  <p className='text-sm ml-1'>{StatName(playerStat.statType)}</p>
                  <p className='font-bold mr-2 text-md'>{+(playerStat.value / playerStat.gameCount).toFixed(2)}</p>
                </div>
                {props.isModerator && props.isModerator == true &&
                  <div className='flex flex-row flex-nowrap items-center'>
                    <button onClick={() => handleUpdate(playerStat.id)}><img className='sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-pencil-24.png'}/></button>
                    <button onClick={() => handleDelete(playerStat.id)}><img className='sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-trash-24.png'}/></button>
                  </div>
                }
              </div>    
            }  
          </div>

        ))}
    </>
  );
}