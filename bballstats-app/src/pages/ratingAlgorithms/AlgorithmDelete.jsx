import { useParams } from "react-router-dom";
import { useAuth } from "../../provider/Authentication";
import {ButtonStyle, FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import {BearerAuth} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function AlgorithmDelete(props) {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [player, setPlayer] = useState([]);
  const { setTokens } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    const getPlayer = async () => {
        const response = (await axios.get(APIEndpoint + '/users/'+params.userId+'/customStatistics/'+params.algoId));
      }
    getPlayer();
  }, []);

  const handleSubmit = async (event) => {
    event.preventDefault();
    const response = (await axios.delete(APIEndpoint + '/users/'+params.userId+'/customStatistics/'+params.algoId
      , {headers: {
        Authorization: BearerAuth(accessToken)
      }}));
    navigate("/profile");  
  }

  return (
    <div className="app">
      <div className={FormContainerStyle}>
        <form onSubmit={handleSubmit}>
          <div className="flex flex-col justify-center items-center">
            <label>Confirm deletion?</label>
            <input className={ButtonStyle} type="submit" />
          </div>
        </form>
      </div>
    </div>
  );
}

export default AlgorithmDelete;