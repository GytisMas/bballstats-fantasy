import { useParams } from "react-router-dom";
import { useAuth } from "../../provider/Authentication";
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import {BearerAuth} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function PlayerStatsDelete(props) {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [player, setPlayer] = useState([]);
  const { setTokens } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    const getPlayer = async () => {
      const response = (await axios.get(APIEndpoint + '/teams/'+params.teamId+'/players/'+params.playerId+'/playerStatistics/'+params.statId));
    }
    getPlayer();
  }, []);

  const handleSubmit = async (event) => {
    event.preventDefault();
    const response = (await axios.delete(APIEndpoint + '/teams/'+params.teamId+'/players/'+params.playerId+'/playerStatistics/'+params.statId
      , {headers: {
        Authorization: BearerAuth(accessToken)
      }}));
    navigate("/players");  
  }

  return (
    <div className="app">
      <div className={FormContainerStyle}>
        <form onSubmit={handleSubmit}>
          <div className="flex flex-col justify-center items-center">
            <label>Confirm deletion?</label>
            <input className={FormSumbitStyle} type="submit" />
          </div>
        </form>
      </div>
    </div>
  );
}

export default PlayerStatsDelete;