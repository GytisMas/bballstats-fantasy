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
  const [player, setPlayer] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const loadPlayer = async () => {
      try {
        const response = (await axios.get(APIEndpoint + '/teams/'+params.teamId+'/players/'+params.playerId));
        setPlayer(response.data);
      } catch (error) {
          console.log(error);
      }
    }
    loadPlayer();
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
    var { name, role, team } = document.forms[0];
    const playerData = {
        name: name.value,
        role: Number(role.value)
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
        // setErrorMessages({ name: "uname", message: "Incorrect user name or password" });
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
      <form onSubmit={handleSubmit}>
        <div className="input-container">
          <label>Full name</label>
          <input className={FormMemberStyle} type="text" name="name" defaultValue={player.name} required />
        </div>
        <div className="input-container">
          <label>Role</label>
          <select className={FormSelectStyle} name="role" defaultValue={player.role}>
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

export default PlayerUpdate;