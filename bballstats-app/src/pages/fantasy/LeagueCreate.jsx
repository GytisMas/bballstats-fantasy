import { useAuth } from "../../provider/Authentication";
import {BearerAuth, FormMemberStyle, FormSelectStyle, FormSumbitStyleForbid, FormWiderContainerStyle, LinkStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle, FormHelperStyle} from '../../components/Helpers';
import { Link, useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";
import { jwtDecode } from "jwt-decode";

function LeagueCreate() {
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [forbidCreation, setForbidCreation] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isTemplLoading, setIsTemplLoading] = useState(true);
  const [teams, setTeams] = useState([]);
  const [templates, setTemplates] = useState([]);
  const defaultStartDate = new Date(new Date().setDate(new Date().getDate() + 1));
  const maxStartDate = new Date(new Date().setDate(new Date().getDate() + 30));
  const [startDate, setStartDate] = useState(defaultStartDate);
  const [endDate, setEndDate] = useState(new Date(new Date().setDate(startDate.getDate() + 7)));

  const [prizes, setPrizes] = useState(Array.from(Array(10).keys()).map(n => n+1).reverse())
  const [prizeSum, setPrizeSum] = useState(prizes.reduce((a,b)=>a+b))
  const [templateId, setTemplateId] = useState(-1);
  const navigate = useNavigate();
  const minEndDate = () => {
    return new Date(new Date().setDate(startDate.getDate() + 1))
  }
  const maxEndDate = () => {
    return new Date(new Date().setDate(startDate.getDate() + 30))
  }

  useEffect(() => {
    const loadAvailableTeams = async () => {
      axios.get(APIEndpoint + '/Teams/ByDates' 
        + '?startDate=' + startDate.toISOString().substring(0,10) 
        + '&endDate=' + endDate.toISOString().substring(0,10))
      .then((response) => {
        setTeams(response.data);
        setForbidCreation(false);
      })
      .catch(error => {
        setForbidCreation(true)
        setTeams([]);
      });
    }
    loadAvailableTeams().then(() => setIsLoading(false));
  }, [isLoading]);

  useEffect(() => {
    const loadTemplates = async () => {
      const response = (await axios.get(APIEndpoint + '/LeagueTemplates'));
      setTemplateId(response.data[0].id);
      setTemplates(response.data);
    }

    loadTemplates();
    setIsTemplLoading(false);
  }, []);

  const errors = {
    templateName: "Invalid league template name",
    roles: "Problem with roles"
  };

  const MakeOptionItem = function(x, i) {
    return <option key={i} value={i}>{x}</option>;
  }

  const handleSubmit = async (event) => {
    event.preventDefault();
    // TODO: validation (league name negali sutapti)
    // console.log("--")
    const formData = { 
      name: document.forms[0].elements["leagueName"].value,
      entryFee: document.forms[0].elements["entryFee"].value,
      startDate: document.forms[0].elements["startDate"].value,
      endDate: document.forms[0].elements["endDate"].value,
      leagueTemplateId: Number(document.forms[0].elements["template"].value), 
      AvailablePlayerTeamIds: teams.map(t => t.id),
      leagueHostId: jwtDecode(accessToken).sub,
      LeaguePayments:
        Array.from(
          document.querySelectorAll('[name^=prize-')
          ).map(p => (Number)(p.value))
    };
    // console.log(formData)
    // console.log("-")

    try {
        const response = await axios.post(APIEndpoint + "/fantasy/leagues", formData
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

  function handleStartChange(event) {
    setStartDate(new Date(event.target.value));
    setIsLoading(true)
  } 

  function handleEndChange(event) {
    setEndDate(new Date(event.target.value));
    setIsLoading(true)
  } 

  const handlePrizeChange = numId => (event) => {
    console.log(prizes)
    let newPrizes = prizes;
    newPrizes[numId] = (Number)(event.target.value);
    setPrizes(newPrizes);
    setPrizeSum(prizes.reduce((a,b)=>a+b));
  }

  // JSX code for login form
  const renderForm = (
    <div className={FormWiderContainerStyle}>
      
       
      <form onSubmit={handleSubmit}>
        <div className="input-container">
          <label>League name</label>
          <input className={FormMemberStyle} type="text" name="leagueName" required />
          <label>Entry fee</label>
          <input className={FormMemberStyle} type="number" min={1} defaultValue={1} step="1" name={"entryFee"} required />

          {renderErrorMessage("leagueName")}
        </div>
        {isTemplLoading ? <div>Loading templates...</div> :
          <div className="input-container">
            <label>Used League Template</label>
            <select className={FormSelectStyle} name="template" onChange={(event) => setTemplateId(event.target.value)}>
              {templates.map((template) => (MakeOptionItem(template.name, template.id)))}
            </select>
            {document.forms[0].elements["template"] != null && 
              <Link to={'/fantasy/templates/'+templateId} target="_blank" rel="noopener noreferrer" ><button type="button" className={LinkStyle}>View Template</button></Link>
            }
          </div>
        }
        <div className="input-container">
          <label>Start Date</label>
          <input className={FormMemberStyle} type="date" name="startDate" onChange={handleStartChange} min={startDate.toISOString().substring(0,10)} max={maxStartDate.toISOString().substring(0,10)} defaultValue={startDate.toISOString().substring(0,10)} required />
          <label>End Date</label>
          <input className={FormMemberStyle} type="date" name="endDate" onChange={handleEndChange} min={minEndDate().toISOString().substring(0,10)} max={maxEndDate().toISOString().substring(0,10)}  defaultValue={endDate.toISOString().substring(0,10)} required />
        </div>

        <div className="input-container">
          <label>Player Winnings (total prize pool: {prizeSum})</label><br/>
            {prizes.map((it, index) => 
            <div key={index}>
              <label>#{index+1}  </label>
              <input name={"prize-"+index} onChange={handlePrizeChange(index)} step={1} min={0} defaultValue={it} /><br/>
            </div>
            )}
        </div>
        
        <div className="input-container">
          <label>League Available Teams</label>
          {isLoading ? 
            <div>Loading...</div> :
            forbidCreation ? <div>No matches scheduled in selected time period. Select a time period with scheduled matches to create league.</div>
            :
            teams.map((team) => (
              <div key={team.id} className="flex flex-row" >
                <label>{team.id}</label>
                <label>{team.name}</label>
              </div>          
            ))
          }
        </div>
        {renderErrorMessage("roles")}
        <input className={!forbidCreation ? FormSumbitStyle : FormSumbitStyleForbid} type="submit" disabled={forbidCreation} />
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

export default LeagueCreate;