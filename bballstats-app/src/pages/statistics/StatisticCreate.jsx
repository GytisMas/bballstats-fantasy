import { useAuth } from "../../provider/Authentication";
import {BearerAuth, FormMemberStyle, FormSelectStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function StatisticCreate() {
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const navigate = useNavigate();

  const errors = {
    uname: "invalid username",
    pass: "invalid password"
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    var { dname, sname, visibility, leaguePoints, leagueChecked } = document.forms[0];
    const statData = {
        displayName: dname.value != "" ? dname.value : null,
        name: sname.value,
        status: Number(visibility.value),
        defaultLeaguePointsPerStat: Number(leaguePoints.value),
        defaultIsChecked: leagueChecked.checked
    };

    try {
        const response = await axios.post(APIEndpoint + "/statistics/", statData
        , {headers: {
            Authorization: BearerAuth(accessToken)
          }}
        );
        navigate("/stats");
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
       
      <form onSubmit={handleSubmit}>
        <div className="input-container">
          <label>Display name</label>
          <input className={FormMemberStyle} type="text" name="dname" required />
        </div>
        <div className="input-container">
          <label>Short name</label>
          <input className={FormMemberStyle} type="text" name="sname" required />
        </div>
        <div className="input-container">
          <label>Visibility status</label>
            <select className={FormSelectStyle} name='visibility'>
              <option key={1} value={1}>Main</option>
              <option key={0} value={0}>Regular</option>
            </select>
        </div>
        <div className="input-container">
          <label>Default FL Points</label>
          <input className={FormMemberStyle} type="number" step={0.001} name="leaguePoints" defaultValue={1} required />
        </div>
        <div className="input-container">
          <label>Used in FL by default</label>
          <br/><input type="checkbox" name="leagueChecked" defaultChecked={false} />
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

export default StatisticCreate;