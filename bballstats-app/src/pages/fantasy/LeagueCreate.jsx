import { useAuth } from "../../provider/Authentication";
import {BearerAuth, FormMemberStyle, FormSelectStyle, FormSumbitStyleForbid, FormTableStyle, FormWiderContainerStyle, LinkStyle} from '../../components/Helpers';
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
        navigate("/fantasy/leagues");
    } catch (error) {
        console.log(error);
        console.log(error.response.data);
        if (error.response.data.includes("exists"))
          setErrorMessages({ name: "name", message: "League with name already exists." });
        else if (error.response.data.includes("funds"))
          setErrorMessages({ name: "submit", message: "Insuffient funds to create league." });

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
        <div className="bg-white shadow-md rounded px-4 py-4 mb-4">
          <div className="text-xl text-center p-1 ">League Info</div>
          <div className="input-container">
            <label>League name</label>
            {renderErrorMessage("name")}
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
        </div>
        
        <div className="bg-white shadow-md rounded px-4 py-4 mb-4">
          <div className="text-xl text-center p-1 ">League Dates</div>
          <label>Start Date</label>
          <input className={FormMemberStyle} type="date" name="startDate" onChange={handleStartChange} min={defaultStartDate.toISOString().substring(0,10)} max={maxStartDate.toISOString().substring(0,10)} defaultValue={startDate.toISOString().substring(0,10)} required />
          <label>End Date</label>
          <input className={FormMemberStyle} type="date" name="endDate" onChange={handleEndChange} min={minEndDate().toISOString().substring(0,10)} max={maxEndDate().toISOString().substring(0,10)}  defaultValue={endDate.toISOString().substring(0,10)} required />
          <div className="text-xl text-center p-1 ">Teams with upcoming games</div>
          <div className="input-container">
            {isLoading ? 
              <div>Loading teams based on upcoming matches...</div> :
              forbidCreation ? <div>No matches scheduled in selected time period. Select a time period with scheduled matches to create league.</div>
              :
              teams.map((team) => (
                <p key={team.id} className="text-md text-center">{team.name}</p>
              ))
            }
          </div>
        </div>

        <div className="bg-white shadow-md rounded px-4 py-4 mb-4">
          <div className="text-xl text-center p-1 ">Player Winnings</div>
          <div className="text-md text-center pb-1 ">Total prize pool (subtracted from balance): {prizeSum}</div>
          <table className={FormTableStyle}>
            <thead>
              <tr>
                <th className='p-2 border-2 border-black text-left'>Placement</th>
                <th className='p-2 border-2 border-black text-left'>Prize</th>
              </tr>
            </thead>
            <tbody>

            {prizes.map((it, index) => 
            <tr key={index}>
              <td className='p-2 border-2 border-black text-left'>#{index+1}</td>
              <td className='border-2 border-black'><input className={FormMemberStyle} name={"prize-"+index} onChange={handlePrizeChange(index)} step={1} min={0} defaultValue={it} /></td>
            </tr>
            )}
            </tbody>
          </table>
        </div>
        
        
        {renderErrorMessage("funds")}
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