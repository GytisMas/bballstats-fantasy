import { useEffect, useState, useCallback } from 'react';
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";
import { useAuth } from "../../provider/Authentication";
import BearerAuth from '../../components/Helpers';
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import TeamPlayersGet from './TeamPlayersGet';
import Modal from '../../components/Modal';

export default function PlayersList(props) {
  const { accessToken } = useAuth();
  const [teams, setTeams] = useState([]);
  const [modalIsShowing, setModalIsShowing] = useState(false);
  const [targetTeam, setTargetTeam] = useState([]);
  const [updateModal, setUpdateModal] = useState(true);

  const showModalUpdate = (teamId) => {
    setTeamInfo(teamId);
    setModalIsShowing(true);
    setUpdateModal(true);
  };

  const showModalDelete = (teamId) => {
    setTeamInfo(teamId);
    setUpdateModal(false);
    setModalIsShowing(true);
  };

  const hideModal = () => {
    setModalIsShowing(false);
  };

  let setTeamInfo = async (teamId) => {
    try {
      const response = (await axios.get(APIEndpoint + '/teams/'+teamId));
      setTargetTeam(response.data)
    } catch (error) {
        console.log(error);
    }
  }

  let loadTeams = useCallback(async () => {
      try {
        const response = await axios.get(APIEndpoint + "/teams");
        setTeams(response.data.sort(function(a, b){return a.id - b.id}));
        const statResponse = await axios.get(APIEndpoint + '/statistics');
        localStorage.setItem('statTypes', JSON.stringify(statResponse.data))
    } catch (error) {
        console.log(error);
    }
  }, [])

  useEffect(() => {
    loadTeams();
  }, [loadTeams]);

  const teamUpdateForm = (submitFunc) => {
    return <div className={FormContainerStyle}>
      <form onSubmit={submitFunc}>
        <div className="input-container">
          <label>Team name</label>
          <input className={FormMemberStyle} type="text" name="name" defaultValue={targetTeam.name} required />
        </div>
        <input className={FormSumbitStyle} type="submit" />
      </form>
    </div>;
  }

  const teamDeleteForm = (submitFunc) => {
    return <div className={FormContainerStyle}>
      <form onSubmit={submitFunc}>
          <div className="flex flex-col justify-center items-center">
            <label>Confirm deletion?</label>
            <input className={FormSumbitStyle} type="submit" />
          </div>
        </form>
    </div>;
  }

  const handleTeamUpdate = async (event) => {
    event.preventDefault();
    var { name } = event.target.elements;
    const teamData = {
        name: name.value,
    };
    try {
      const response = await axios.put(APIEndpoint + "/teams/"+targetTeam.id, teamData
      // , {headers: {
      //     Authorization: BearerAuth(accessToken)
      //   }}
      );
    } catch (error) {
      console.log(error);
    }
    loadTeams();
    hideModal();
  };

  const handleTeamDelete = async (event) => {
    event.preventDefault();
    try {
      const response = await axios.delete(APIEndpoint + "/teams/"+targetTeam.id,
      // , {headers: {
      //     Authorization: BearerAuth(accessToken)
      //   }}
      );
    } catch (error) {
      console.log(error);
    }
    loadTeams();
    hideModal();
  };

  return (
    <div className=''>
      <div className='m-4 max-w-md mx-auto px-2 py-5 flex flex-col items-center bg-white border-2 rounded-3xl'>
        <p className='text-2xl pb-5'>Player statistics</p>
        {props.isModerator && props.isModerator == true &&
          <>
            <a href='/players/create' className='bth p-1 mx-auto w-48 text-center border-2 border-b-0  duration-200 bg-slate-100 hover:bg-blue-100'>Create new player</a>
            <a href="https://www.flaticon.com/free-icons/modify" title="modify icons"></a>
          </>
        }
      </div>
      <div className=' m-4 flex flex-row flex-wrap justify-evenly items-start max-w-6xl mx-auto px-2 pb-10 bg-white border-2 rounded-3xl'>
        {teams.map((team) => (
          <div key={team.id} className='flex flex-col w-80'>
            <div className='flex flex-row justify-between pt-10 border-b-2 '>
              <p className='text-xl font-bold pb-1 '>{team.name}</p>
              {props.isModerator && props.isModerator == true && 
                <div className='flex flex-row items-center'>
                    {updateModal ?
                      <Modal show={modalIsShowing} handleClose={() => hideModal()} formContent={teamUpdateForm(handleTeamUpdate)}>
                        <p>Modal</p>
                      </Modal>
                    : <Modal show={modalIsShowing} handleClose={() => hideModal()} formContent={teamDeleteForm(handleTeamDelete)}>
                        <p>Modal</p>
                      </Modal>
                    }
                    <button onClick={() => showModalUpdate(team.id)}><img className='w-7 sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-pencil-24.png'}/></button>
                    <button onClick={() => showModalDelete(team.id)}><img className='w-7 sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-trash-24.png'}/></button>
                </div>
              }
            </div>
              
            <TeamPlayersGet isModerator={props.isModerator} teamId={team.id}/>
          </div>            
        ))} 
      </div>
    </div>
  );
}