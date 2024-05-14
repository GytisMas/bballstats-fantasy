import { useAuth } from "../../provider/Authentication";
import {BearerAuth, ButtonStyle, DisabledFormMemberStyle, Form2XLContainerStyle, Form4XLContainerStyle, FormMemberStyle, FormSelectStyle, FormSumbitStyleCancel, FormSumbitStyleCancel2, FormWiderContainerStyle} from '../../components/Helpers';
import {FormContainerStyle, FormSumbitStyle, FormHelperStyle} from '../../components/Helpers';
import { useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from 'react';
import { roles } from "../../components/PlayerRoles";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";
import Modal from '../../components/Modal';
import { jwtDecode } from "jwt-decode";

function LeagueParticipate(props) {
  let params = useParams();
  const { accessToken } = useAuth();
  const [errorMessages, setErrorMessages] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);  
  const [isLoading, setIsLoading] = useState(true);
  const [statTypes, setStatTypes] = useState([]);
  const [isTemplLoading, setIsTemplLoading] = useState(true);
  const [template, setTemplate] = useState([]);
  const [leagueRoles, setLeagueRoles] = useState([]);
  const [leagueRolesUnavailable, setLeagueRolesUnavailable] = useState([]);
  const [players, setPlayers] = useState([]);
  const [isEditMode, setIsEditMode] = useState(false)
  const [isActiveLeague, setIsActiveLeague] = useState(false)
  const [rosterChangesAllowed, setRosterChangesAllowed] = useState(false)
  const [modalIsShowing, setModalIsShowing] = useState(false);
  const [showPasswordForm, setShowPasswordForm] = useState(false);
  
  const [playersPrice, setPlayersPrice] = useState(0)

  const [participant, setParticipant] = useState([]);
  const [team, setTeam] = useState([]);
  const [leagueEntryFee, setLeagueEntryFee] = useState(0)

  const navigate = useNavigate();

  useEffect(() => {
    const loadData = async () => {
      let edit = params.participantId != null;
      setIsEditMode(edit)

      if (!edit) {
        const searchParticipantResponse = (await axios.get(APIEndpoint + '/Fantasy/Leagues/' + params.leagueId + '/User'));
        if (searchParticipantResponse.data != null && searchParticipantResponse.data != '') {
          navigate("/fantasy/leagues/" + params.leagueId + "/participants/" + searchParticipantResponse.data, {replace: true})
          return;
        }
      }

      if (!edit) {
        const response = (await axios.get(APIEndpoint + '/Statistics'));
        setStatTypes(response.data);
        const response2 = (await axios.get(APIEndpoint + '/Fantasy/Leagues/' + params.leagueId + '/Players'));
        setIsActiveLeague(response2.data.isActive)
        setShowPasswordForm(response2.data.isPrivate);
        setLeagueEntryFee(response2.data.entryFee);
        setPlayers(response2.data.players
          .sort((a, b) => a.teamId > b.teamId ? 1 : -1));
        const templResponse = (await axios.get(APIEndpoint + '/LeagueTemplates/' + response2.data.leagueTemplateId));
        console.log(templResponse.data);
        setTemplate(templResponse.data);
        setLeagueRoles(templResponse.data.leagueRoles)
      } else {
        const response = (await axios.get(APIEndpoint + '/Fantasy/Leagues/' + params.leagueId +'/Participants/' + params.participantId));
        setParticipant(response.data);
        setRosterChangesAllowed(response.data.allowRosterChanges);
        setIsActiveLeague(response.data.leagueIsActive)
        const nextMatchesResponse = (await axios.get(APIEndpoint + '/Participants/' + params.participantId +'/matches'))
        if (response.data.allowRosterChanges) {
          setTeam(response.data.team
            .sort((a, b) => a.roleId > b.roleId ? 1 : -1));
        }
        if (!response.data.leagueIsActive) {
          const statsResponse = (await axios.get(APIEndpoint + '/Statistics'));
          setStatTypes(statsResponse.data);
          const leaguesResponse = (await axios.get(APIEndpoint + '/Fantasy/Leagues/' + params.leagueId + '/Players'));
          setPlayers(leaguesResponse.data.players
            .sort((a, b) => a.teamId > b.teamId ? 1 : -1));
          const templResponse = (await axios.get(APIEndpoint + '/LeagueTemplates/' + leaguesResponse.data.leagueTemplateId));
          setTemplate(templResponse.data);
          setLeagueRoles(templResponse.data.leagueRoles)
          setUnavailableRoles(response.data.team, templResponse.data.leagueRoles, leaguesResponse.data.players)
        }
      }
    }

    loadData().then(() => setIsLoading(false));
    }, [isLoading]);

  const setUnavailableRoles = (_team, _roles, _players) => {
    let newUnavailableRoles = []
    for (let i = 0; i < _team.length; i++) {
      const player = _team[i]
      let newElement = _roles.find((e) => e.id == player.roleId)
      // console.log(newElement)
      if (newElement == null) {
        continue;
      }
      // console.log("--")
      const leaguePlayerId = _players.find((e) => e.id == player.playerId).leaguePlayerId
      newElement.playerId = leaguePlayerId
      newUnavailableRoles = [...newUnavailableRoles, newElement]
    }
    setLeagueRolesUnavailable(newUnavailableRoles);
  }

  
  const showModal = () => {
    setModalIsShowing(true);
  };

  const hideModal = () => {
    setModalIsShowing(false);
  };

  const MakeRoleItem = function(x, i, ri) {
    return (
        <td className='p-2 border-2 text-left'>{x.name}</td>
    );
  }

  const changeAvailableRoles = (playerId, playerPrice) => (event) => {
    let currPlayerPrice = playersPrice;
    console.log(currPlayerPrice)
    if (event.target.value != -1) {
      let newUnavailableRoles = leagueRolesUnavailable;
      let existingElement = leagueRolesUnavailable.find((e) => e.playerId == playerId)
      if (existingElement != null) {
        currPlayerPrice -= existingElement.playerPrice;
        newUnavailableRoles = (leagueRolesUnavailable.filter(item => item.playerId != playerId))
      }
      
      let newElement = leagueRoles.find((e) => e.id == event.target.value)
      newElement.playerId = playerId
      newElement.playerPrice = playerPrice
      currPlayerPrice += newElement.playerPrice;
      newUnavailableRoles = [...newUnavailableRoles, newElement]
      // console.log(newElement)
      setLeagueRolesUnavailable(newUnavailableRoles);
    } else {
      console.log(playerId)
      // console.log(leagueRolesUnavailable)
      // console.log(leagueRolesUnavailable.filter(item => item.playerId != playerId))
      // console.log("--")
      let existingElement = leagueRolesUnavailable.find((e) => e.playerId == playerId)
      if (existingElement != null) {
        console.log(existingElement)
        currPlayerPrice -= playerPrice;
      }
      setLeagueRolesUnavailable((leagueRolesUnavailable) => (leagueRolesUnavailable.filter(item => item.playerId != playerId)))
    }
    console.log(currPlayerPrice)
    setPlayersPrice(currPlayerPrice);
  } 

  const MakePlayerItem = function(x) {
    // console.log(leagueRoles[0])
    return (
      <td key={x.leaguePlayerId} className='p-2 border-2 text-left'>
        <div>
          <p>{x.name}</p>
          <p>{x.teamId}</p>
          <p>Price: {x.price}</p>
          <select onChange={changeAvailableRoles(x.leaguePlayerId, x.price)} defaultValue={defaultSelectedPlayerRole(x)} name={"roleof-"+x.leaguePlayerId}>
            <option key={-1} value={-1}>Not Selected</option>
            {leagueRoles.map((role, index) => (
              <option key={role.id} value={role.id} disabled={leagueRolesUnavailable.find((e) => e.id == role.id) != null ? true : false}>{role.name} {role.roleToReplaceIndex && "(substitute for " + leagueRoles.find((e) => e.id == role.roleToReplaceIndex).name + ")"}</option>
            ))
            }
          </select>
        </div>
      </td>);
  }

  const handleRoleChange = (newActivePlayer) => {
    console.log(newActivePlayer.playerName)
    let oldActivePlayer = team.find((plr) => plr.roleId == newActivePlayer.roleToReplaceId)
    console.log(oldActivePlayer.playerName)

    const changedTeam = team.map(player => {
      if (player.playerId === newActivePlayer.playerId) {
        return {
          ...oldActivePlayer,
          roleId: newActivePlayer.roleId,
          roleName: newActivePlayer.roleName,
          roleToReplaceId: newActivePlayer.roleToReplaceId
        }
      } 
      else if (player.playerId === oldActivePlayer.playerId) {
        return {
          ...newActivePlayer,
          roleId: oldActivePlayer.roleId,
          roleName: oldActivePlayer.roleName,
          roleToReplaceId: oldActivePlayer.roleToReplaceId
        }
      }
      else {
        return player;
      }
    });

    setTeam(changedTeam);
  }

  const MakePlayerEditItem = function(x) {
    return (
      <td key={x.leaguePlayerId} className='p-2 border-2 text-left'>
        <div>
          <p>{x.roleName}</p>
          <p>{x.playerName}</p>
          <p>{x.teamName}</p>
          <p>Price: {x.price}</p>
          {x.roleToReplaceId != null &&
            <button type="button" className={FormSumbitStyle} onClick={() => handleRoleChange(x)}>Replace Active Role ({team.find((plr) => plr.roleId == x.roleToReplaceId).roleName})</button>

          }
        </div>
      </td>);
  }

  const errors = {
    templateName: "Invalid league template name",
    password: "Invalid Password",
    roles: "Problem with roles"
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    // console.log(document.forms[0].elements);
    if (isEditMode) {
      if (isActiveLeague) {
        const playerRolePairs = team.map((plr) => {
          return { Id: plr.id, RoleId: plr.roleId, RoleName: plr.roleName }
        })
        const formData = {
          TeamName: document.forms[0].elements["teamName"].value,
          PlayerRolePairs: playerRolePairs, 
        }
        console.log(formData)
        try {
            const response = await axios.put(APIEndpoint + "/fantasy/leagues/" + params.leagueId + '/participants/' + params.participantId + '/roles', formData
            , {headers: {
              Authorization: BearerAuth(accessToken)
            }}
            );
            navigate('/fantasy/leagues/' + params.leagueId + '/participants/' + params.participantId, {replace: true});
        } catch (error) {
            console.log(error);
            console.log(error.response.data);
            if (error.response.data.includes("exists"))
              setErrorMessages({ name: "templateName", message: error.message });
        }
      } else {
        let selectedStats = Array.from(document.querySelectorAll('[name^=roleof-]')).filter(r => r.value != -1)
        const formPlayers = []
        for (let i = 0; i < selectedStats.length; i++) {
          const stat = selectedStats[i]
          formPlayers.push({leaguePlayerRoleId: stat.value, leagueAvailablePlayerId: stat.name.split('-')[1]})      
        }

        if (selectedStats.length !== leagueRoles.length) {
          setErrorMessages({ name: "submit", message: "Not enough roles selected" });
          return;
          // TODO: normal validation
        }

        const formData = {
          teamName: document.forms[0].elements["teamName"].value, 
          players: formPlayers
        }
        console.log("--")
        console.log(formData)
        console.log("-")
        try {
            const response = await axios.put(APIEndpoint + "/fantasy/leagues/" + params.leagueId + '/participants/' + params.participantId, formData
            , {headers: {
              Authorization: BearerAuth(accessToken)
            }}
            );
            navigate('/fantasy/leagues/' + params.leagueId + '/participants/' + params.participantId, {replace: true});
        } catch (error) {
            console.log(error);
            if (error.response.data && error.response.data.includes("exists"))
              setErrorMessages({ name: "templateName", message: "League template with name already exists" });
        }
      }
      
      return;
    }

    let selectedStats = Array.from(document.querySelectorAll('[name^=roleof-]')).filter(r => r.value != -1)
    const formPlayers = []
    for (let i = 0; i < selectedStats.length; i++) {
      const stat = selectedStats[i]
      formPlayers.push({leaguePlayerRoleId: stat.value, leagueAvailablePlayerId: stat.name.split('-')[1]})      
    }

    if (selectedStats.length !== leagueRoles.length) {
      setErrorMessages({ name: "submit", message: "Not enough roles selected" });
      return;
    }

    const formData = {
      teamName: document.forms[0].elements["teamName"].value, 
      players: formPlayers
    }
    // console.log("--")
    // console.log(formData)
    // console.log("-")
    try {
        const response = await axios.post(APIEndpoint + "/fantasy/leagues/" + params.leagueId + '/participants/', formData
        , {headers: {
          Authorization: BearerAuth(accessToken)
        }}
        );
        console.log(response)
        navigate('/fantasy/leagues/' + params.leagueId + '/participants/' + response.data, {replace: true});
    } catch (error) {
        console.log(error);
        console.log(error.response.data);
        if (error.response.data.includes("exists"))
          setErrorMessages({ name: "templateName", message: "League template with name already exists" });
    }
  };

  // Generate JSX code for error message
  const renderErrorMessage = (name) =>
    name === errorMessages.name && (
      <div className="error text-red-500">{errorMessages.message}</div>
  );

  const defaultSelectedPlayerRole = (player) => {
    if (isEditMode) {
      const selectedRole = leagueRolesUnavailable.find(r => r.playerId == player.leaguePlayerId)
      if (selectedRole != null) {
        return selectedRole.id;
      }
    }
    return -1;
  }

  const cancelParticipationForm = (submitFunc) => {
    return <div className={FormContainerStyle}>
      <form onSubmit={submitFunc}>
        <div className="flex flex-col justify-center items-center">
          <label>Confirm participation cancellation?</label>
          <input className={FormSumbitStyle} type="submit" />
        </div>
      </form>
    </div>;
  }

  const submitCancel = async (event) => {
    event.preventDefault();
    console.log("deleting");
    try {
      const response = await axios.delete(APIEndpoint + '/Fantasy/Leagues/' + params.leagueId +'/Participants/' + params.participantId,
        {headers: {
          Authorization: BearerAuth(accessToken)
        }}
      );
      navigate('/Fantasy/Leagues/', {replace: true});
    } catch (error) {
      console.log(error);
    }
    hideModal();
  };

  const submitPassword = async (event) => {
    event.preventDefault();
    try {
      const response = await axios.post(APIEndpoint + '/Fantasy/Leagues/' + params.leagueId 
      + '/Password/?Password=' + document.forms[0].elements["password"].value, 
        {headers: {
          Authorization: BearerAuth(accessToken)
        }}
      );
      if (response.data == true) {
        document.getElementById("passwordForm").reset();
        setShowPasswordForm(false);
      } else {
        setErrorMessages({ name: "password", message: "Wrong league password" });
      }
    } catch (error) {
      console.log(error);
    }
    hideModal();
  };

  const passwordForm = (
    <div className={Form4XLContainerStyle}>
      <form id="passwordForm" onSubmit={submitPassword}>
        <div className="input-container">
          <label>League entry password</label>
          <input className={FormMemberStyle} type="text" name="password" required />
          {renderErrorMessage("password")}
        </div>
        <div className="flex flex-row items-center">
          <button className={FormSumbitStyle + " h-16"} type="submit" >Submit</button>
        </div>
      </form>
    </div>
  );

  // JSX code for login form
  const renderForm = (
    <div className={Form4XLContainerStyle}>
      <form id="mainForm" onSubmit={handleSubmit}>
        <div className="input-container">
          <label>Team name</label>
          {isEditMode ?<input className={DisabledFormMemberStyle} type="text" disabled name="teamName" required defaultValue={participant.teamName ?? ""} /> :
          <input className={DisabledFormMemberStyle} type="text" name="teamName" required defaultValue={participant.teamName ?? ""} />
          
          }
          {renderErrorMessage("teamName")}
        </div>
        <div className="input-container">
          {!isActiveLeague ?
            isLoading ? <div>Loading...</div> :
              <div className='flex flex-row flex-wrap justify-center items-stretch mt-5 max-w-6xl mx-auto px-2 pb-10 bg-white border-2 rounded-3xl'>
                {players.map((player, index) => (
                  <div key={index} className='w-64 mt-10 mx-1 pt-1 pb-2 rounded-xl flex flex-col justify-start items-center bg-gradient-to-b bg-white'>
                    {/* <p className="rounded-t-xl text-xl text-white text-center w-64 py-2 bg-gradient-to-b from-slate-500 to-black">{player.roleName}</p> */}
                    <div className='w-64 h-24 flex border-2 border-slate-400 rounded-t-xl flex-col p-1 items-center justify-center bg-gradient-to-b from-slate-300 to-slate-100'>
                      <p className="px-1 text-lg w-full text-left">{player.name}</p>
                      <p className="px-1 text-slate-600 w-full text-sm text-left">{player.teamId}</p>
                    </div>                      
                    <div className='w-64 flex border-x-2 border-slate-400  flex-col p-1 items-center bg-slate-100'>
                      <p className="px-1 text-slate-600 w-full text-sm text-left">Fantasy role</p>
                      <select className={FormSelectStyle} onChange={changeAvailableRoles(player.leaguePlayerId, player.price)} defaultValue={defaultSelectedPlayerRole(player)} name={"roleof-"+player.leaguePlayerId}>
                        <option key={-1} value={-1}>Not Selected</option>
                        {leagueRoles.map((role, index) => ( 
                          <option key={role.id} value={role.id} disabled={leagueRolesUnavailable.find((e) => e.id == role.id) != null ? true : false}>{role.name} {role.roleToReplaceIndex && "(substitute for " + leagueRoles.find((e) => e.id == role.roleToReplaceIndex).name + ")"}</option>
                          ))
                        }
                      </select>
                    </div>
                    <div className='w-64 flex border-2 border-slate-400  flex-col rounded-b-xl p-1 items-center bg-slate-200 bg-gradient-to-b from-slate-100 to-slate-300'>
                      <p className="px-1 w-full text-sm text-left">Price: {player.price}</p>
                    </div>
                  </div>
                ))}
              </div>
            : 
            isLoading ? <div>Loading...</div> : isEditMode &&
              !rosterChangesAllowed ? <div>Roster changes in active leagues are only allowed between midnight and 12pm UTC</div> :
              <div className='flex flex-row flex-wrap justify-center items-stretch mt-5 max-w-6xl mx-auto px-2 pb-10 bg-white border-2 rounded-3xl'>
                  {team.map((player, index) => (
                    <div key={index} className='w-64 mt-10 mx-1 pt-1 pb-2 rounded-xl flex flex-col justify-start items-center bg-gradient-to-b bg-white'>
                      {/* <p className="rounded-t-xl text-xl text-white text-center w-64 py-2 bg-gradient-to-b from-slate-500 to-black">{player.roleName}</p> */}
                      <div className='w-64 h-24 flex border-2 border-slate-400 rounded-t-xl flex-col p-1 items-center justify-center bg-gradient-to-b from-slate-300 to-slate-100'>
                        <p className="px-1 text-lg w-full text-left">{player.playerName}</p>
                        <p className="px-1 text-slate-600 w-full text-sm text-left">{player.teamName}</p>
                      </div>                      
                      <div className='w-64 flex border-x-2 border-slate-400  flex-col p-1 items-center bg-slate-100'>
                        <p className="px-1 text-slate-600 w-full text-sm text-left">Fantasy role</p>
                        <p className="px-1 text-lg w-full text-left ">{player.roleName}</p>
                        {player.roleToReplaceId != null &&
                          <button type="button" className={ButtonStyle + " w-48"} onClick={() => handleRoleChange(player)}>Replace Active Role ({team.find((plr) => plr.roleId == player.roleToReplaceId).roleName})</button>
                        }
                      </div>
                      <div className='w-64 flex border-2 border-slate-400  flex-col rounded-b-xl p-1 items-center bg-slate-200 bg-gradient-to-b from-slate-100 to-slate-300'>
                        <p className="px-1 w-full text-sm text-left">Price: {player.price}</p>
                      </div>
                    </div>
                  ))}
              </div>
          }
          
        </div>
        {renderErrorMessage("roles")}
        {/* <div className='flex flex-row items-center'>
          
        </div> */}
        <div>
          {!isActiveLeague && !isEditMode ? <>
            <p>Cost to enter league:</p> 
            <p>{playersPrice} (cost for players)</p> 
            <p>{leagueEntryFee} (league entry fee)</p> 
            <p>{playersPrice + leagueEntryFee} (total)</p> 
            </>
            : !isActiveLeague && isEditMode &&
            <p>Cost to edit team: {playersPrice}</p>

          }
        </div>
        <div className="flex flex-row items-center">
          <input className={FormSumbitStyle + " h-16"} type="submit" />
          {!isActiveLeague && isEditMode &&
            <button onClick={() => showModal()}type="button" className={FormSumbitStyleCancel2 + " h-16"}>Cancel participation</button>
          }
        </div>
        
      </form>
      {modalIsShowing &&
        <Modal show={modalIsShowing} handleClose={() => hideModal()} formContent={cancelParticipationForm(submitCancel)}>
          <p>Modal</p>
        </Modal>
      }
      {renderErrorMessage("submit")}
    </div>
  );

  return (
    
    <div className="app">
      {isLoading ? <div>Loading...</div>
      :
      !isEditMode && isActiveLeague ? <div>Joining league not allowed when league is active.</div> :
      <div className="login-form">
        {showPasswordForm ? passwordForm :
          isSubmitted ? <div>Submitted</div> : renderForm
        }
      </div>
      }
    </div>
  );
}

export default LeagueParticipate;