import { useAuth } from "../../provider/Authentication";
import {BearerAuth, Form2XLContainerStyle, FormMaxWidthContainerStyle, FormMemberStyle, FormWiderContainerStyle, RoleMemberStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle, FormHelperStyle} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function FantasyTemplateCreate() {
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [isExistingTeam, setIsExistingTeam] = useState(true);    
  const [statTypes, setStatTypes] = useState([]);
  const [leagueRoles, setLeagueRoles] = useState([]);
  const [leagueRoleStats, setLeagueRoleStats] = useState([]);
  const [teams, setTeams] = useState([]);
  const [selectedRoleIndex, setSelectedRoleIndex] = useState(-1);
  const navigate = useNavigate();
  const [startFiveMode, setStartFiveMode] = useState(false);    

  useEffect(() => {
    const loadStatTypes = async () => {
        const response = (await axios.get(APIEndpoint + '/Statistics'));
        setStatTypes(response.data);
    }
    loadStatTypes();
  }, []);

  const errors = {
    templateName: "Invalid league template name",
    roles: "Problem with roles"
  };

  const roleTableState = (roleIndex) => {
    return selectedRoleIndex == roleIndex ? "visible" : "hidden";
  }

  const MakeItem = function(x, i, ri) {
    const name = x.displayname != null && x.displayname != "" 
      ? x.displayname : x.name;
    return (
      <tr key={i}>
        <td className='p-2 border-2 text-left'>{name}</td>
        <td className='p-2 border-2 text-left'>
          <input type="checkbox" name={"useStat-"+ri+"-"+i} defaultChecked={x.defaultIsChecked}/>
        </td>
        <td>
          <input className={FormMemberStyle} type="number" defaultValue={x.defaultLeaguePointsPerStat} step="0.01" name={"statvalue-"+ri+"-"+i} required />
        </td>
      </tr>
    );
  }

  const handleStartFive = (event) => {
    // event.preventDefault();
    setStartFiveMode(event.target.checked)
  }

  const handleSubmit = async (event) => {
    event.preventDefault();
    setErrorMessages({ name: "roles", message: "" });
    setErrorMessages({ name: "templateName", message: "" });
    if (startFiveMode && (leagueRoles.length != 10)) {
      setErrorMessages({ name: "roles", message: "10 roles needed (start five and bench) when using start five rosters" });
      return;
    }
    else if (leagueRoles.length < 5 || leagueRoles.length > 10) {
      setErrorMessages({ name: "roles", message: "5-10 roles required" });
      return;
    }


    const formData = { 
      name: document.forms[0].elements["templateName"].value, 
      teamWinPoints: Number(document.forms[0].elements["teamWinPts"].value),
      teamLosePoints: Number(document.forms[0].elements["teamLosePts"].value),
      leagueRoles: []
    }
    if (document.forms[0].elements["benchMultiplier"] != null) {
      formData.benchMultiplier = document.forms[0].elements["benchMultiplier"].value
    }

    for (var i = 0; i < leagueRoles.length; i++) {
      let roleName = document.forms[0].elements["rolename"+i].value
      let roleLeagueStats = []

      let roleStats = document.querySelectorAll('[name^=useStat-'+i+']')
      for (var ii = 0; ii < roleStats.length; ii++) {
        if (!roleStats[ii].checked)
          continue;
        const roleStatArr = roleStats[ii].name.split("-")
        const statId = roleStatArr[roleStatArr.length - 1]
        roleLeagueStats.push(
          { 
            pointsPerStat: document.forms[0].elements["statvalue-"+i+"-"+statId].value,
            statisticId: statId
          })
      }

      formData.leagueRoles.push(
        {
          name: roleName,
          leagueStats: roleLeagueStats,
          roleToReplaceIndex: startFiveMode ? (i+1) % 2 == 0 ? i-1 : null : null
        }
      )
    }
    console.log("-")
    console.log(formData)
    try {
        const response = await axios.post(APIEndpoint + "/leagueTemplates/", formData
        , {headers: {
          Authorization: BearerAuth(accessToken)
        }}
        );
        navigate("/players");
    } catch (error) {
        console.log(error);
        console.log(error.response.data);
        if (error.response.data.includes("exists"))
          setErrorMessages({ name: "templateName", message: "League template with name already exists" });
    }
  };

  // Generate JSX code for error message
  const renderErrorMessage = (name) =>
    name === errorMessages.name && (
      <div className="error text-red-500">{errorMessages.message}</div>
    );

  function handleTeamClick(newVal) {
    if (newVal == isExistingTeam)
      return;
    setIsExistingTeam(newVal);
  }

  const handleAdd = (event) => {
    event.preventDefault();
    setLeagueRoles(leagueRoles => [...leagueRoles, "role"+leagueRoles.length]);
    setLeagueRoleStats(leagueRoleStats => [...leagueRoleStats, []]);
  }

  const handleRemove = (event) => {
    event.preventDefault();    
    setLeagueRoles((leagueRoles) => (leagueRoles.slice(0, -1)));
    setLeagueRoleStats((leagueRoleStats) => (leagueRoleStats.slice(0, -1)));
  }

  // JSX code for login form
  const renderForm = (
    <div className={FormMaxWidthContainerStyle}>
      <form onSubmit={handleSubmit}>
        <div className="input-container max-w-sm mx-auto">
          <label>Template name</label>
          <input className={FormMemberStyle} type="text" name="templateName" required />
          {renderErrorMessage("templateName")}
          <label>Use active / start five rosters</label><br/>
          <input type="checkbox" name="startFiveMode" onChange={handleStartFive} /><br/>
          {startFiveMode && 
          <>
            <label>Bench player points, 1-100% (active players are 100%)</label><br/>
            <input type="number" min={0} max={100} step={1} defaultValue={30} name="benchMultiplier" /> %
          </>
          }
          <label>Player gained points per game for team win / loss</label><br/>
          <input type="number" step={1} defaultValue={3} name="teamWinPts" />
          <input type="number" step={1} defaultValue={-3} name="teamLosePts" />
          

        </div>
        <div className="input-container justify-center flex flex-col items-center">
          <label>League Player Roles</label>
          {startFiveMode &&
            <label className="max-w-lg text-center">Note: when using Start Five Mode, every odd role (1st, 3rd, etc.) is considered active, and every even role (2nd, 4th, etc.) is considered the previous roles' direct and only replacement.</label>
          }
          <div className="flex flex-row flex-wrap justify-center">
          {leagueRoles.map((role, roleIndex) => (
            <div key={role} className={RoleMemberStyle} >
              <label>Role name</label>
              <input className={FormMemberStyle} type="text" name={"rolename"+roleIndex} defaultValue={role} required />
                <table className={FormMemberStyle}>
                  <thead>
                    <tr>
                      <th className='p-2 border-2 text-left'>Statistic name</th>
                      <th className='p-2 border-2 text-left'>Use statistic?</th>
                      <th className='p-2 border-2 text-left'>League points per statistic point</th>
                    </tr>
                  </thead>
                  <tbody>
                    {statTypes.map((stat) => (MakeItem(stat, stat.id, roleIndex)))}
                  </tbody>
                </table>
            </div>          
          ))}
          </div>
        </div>
      <div className="flex flex-row justify-between">
        <button className={FormHelperStyle} onClick={handleAdd}>Add player role</button>
        <button className={FormHelperStyle} onClick={handleRemove}>Remove Last Role</button>
      </div>
        {/* {isExistingTeam ? (
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
        </div> */}
        {renderErrorMessage("roles")}
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

export default FantasyTemplateCreate;