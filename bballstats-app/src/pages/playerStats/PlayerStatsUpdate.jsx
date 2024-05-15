import { useParams } from "react-router-dom";
import { useAuth } from "../../provider/Authentication";
import {ButtonStyle, FormContainerStyle, FormMemberStyle, FormSelectStyle, FormSumbitStyle} from '../../components/Helpers';
import {BearerAuth} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function PlayerStatsUpdate(props) {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [playerStat, setPlayerStat] = useState([]);
  const [selectedStatType, setSelectedStatType] = useState(1);
  const [statistics, setStatistics] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const loadPlayer = async () => {
      try {
        const response = (await axios.get(APIEndpoint + '/statistics/'));        
        // setFormulaMembers(formulaMembers => [...formulaMembers, 's'+(formulaMembers.length+1)]);
        for (let i = 0; i < response.data.length; i++) {
          let m = response.data[i]
          if (m.status != 0 && statistics.find(x => x.id == m.id) == null) {
            setStatistics(statistics => [...statistics, m]);
          }
        }
        const response2 = (await axios.get(APIEndpoint + '/teams/'+params.teamId+'/players/'+params.playerId+'/playerStatistics/'+params.statId));
        console.log(response2.data);
        setPlayerStat(response2.data);
        setSelectedStatType(response2.data.statType)
      } catch (error) {
          console.log(error);
      }
    }
    loadPlayer().then(() => setIsLoading(false));
  }, [isLoading]);

  const errors = {
    uname: "invalid username",
    pass: "invalid password"
  };

  const MakeItem = function(x, i) {
    return <option key={i} value={i}>{x}</option>;
  }

  const handleSubmit = async (event) => {
    event.preventDefault();
    var { statType, value, attemptValue, gameCount } = document.forms[0];
    const playerData = {
        statType: Number(statType.value),
        value: Number(value.value),
        attemptValue: Number(attemptValue.value),
        gameCount: Number(gameCount.value),
    };
    try {
        const response = await axios.put(APIEndpoint + "/teams/"+params.teamId+'/players/'+params.playerId+'/playerStatistics/'+params.statId, playerData
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
      <form onSubmit={handleSubmit} className="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4" >
        <div className="input-container">
          <label>Statistic type</label>
          <select className={FormSelectStyle} name="statType" defaultValue = {selectedStatType}>
            {statistics.map((statistic, i) => (MakeItem(statistic.displayName ?? statistic.name, statistic.id)))}
          </select>
        </div>
        <div className="input-container">
          <label>Total Value</label>
          <input className={FormMemberStyle} type="number" step="0.01" name="value" defaultValue={playerStat.value} required />
        </div>
        <div className="input-container">
          <label>Total Attempt Value</label>
          <input className={FormMemberStyle} type="number" step="0.01" name="attemptValue" defaultValue={playerStat.attemptValue} required />
        </div>
        <div className="input-container">
          <label>Game Count</label>
          <input className={FormMemberStyle} type="number" step="1" name="gameCount" defaultValue={playerStat.gameCount} required />
        </div>
        <input className={ButtonStyle} type="submit" />
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

export default PlayerStatsUpdate;