import { useAuth } from "../../provider/Authentication";
import {BearerAuth, ButtonStyle, UserRoles} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import { useState, useEffect } from 'react';
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function ChangePassword() {
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const navigate = useNavigate();

  const handleSubmit = async (event) => {
    event.preventDefault();
    var { oldpass, newpass } = document.forms[0];

    const userData = {
        oldpassword: oldpass.value,
        newpassword: newpass.value
    };
    try {
        const response = await axios.post(APIEndpoint + "/changePassword/", userData
        , {headers: {
            Authorization: BearerAuth(accessToken)
          }}
        );
        navigate("/profile");
    } catch (error) {
        setErrorMessages({ name: "any", message: error.response.data });
    }
  };

  // Generate JSX code for error message
  const renderErrorMessage = (name) =>
    name === errorMessages.name && (
      <div className="error text-red-500">{errorMessages.message}</div>
    );

  const renderForm = (
    <div className={FormContainerStyle}>
      <form onSubmit={handleSubmit} className="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4" >
        <div className="text-xl text-center p-1 ">Change Password</div>
        <div className="input-container mt-2">
          <label>Old Password</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="password" name="oldpass" required />
        </div>
        <div className="input-container">
          <label>New Password</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="password" name="newpass" required />
        </div>
        <div>
          <input className={ButtonStyle} type="submit" />
          {renderErrorMessage("any")}
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

export default ChangePassword;