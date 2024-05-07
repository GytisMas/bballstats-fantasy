import { useAuth } from "../../provider/Authentication";
import {BearerAuth, FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import { useNavigate, useSearchParams } from "react-router-dom";
import { useState } from 'react';
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";
function Login() {
  const [params, setParams] = useSearchParams();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const { setTokens } = useAuth();
  const navigate = useNavigate();

  const errors = {
    uname: "invalid username",
    pass: "invalid password"
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setErrorMessages({ name: "all", message: "" });

    var { uname, pass } = document.forms[0];
    const userData = {
        userName: uname.value,
        password: pass.value
    };
    try {
        const response = await axios.post(APIEndpoint + "/login", userData);
        setTokens(response.data.accessToken, response.data.refreshToken);
        setIsSubmitted(true)
        let nextPage = '/profile';
        // let nextPage = params.get('redirected') ? -1 : '/profile';
        navigate(nextPage);
    } catch (error) {
        console.log(error);
        const errorMsg = error.response && error.response.data.includes("incorrect") 
          ? "Incorrect user name or password" 
          : "Server error. Try again later."
        setErrorMessages({ name: "all", message: errorMsg });
    }
  };

  // Generate JSX code for error message
  const renderErrorMessage = (name) =>
    name === errorMessages.name && (
      <div className="error">{errorMessages.message}</div>
    );

  // JSX code for login form
  const renderForm = (
    <div>
      <form className="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4" onSubmit={handleSubmit}>
        <div className="text-xl text-center p-1 ">Login</div>
        <div className="input-container">
          <label>Username </label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="text" name="uname" required />
          {renderErrorMessage("uname")}
        </div>
        <div className="input-container">
          <label>Password </label>
          <input className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" type="password" name="pass" required />
          {renderErrorMessage("pass")}
        </div>
        <div>
          <input className="bg-blue-500 hover:bg-blue-700 mt-2 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline" type="submit" value="Sign In" />
          {renderErrorMessage("all")}
        </div>
      </form>
    </div>
  );

  return (
    <div>
      <div className="max-w-xs mx-auto pt-10">
        {params.get('redirected') && <>
          <p>To access this page, you have to login.</p>
          <button className="bg-slate-500 hover:bg-slate-700 mt-2 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
           onClick={() => navigate(-2)}>Go Back</button>
        </>
        }
        {isSubmitted ? <div>User is successfully logged in</div> : renderForm}
      </div>
    </div>
  );
}

export default Login;