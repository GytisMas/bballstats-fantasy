import { useParams } from "react-router-dom";
import { useAuth } from "../../provider/Authentication";
import {BearerAuth, ButtonStyle, UserRoles} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';

function UserUpdate() {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [user, setUser] = useState([]);
  const [userRoles, setUserRoles] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const loadUser = async () => {
      try {
        const response = (await axios.get(APIEndpoint + '/users/'+params.userId));
        setUser(response.data);
        let roleNum = 1;
        let rolesToAdd = 0;
        for (let i = 0; i < UserRoles.length; i++) {
          if (response.data.roles.includes(UserRoles[i])) {
            rolesToAdd += roleNum;
          }
          roleNum = roleNum * 2;
        }
        setUserRoles(rolesToAdd);
      } catch (error) {
          console.log(error);
      }
    }
    loadUser();
  }, []);

  const MakeItem = function(x, i) {
    return <option key={i} value={i}>{x}</option>;
  }

  const handleSubmit = async (event) => {
    event.preventDefault();
    setErrorMessages({ name: "password", message: "" });
    var { oldpass, newpass, email, roles } = document.forms[0];
    let unfilledPasswords = oldpass.value == "" ? 1 : 0;
    unfilledPasswords = newpass.value == "" ? unfilledPasswords + 1 : unfilledPasswords;

    if (unfilledPasswords == 1) {
        setErrorMessages({ name: "password", message: "Before submitting, fill either both password fields or skip them." });
      return;
    }
    setErrorMessages({ name: "password", message: "" });

    const userData = {
      oldPassword: oldpass.value,
      newPassword: newpass.value,
      email: email.value,
      role: Number(roles.value)
    };
    console.log(userData)

    try {
        const response = await axios.put(APIEndpoint + "/users/" + params.userId, userData
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
          <label>Username</label>
          <input disabled className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="text" name="username" defaultValue={user.username} />
        </div>
        <div className="input-container">
          <label>Old Password</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="password" name="oldpass" />
        </div>
        <div className="input-container">
          <label>New Password</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="password" name="newpass" />
        </div>
        <div className="input-container">
          <label>Email</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="email" name="email" defaultValue={user.email} required />
        </div>
        <div className="input-container">
          <label>Roles (1-11)</label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="number" min='1' max='15' name="roles" defaultValue={userRoles} required />
        </div>
        <div >
          <input className={ButtonStyle} type="submit" />
          {renderErrorMessage("password")}
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

export default UserUpdate;