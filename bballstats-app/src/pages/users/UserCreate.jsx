import { useParams } from "react-router-dom";
import { useAuth } from "../../provider/Authentication";
import {BearerAuth} from '../../components/Helpers';
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function UserCreate() {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [userRoles, setUserRoles] = useState([]);
  const navigate = useNavigate();

  const MakeItem = function(x, i) {
    return <option key={i} value={i}>{x}</option>;
  }

  const handleSubmit = async (event) => {
    event.preventDefault();
    var { username, pass, email, roles } = document.forms[0];

    const userData = {
      username: username.value,
      password: pass.value,
      email: email.value,
      role: Number(roles.value)
    };
    try {
        const response = await axios.post(APIEndpoint + "/users/", userData
        , {headers: {
            Authorization: BearerAuth(accessToken)
          }}
        );
        navigate("/users");
    } catch (error) {
        // console.log(error);
        setErrorMessages({ name: "any", message: error.response.data });
    }
  };

  // Generate JSX code for error message
  const renderErrorMessage = (name) =>
    name === errorMessages.name && (
      <div className="error text-red-500">{errorMessages.message}</div>
    );

  // JSX code for login form
  const renderForm = (
    <div className={FormContainerStyle}>
      <form onSubmit={handleSubmit} className="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4">
        <div className="input-container">
          <label>User name</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="text" name="username" required />
        </div>
        <div className="input-container">
          <label>Password</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="password" name="pass" required />
        </div>
        <div className="input-container">
          <label>Email</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="email" name="email" required />
        </div>
        <div className="input-container">
          <label>Roles (1-15)</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="number" min='1' max='15' name="roles" defaultValue={1} required />
        </div>
        <div>
          <input className="bg-blue-500 mt-2 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline" type="submit" />
        </div>
        {renderErrorMessage("any")}
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

export default UserCreate;