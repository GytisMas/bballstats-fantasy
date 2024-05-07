import { useParams } from "react-router-dom";
import { useAuth } from "../../provider/Authentication";
import {BearerAuth, UserRoles} from '../../components/Helpers';
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function Register() {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const navigate = useNavigate();

  const handleSubmit = async (event) => {
    event.preventDefault();
    var { username, pass, email, roles } = document.forms[0];
    const userData = {
      username: username.value,
      password: pass.value,
      email: email.value,
      role: 8
    };

    try {
      const response = await axios.post(APIEndpoint + "/register/", userData);
      navigate("/login");
    } catch (error) {
      let errors = "";
      for (let err of Array.from(error.response.data.map(x => x.description))) {
        errors += err + "\n"
      }
      setErrorMessages({ name: "password", message: errors });
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
      <form className="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4" onSubmit={handleSubmit}>
        <div className="text-xl text-center p-1 ">Register</div>
        <div className="input-container">
          <label>User name</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="text" name="username" required />
        </div>
        <div className="input-container">
          <label>Password</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="password" name="pass" required />
          {renderErrorMessage("password")}
        </div>
        <div className="input-container">
          <label>Email</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="email" name="email" required />
        </div>
        <div>
          <input className="bg-blue-500 hover:bg-blue-700 mt-2 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline" type="submit" />
        </div>
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

export default Register;