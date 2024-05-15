import { useNavigate } from "react-router-dom";
import { useEffect } from 'react';
import { useAuth } from "../../provider/Authentication";
import axios from "axios";
import { APIEndpoint, BearerAuth } from "../../components/Helpers";

const Logout = () => {
  const { setTokens, accessToken } = useAuth();
  const navigate = useNavigate();

  

  useEffect(() => {
    const handleLogout = async () => {
      if (accessToken != null) {
        const response = await axios.post(APIEndpoint + "/logout", []
        , {headers: {
          Authorization: BearerAuth(accessToken)
        }});
      }
      setTokens();
    };

    handleLogout().then(() => navigate("/login"));
  })

  return <>Logout Page</>;
};

export default Logout;