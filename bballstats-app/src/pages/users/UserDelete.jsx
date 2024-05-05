import { useParams } from "react-router-dom";
import { useAuth } from "../../provider/Authentication";
import {BearerAuth} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import { useState, useEffect } from 'react';
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";

function UserDelete(props) {
  let params = useParams();
  const { accessToken } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    const getUser = async () => {
        const response = (await axios.get(APIEndpoint + '/users/'+params.userId));
      }
    getUser();
  }, []);

  const handleSubmit = async (event) => {
    event.preventDefault();
    const response = (await axios.delete(APIEndpoint + '/users/'+params.userId
      , {headers: {
        Authorization: BearerAuth(accessToken)
      }}));
    navigate("/users");  
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

export default UserDelete;