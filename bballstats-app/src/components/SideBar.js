import React from 'react';
import { slide as Menu } from 'react-burger-menu';
import '../Sidebar.css';
import { useAuth } from '../provider/Authentication';

export default props => {
  const { currentUserRoles } = useAuth();

  return (
    <Menu>
        <a href='/home' className="duration-75 brightness-95 hover:brightness-100 text-xl font-bold py-5 text-white inline" >
            <img className="h-10" src={process.env.PUBLIC_URL + '/icons/home.svg'}/>
        </a>
        <a href='/algorithms' className="duration-75 brightness-95 hover:brightness-100 text-xl font-bold py-5 text-white inline" >
            Algorithms
        </a>
        <a href='/players' className="duration-75 brightness-95 hover:brightness-100 text-xl font-bold py-5 text-white inline" >
            Players
        </a>
        {currentUserRoles &&
        <a href='/profile' className="duration-75 brightness-95 hover:brightness-100 text-xl font-bold py-5 text-white inline" >
            Profile
        </a>
        }
        
        
        {currentUserRoles && currentUserRoles.includes('Curator') &&
            <a href='/stats' className="duration-75 brightness-95 hover:brightness-100 text-xl font-bold py-5 text-white inline" >
                Stats
            </a>
        }

        {currentUserRoles && currentUserRoles.includes('Admin') &&
            <a href='/users' className="duration-75 brightness-95 hover:brightness-100 text-xl font-bold py-5 text-white inline" >
                Users
            </a>
        }
        
        {currentUserRoles &&
            <a href='/logout' className="duration-75 brightness-95 hover:brightness-100 text-xl font-bold py-5 text-white inline" >
                Logout
            </a>
        }
        {!currentUserRoles &&
            <a href='/login' className="duration-75 brightness-95 hover:brightness-100 text-xl font-bold py-5 text-white inline" >
                Login
            </a>
        }
        {!currentUserRoles &&
            <a href='/register' className="duration-75 brightness-95 hover:brightness-100 text-xl font-bold py-5 text-white inline" >
                Register
            </a>
        }
    </Menu>
  );
};