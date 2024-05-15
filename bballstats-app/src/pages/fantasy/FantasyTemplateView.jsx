import { useAuth } from "../../provider/Authentication";
import {BearerAuth, DisabledFormMemberStyle, Form2XLContainerStyle, FormMaxWidthContainerStyle, FormMemberHalfStyle, FormWiderContainerStyle, RoleMemberStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle, FormHelperStyle} from '../../components/Helpers';
import { useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function FantasyTemplateView() {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [isExistingTeam, setIsExistingTeam] = useState(true);    
  const [template, setTemplate] = useState([]);
  const [statTypes, setStatTypes] = useState([]);
  const [leagueRoles, setLeagueRoles] = useState([]);
  const [leagueRoleStats, setLeagueRoleStats] = useState([]);
  const [teams, setTeams] = useState([]);
  const [selectedRoleIndex, setSelectedRoleIndex] = useState(-1);

  const navigate = useNavigate();
  const [startFiveMode, setStartFiveMode] = useState(false);    
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadStatTypes = async () => {
      const statsResponse = (await axios.get(APIEndpoint + '/Statistics'));
      setStatTypes(statsResponse.data);
      const templateResponse = (await axios.get(APIEndpoint + '/LeagueTemplates/' + params.templateId));
      setTemplate(templateResponse.data);
      setLeagueRoles(templateResponse.data.leagueRoles
        .sort((a, b) => a.id > b.id ? 1 : -1));
    }
    loadStatTypes().then(() => setIsLoading(false));
  }, []);

  const errors = {
    templateName: "Invalid league template name",
    roles: "Problem with roles"
  };

  const roleTableState = (roleIndex) => {
    return selectedRoleIndex == roleIndex ? "visible" : "hidden";
  }

  const MakeItem = function(x, i, role) {
    const name = x.displayName != null && x.displayName != "" 
      ? x.displayName : x.name;
    const statEntry = role.leagueStats.find(x => x.statisticId == i);
    if (statEntry == null) {
      return null;
    }
    console.log(x);
    console.log(name);
    return (
      <tr key={i}>
        <td className='p-2 border-2 text-left'>{name}</td>
        <td className='p-2 border-2'>
          <input className={DisabledFormMemberStyle} type="number" defaultValue={statEntry.pointsPerStat} disabled step="0.01" name={"statvalue-"+role.id+"-"+i} required />
        </td>
      </tr>
    );
  }

  // Generate JSX code for error message
  const renderErrorMessage = (name) =>
    name === errorMessages.name && (
      <div className="error text-red-500">{errorMessages.message}</div>
    );

  // JSX code for login form
  const renderForm = (
    <div className={FormMaxWidthContainerStyle}>
      <div className="input-container max-w-sm mx-auto">
        <div className="input-container max-w-sm mx-auto bg-white shadow-md rounded px-4 py-4 mb-4">
          <label>Template name</label>
          <input className={DisabledFormMemberStyle} type="text" name="templateName" value={template.name} required disabled />
          <p className="text-center py-1">Player gained points per game for team win / loss</p>
          <input className={FormMemberHalfStyle} type="number" step={1} name="teamWinPts" disabled value={template.teamWinPoints}/>
          <input className={FormMemberHalfStyle} type="number" step={1} name="teamLosePts" disabled value={template.teamLosePoints}/>
        </div>
        <div className="max-w-sm mx-auto bg-white shadow-md rounded px-4 py-4 mb-4">
          <label>Use active / start five rosters</label><br/>
          <input type="checkbox" name="startFiveMode" checked={template.benchMultiplier != null} disabled /><br/>
          {template.benchMultiplier != null && 
          <>
            <label>Bench player points, 0-100% (active players are 100%)</label><br/>
            <input type="number" min={0} max={100} step={1} disabled value={template.benchMultiplier * 100} name="benchMultiplier" /> %
          </>
        }
      </div>
        
        

      </div>
      <div className="input-container justify-center flex flex-col items-center">
        <label>League Player Roles</label>
        {startFiveMode &&
          <label className="max-w-lg text-center">Note: when using Start Five Mode, every odd role (1st, 3rd, etc.) is considered active, and every even role (2nd, 4th, etc.) is considered the previous roles' direct and only replacement.</label>
        }
        <div className="flex flex-row flex-wrap justify-center">
        {leagueRoles.map((role, roleIndex) => (
          <div key={role.id} className={RoleMemberStyle} >
            <label>Role name</label>
            <input className={DisabledFormMemberStyle} type="text" name={"rolename"+roleIndex} disabled defaultValue={role.name} required />
            {role.roleToReplaceIndex != null && 
              <>
                <p className="py-1">Is replacement for role</p>
                <input className={DisabledFormMemberStyle} type="text" name={"rolename"+roleIndex} disabled defaultValue={leagueRoles.find(x => x.id == role.roleToReplaceIndex).name} required />
              </>
            }
              <table className={DisabledFormMemberStyle}>
                <thead>
                  <tr>
                    <th className='p-2 border-2 text-left'>Statistic name</th>
                    <th className='p-2 border-2 text-left'>League points per statistic point</th>
                  </tr>
                </thead>
                <tbody>
                  {statTypes.map((stat) => (MakeItem(stat, stat.id, role)))}
                </tbody>
              </table>
          </div>          
        ))}
        </div>
      </div>
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

export default FantasyTemplateView;