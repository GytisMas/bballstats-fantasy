import { useAuth } from "../../provider/Authentication";
import {BearerAuth, FormMemberStyle, FormSelectStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle, FormHelperStyle} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function PlayerCreate() {
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [isExistingTeam, setIsExistingTeam] = useState(true);  
  const [teams, setTeams] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const loadTeams = async () => {
        const response = (await axios.get(APIEndpoint + '/teams/'));
        setTeams(response.data);
    }
    loadTeams();
  }, []);

  const errors = {
    uname: "invalid username",
    pass: "invalid password"
  };

  const MakeItem = function(x, i) {
    return <option key={i} value={i}>{x}</option>;
  }

  const handleSubmit = async (event) => {
    event.preventDefault();
    let team = "";
    if (document.forms[0].elements.teamName) {
      const teamResponse = await axios.post(APIEndpoint + "/teams/", {name: document.forms[0].elements.teamName.value}
        , {headers: {
            Authorization: BearerAuth(accessToken)
          }}
        );
      team = teamResponse.data.id;
    }
    var { name, role } = document.forms[0];
    if (team == "")
      team = document.forms[0].elements.team.value;
    const playerData = {
        name: name.value,
        role: Number(role.value)
    };
    try {
        const response = await axios.post(APIEndpoint + "/teams/"+team+'/players/', playerData
        , {headers: {
            Authorization: BearerAuth(accessToken)
          }}
        );
        navigate("/players");
    } catch (error) {
        console.log(error);
        // setErrorMessages({ name: "uname", message: "Incorrect user name or password" });
    }
  };

  // Generate JSX code for error message
  const renderErrorMessage = (name) =>
    name === errorMessages.name && (
      <div className="error">{errorMessages.message}</div>
    );

  function handleTeamClick(newVal) {
    if (newVal == isExistingTeam)
      return;
    setIsExistingTeam(newVal);
  }

  // JSX code for login form
  const renderForm = (
    <div className={FormContainerStyle}>
      <div className="flex flex-row justify-between">
        <button className={FormHelperStyle} onClick={() => handleTeamClick(true)}>Use existing team</button>
        <button className={FormHelperStyle} onClick={() => handleTeamClick(false)}>Create new team</button>
      </div>
       
      <form onSubmit={handleSubmit}>
        <div className="input-container">
          <label>Full name</label>
          <input className={FormMemberStyle} type="text" name="name" required />
        </div>
        {isExistingTeam ? (
          <>
            <div className="input-container">
              <label>Team</label>
              <select className={FormSelectStyle} name="team" defaultValue = {teams[0]}>
                {teams.map((team) => (MakeItem(team.name, team.id)))}
              </select>
            </div>
          </>
        ) : (
          <>
            <div className="input-container">
              <label>Team name</label>
              <input className={FormMemberStyle} type="text" name="teamName" required />
            </div>
          </>
        )}     
        <div className="input-container">
          <label>Role</label>
          <select className={FormSelectStyle} name="role" defaultValue = {roles[0]}>
            {roles.map((role, i) => (MakeItem(role, i)))}
          </select>
        </div>
        <input className={FormSumbitStyle} type="submit" />
      </form>
    </div>
  );

  return (
    <div className="app">
      <div className="login-form">
        {isSubmitted ? <div>Submitted</div> : renderForm}
      </div>
    </div>
  );
}

export default PlayerCreate;