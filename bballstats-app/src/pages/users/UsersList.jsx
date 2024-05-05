import { useEffect, useState } from 'react';
import { jwtDecode } from 'jwt-decode'
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";
import { useAuth } from "../../provider/Authentication";
import {BearerAuth} from '../../components/Helpers';
import { useNavigate } from "react-router-dom";
import NotFound from '../../components/NotFound';

export default function UsersList(props) {
    const { accessToken, currentUserRoles } = useAuth();
    const [users, setUsers] = useState([]);
    const navigate = useNavigate();
      useEffect(() => {
        const loadUsers = async () => {
          if (!props.isAdmin)
            return;
          try {
            const response = await axios.get(APIEndpoint + "/users"
            , {headers: {
              Authorization: BearerAuth(accessToken)
            }}
            );
            setUsers(response.data.sort(function(a, b){return a.id - b.id}));
          } catch (error) {
              console.log(error);
          }
        };
    
        loadUsers();
      }, []);

      const handleUpdate = (userId) => {
        navigate('/users/update/'+userId); 
      }
    
      const handleDelete = (userId) => {
        navigate('/users/delete/'+userId); 
      }
    
      if (!props.isAdmin)
      return <NotFound/>;

      return (
        <div className='flex flex-col flex-wrap justify-evenly items-center mt-5 pb-5 max-w-xl mx-auto px-2  bg-white border-2 rounded-3xl'>
          <a href='/users/create' className='bth p-1 duration-200 bg-slate-300 hover:bg-green-300 border-2 my-2 border-black'>Create New User</a>
          <table>
            <thead>
              <tr>
                <th className='p-2 border-2 text-left'>User name</th>
                <th className='p-2 border-2 text-left'>Email</th>
                <th className='p-2 border-2 text-left'>Manage</th>
              </tr>
            </thead>   
            <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td className='p-2 border-2 text-left'>{user.username}</td>
              <td className='p-2 border-2 text-left'>{user.email}</td>
              <td className='p-2 border-2'>
                <button onClick={() => handleUpdate(user.id)} className='align-middle block mx-auto'><img className='sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-pencil-24.png'}/></button>
                <button onClick={() => handleDelete(user.id)} className='align-middle block mx-auto'><img className='sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-trash-24.png'}/></button>
              </td>
            </tr>            
          ))}
            </tbody>       
          </table>
        </div>
      );
    }