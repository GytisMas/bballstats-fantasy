import { useParams } from "react-router-dom";
import { useAuth } from "../../provider/Authentication";
import {BearerAuth, FormMemberStyle, FormSelectStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function PlayerStatsCreate() {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [isExistingTeam, setIsExistingTeam] = useState(true);  
  const [statistics, setStatistics] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const loadStatistics = async () => {
        const response = (await axios.get(APIEndpoint + '/statistics/'));
        
        // setFormulaMembers(formulaMembers => [...formulaMembers, 's'+(formulaMembers.length+1)]);
        for (let i = 0; i < response.data.length; i++) {
          let m = response.data[i]
          if (m.status != 0 && !statistics.includes(m)) {
            setStatistics(statistics => [...statistics, m]);
          }
        }
    }
    loadStatistics();
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
    var { statType, value } = document.forms[0];
    const playerData = {
        statisticId: Number(statType.value),
        value: Number(value.value)
    };
    try {
        const response = await axios.post(APIEndpoint + "/teams/"+params.teamId+'/players/'+params.playerId+'/playerStatistics', playerData
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
      <form onSubmit={handleSubmit}>
        <div className="input-container">
          <label>Statistic type</label>
          <select className={FormSelectStyle} name="statType" defaultValue = {statistics[0]}>
            {statistics.map((statistic, i) => (MakeItem(statistic.displayName, statistic.id)))}
          </select>
        </div>
        <div className="input-container">
          <label>Value</label>
          <input className={FormMemberStyle} type="number" step="0.01" name="value" required />
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

export default PlayerStatsCreate;