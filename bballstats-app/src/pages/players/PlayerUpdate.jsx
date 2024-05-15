import { useParams } from "react-router-dom";
import { useAuth } from "../../provider/Authentication";
import {BearerAuth, FormSelectStyle} from '../../components/Helpers';
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function PlayerUpdate(props) {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [isLoading, setIsLoading] = useState(true);  
  const [teams, setTeams] = useState([]);
  const [player, setPlayer] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const loadTeams = async () => {
      const response = (await axios.get(APIEndpoint + '/teams/'));
      setTeams(response.data);
    }
    const loadPlayer = async () => {
      try {
        const response = (await axios.get(APIEndpoint + '/teams/'+params.teamId+'/players/'+params.playerId));
        setPlayer(response.data);
      } catch (error) {
          console.log(error);
      }
    }
    loadPlayer();
    loadTeams().then(() => setIsLoading(false));
}, []);

  const errors = {
    uname: "invalid username",
    pass: "invalid password"
  };

  const MakeItem = function(x, i) {
    return <option key={i} value={i}>{x}</option>;
  }

  const MakeTeamItem = function(team, i) {
    return <option key={i} value={team.id}>{team.name}</option>;
  }

  const handleSubmit = async (event) => {
    event.preventDefault();
    var { name, role, team, forbidAutoUpdate } = document.forms[0];
    const playerData = {
        name: name.value,
        role: Number(role.value),
        teamId: team.value,
        forbidAutoUpdate: forbidAutoUpdate.checked,
    };
    try {
        const response = await axios.put(APIEndpoint + "/teams/"+params.teamId+'/players/'+params.playerId, playerData
        , {headers: {
            Authorization: BearerAuth(accessToken)
          }}
        );
        navigate("/players");
    } catch (error) {
        console.log(error);
    }
  };

  // Generate JSX code for error message
  const renderErrorMessage = (name) =>
    name === errorMessages.name && (
      <div className="error">{errorMessages.message}</div>
    );

  // JSX code for login form
  const renderForm = (
    <div className={FormContainerStyle}>
      <form onSubmit={handleSubmit} className=" bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4">
        <div className="input-container">
          <label>Full name</label>
          <input className={FormMemberStyle} type="text" name="name" defaultValue={player.name} required />
        </div>
        <div className="input-container mt-2">
          <label>Role</label>
          <select className={FormSelectStyle} name="role" defaultValue={player.role}>
            {roles.map((role, i) => (MakeItem(role, i)))}
          </select>
        </div>
        <div className="input-container mt-2">
          <label>Team</label>
          <select className={FormSelectStyle} name="team" defaultValue={player.teamId}>
            {teams.map((team, i) => (MakeTeamItem(team, i)))}
          </select>
        </div>
        <div className="input-container mt-2">
          <label>Forbid automatic info updating</label><br/>
          <input type="checkbox" name="forbidAutoUpdate" defaultChecked={player.forbidAutoUpdate} />
        </div>
        <input className={FormSumbitStyle} type="submit" />
      </form>
    </div>
  );

  return (
    <div className="app">
      <div className="login-form">
        {isLoading ? <div>Loading...</div> : renderForm}
      </div>
    </div>
  );
}

export default PlayerUpdate;