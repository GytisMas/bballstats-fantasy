import { useEffect, useState } from 'react';
import { useNavigate } from "react-router-dom";
import axios from 'axios';
import { APIEndpoint, BearerAuth, ButtonStyle, FormContainerStyle, FormMemberStyle, FormSelectStyle, FormSumbitStyle } from "../../components/Helpers";
import NotFound from '../../components/NotFound';
import { useAuth } from '../../provider/Authentication';
import Modal from '../../components/Modal';
export default function Stats(props) {
  const { accessToken } = useAuth();

  const [stats, setStats] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  const [targetStatId, setTargetStatId] = useState(-1);
  const [targetStat, setTargetStat] = useState([]);
  const [statVisibility, setStatVisibility] = useState(1);

  const [modalIsShowing, setModalIsShowing] = useState(false);
  const [modalType, setModalType] = useState("update");

  const StatusNames = ["Regular", "Main"]

  const navigate = useNavigate();
    useEffect(() => {
      const loadStats = async () => {
        if (!props.isCurator)
          return;
        const response = await axios.get(APIEndpoint + '/statistics');
        setStats(response.data.sort(function(a, b){return a.id - b.id}));
        setStatVisibility(response.data.status)
      };

      loadStats().then(() => setIsLoading(false));
    }, [isLoading]);  

    const handleCreateSubmit = async (event) => {
      event.preventDefault();
      var { dname, sname, visibility, leaguePoints, leagueChecked } = document.forms[0];
      const statData = {
        displayName: dname.value != "" ? dname.value : null,
        name: sname.value,
        status: Number(visibility.value),
        defaultLeaguePointsPerStat: Number(leaguePoints.value),
        defaultIsChecked: leagueChecked.checked
      };
  
      try {
          const response = await axios.post(APIEndpoint + "/statistics", statData
          , {headers: {
              Authorization: BearerAuth(accessToken)
            }}
          );
          setIsLoading(true);
          hideModal();
      } catch (error) {
          console.log(error);
          // setErrorMessages({ name: "uname", message: "Incorrect user name or password" });
      }
    };

  const handleUpdateSubmit = async (event) => {
    event.preventDefault();
    var { dname, sname, visibility, leaguePoints, leagueChecked } = document.forms[0];
    const statData = {
        displayName: dname.value != "" ? dname.value : null,
        name: sname.value,
        status: Number(visibility.value),
        defaultLeaguePointsPerStat: Number(leaguePoints.value),
        defaultIsChecked: leagueChecked.checked
    };

    try {
        const response = await axios.put(APIEndpoint + "/statistics/"+ targetStat.id, statData
        , {headers: {
            Authorization: BearerAuth(accessToken)
          }}
        );
        setIsLoading(true);
        hideModal();
    } catch (error) {
        console.log(error);
        // setErrorMessages({ name: "uname", message: "Incorrect user name or password" });
    }
  };

  const handleDeleteSubmit = async (event) => {
    event.preventDefault();
    try {
      const response = await axios.delete(APIEndpoint + "/statistics/" + targetStat.id,
      // , {headers: {
      //     Authorization: BearerAuth(accessToken)
      //   }}
      );
      setIsLoading(true)
      hideModal();
    } catch (error) {
      console.log(error);
    }
  };
  
  const statCreateForm = (submitFunc) => {
    return (
    <div key={-1} className={FormContainerStyle}>
      <form onSubmit={submitFunc}>
        <div className="input-container">
          <label>Display name</label>
          <input className={FormMemberStyle} type="text" name="dname" required />
        </div>
        <div className="input-container">
          <label>Short name</label>
          <input className={FormMemberStyle} type="text" name="sname" required />
        </div>
        <div className="input-container">
          <label>Visibility status</label>
          <select className={FormSelectStyle} name='visibility'>
            <option key={1} value={1}>Main</option>
            <option key={0} value={0}>Regular</option>
          </select>
        </div>
        <div className="input-container">
          <label>Default FL Points</label>
          <input className={FormMemberStyle} type="number" step={0.001} name="leaguePoints" defaultValue={1} required />
        </div>
        <div className="input-container">
          <label>Used in FL by default</label>
          <br/><input type="checkbox" name="leagueChecked" defaultChecked={false} />
        </div>
        <input className={FormSumbitStyle} type="submit" />
      </form>
    </div>
    );
  }

  const statUpdateForm = (submitFunc) => {
    return (
    <div key={targetStat.id} className={FormContainerStyle}>
      <form onSubmit={submitFunc}>
        <div className="input-container">
          <label>Display name</label>
          <input className={FormMemberStyle} type="text" name="dname" defaultValue={targetStat.displayName} />
        </div>
        <div className="input-container">
          <label>Short name</label>
          <input className={FormMemberStyle} type="text" name="sname" defaultValue={targetStat.name} required />
        </div>
        <div className="input-container">
          <label>Visibility status</label>
          <select className={FormSelectStyle} defaultValue={targetStat.status} name='visibility'>
            <option key={1} value={1}>Main</option>
            <option key={0} value={0}>Regular</option>
          </select>
        </div>
        <div className="input-container">
          <label>Default FL Points</label>
          <input className={FormMemberStyle} type="number" step={0.001} name="leaguePoints" defaultValue={targetStat.defaultLeaguePointsPerStat} required />
        </div>
        <div className="input-container">
          <label>Used in FL by default</label>
          <br/><input type="checkbox" name="leagueChecked" defaultChecked={targetStat.defaultIsChecked} />
        </div>
        <input className={FormSumbitStyle} type="submit" />
      </form>
    </div>
    );
  }

  const statDeleteForm = (submitFunc) => {
    return <div key={targetStat.id} className={FormContainerStyle}>
      <form onSubmit={submitFunc}>
          <div className="flex flex-col justify-center items-center">
            <label>Confirm deletion?</label>
            <input className={FormSumbitStyle} type="submit" />
          </div>
        </form>
    </div>;
  }

  const showModalCreate = () => {
    setTargetStatId(-1)
    setModalIsShowing(true);
    setModalType("create");
  };

  const showModalUpdate = (statId) => {
    setTargetStatId(statId)
    setStatInfo(statId);
    setModalIsShowing(true);
    setModalType("update");
  };

  const showModalDelete = (statId) => {
    setTargetStatId(statId)
    setStatInfo(statId);
    setModalIsShowing(true);
    setModalType("delete");
  };

  const hideModal = () => {
    setTargetStatId(-1)
    setModalIsShowing(false);
  };

  let setStatInfo = async (statId) => {
    try {
      const response = (await axios.get(APIEndpoint + '/statistics/'+ statId));
      setTargetStat(response.data)
    } catch (error) {
        console.log(error);
    }
  }

  if (!props.isCurator)
    return <NotFound/>;

  return (
    <div className='flex flex-col flex-wrap justify-evenly items-center mt-5 pb-5 max-w-3xl mx-auto px-2  bg-white border-2 rounded-3xl'>
      <button type="button" onClick={() => showModalCreate()} className={ButtonStyle}>Create Statistic Type</button>
      <table>
        <thead>
        <tr>
          <th className='p-2 border-2 text-left'>ID</th>
          <th className='p-2 border-2 text-left'>Display Name</th>
          <th className='p-2 border-2 text-left'>Name</th>
          <th className='p-2 border-2 text-left'>Visibility Status</th>
          <th className='p-2 border-2 text-left'>Default FL points</th>
          <th className='p-2 border-2 text-left'>Used in FL by default</th>
          <th className='p-2 border-2 text-left'>Manage</th>
        </tr>
        </thead>
        <tbody>
        {stats.map((stat) => (
          <tr key={stat.id}>
            <td className='p-2 border-2 text-right'>{stat.id}</td>
            <td className='p-2 border-2 text-left'>{stat.displayName}</td>
            <td className='p-2 border-2 text-left'>{stat.name}</td>
            <td className='p-2 border-2 text-left'>{StatusNames[stat.status]}</td>
            <td className='p-2 border-2 text-right'>{stat.defaultLeaguePointsPerStat}</td>
            <td className='p-2 border-2 text-center'>{stat.defaultIsChecked ? (<p className='font-bold'>Yes</p>) : "No"}</td>
            <td className='p-2 border-2'>
              
              <button className='align-middle block mx-auto' onClick={() => showModalUpdate(stat.id)} ><img className='sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-pencil-24.png'}/></button>
              <button className='align-middle block mx-auto' onClick={() => showModalDelete(stat.id)} ><img className='sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-trash-24.png'}/></button>
            </td>
          </tr>
        ))}
        </tbody>
      </table>
      {modalType == "update" ?
        <Modal show={modalIsShowing} handleClose={() => hideModal()} formContent={statUpdateForm(handleUpdateSubmit)}>
          <p>Modal</p>
        </Modal>
      : modalType == "delete" ? 
        <Modal  show={modalIsShowing} handleClose={() => hideModal()} formContent={statDeleteForm(handleDeleteSubmit)}>
          <p>Modal</p>
        </Modal> 
      : <Modal show={modalIsShowing} handleClose={() => hideModal()} formContent={statCreateForm(handleCreateSubmit)}>
          <p>Modal</p>
        </Modal> 
      }
    </div>
  );
}