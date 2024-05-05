import logo from './logo.svg';
import './App.css';
import { useEffect, useState } from 'react';
import Stats from './pages/Stats';
import UserGet from './pages/UserGet';
import UsersList from './pages/UsersList';


function App() {
  const [openPage, setOpenPage] = useState('user');
  return (
    <div>
      <div className='min-w-full'>
        {
          {
            'home': <Stats />,
            'user': <UserGet userId='8ea491e5-2ecb-4cb3-b6a9-64f017aba55e'/>,
            'users': <UsersList/>
          }[openPage] || <div/>
        } 
      </div>                 
    </div>
  );
}

export default App;
