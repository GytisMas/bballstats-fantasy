import { useEffect, useState } from 'react';
import {FormulaSecondHalf} from '../../components/Helpers';
import { jwtDecode } from 'jwt-decode'
import { useAuth } from "../../provider/Authentication";
import {BearerAuth} from '../../components/Helpers';
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";
import UserGet from './UserGet';
import { useNavigate } from "react-router-dom";
import Modal from '../../components/Modal';
export default function AlgorithmGet(props) {
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();
  const [algorithm, setAlgorithm] = useState([]);
  const [algoStats, setAlgoStats] = useState([]);
  const { accessToken, currentUserRoles } = useAuth();
  const [impressionsPos, setImpressionsPos] = useState(0);
  const [impressionsNeg, setImpressionsNeg] = useState(0);
  const [posIsSet, setPosIsSet] = useState(false);
  const [negIsSet, setNegIsSet] = useState(false);
  const [modalIsShowing, setModalIsShowing] = useState(false);

  const showModal = () => {
    setModalIsShowing(true);
  };
  const hideModal = () => {
    setModalIsShowing(false);
  };

  useEffect(() => {
    const loadAlgoStats = async () => {
        const response = (await axios.get(APIEndpoint + '/users/' 
            + props.userId + '/customStatistics/' + props.algoId,
        ));
        setAlgorithm(response.data);
        setAlgoStats(response.data.statIds);
        const response3 = (await axios.get(APIEndpoint + '/users/' 
            + props.userId + '/customStatistics/' + props.algoId + '/algorithmImpressions',
        )).data;
        let numToAddPos = 0
        let numToAddNeg = 0
        for (let i = 0; i < response3.length; i++) {
          let impr = response3[i]
          if (impr.positive) {
            numToAddPos += 1
          } else {
            numToAddNeg += 1
          }
        }
        setImpressionsPos(impressionsPos + numToAddPos);
        setImpressionsNeg(impressionsNeg + numToAddNeg);
        handleImpression(false, true)
        
    }
    loadAlgoStats().then(() => setIsLoading(false));
  }, []);

  const handleImpression = async (thumbsUp, onlyCheck) => {
    const impressionData = {
      positive: thumbsUp
    }

    const impressionsResponse = await axios.get(APIEndpoint + '/users/' 
    + props.userId + '/customStatistics/' + props.algoId + '/algorithmImpressions');
    let update = false;
    let imprId = 0;
    let imprIndex = 0;
    if (accessToken) {
      const thisUserId = jwtDecode(accessToken).sub
      for (let i = 0; i < impressionsResponse.data.length; i++) {
        if (impressionsResponse.data[i].userId == thisUserId) {
          update = true;
          imprIndex = i;
          imprId = impressionsResponse.data[i].id;
          if (impressionsResponse.data[i].positive) {
            setPosIsSet(true);
            setNegIsSet(false);
          } else {
            setNegIsSet(true);
            setPosIsSet(false);
          }
          break;
        }
      }
    }
    
    if (onlyCheck)
      return;
    if (!update) {
      try {
        const response = await axios.post(APIEndpoint + '/users/' 
          + props.userId + '/customStatistics/' + props.algoId + '/algorithmImpressions', impressionData
          , {headers: {
              Authorization: BearerAuth(accessToken)
            }}
        );
        if (response.data.positive) {
          setPosIsSet(true);
          setImpressionsPos(impressionsPos+1)
        }
        else if (!response.data.positive) {
          setNegIsSet(true);
          setImpressionsNeg(impressionsNeg+1)
        }
      } catch (error) {
          console.log(error);
      }
    } else {
      if (impressionsResponse.data[imprIndex].positive == thumbsUp) {
        try {
          const response = await axios.delete(APIEndpoint + '/users/' 
            + props.userId + '/customStatistics/' + props.algoId + '/algorithmImpressions/' + imprId
            , {headers: {
                Authorization: BearerAuth(accessToken)
              }}
          );
          if (thumbsUp) {
            setPosIsSet(false);
            setImpressionsPos(impressionsPos-1)
          }
          else if (!thumbsUp) {
            setNegIsSet(false);
            setImpressionsNeg(impressionsNeg-1)
          }
        } catch (error) {
            console.log(error);
        }
      } else {
        try {
          const response = await axios.put(APIEndpoint + '/users/' 
            + props.userId + '/customStatistics/' + props.algoId + '/algorithmImpressions/' + imprId, impressionData
            , {headers: {
                Authorization: BearerAuth(accessToken)
              }}
          );
          if (response.data.positive) {
            setPosIsSet(true);
            setNegIsSet(false);
            setImpressionsPos(impressionsPos+1)
            setImpressionsNeg(impressionsNeg-1)
          }
          else if (!response.data.positive) {
            setNegIsSet(true);
            setPosIsSet(false);
            setImpressionsNeg(impressionsNeg+1)
            setImpressionsPos(impressionsPos-1)
          }
        } catch (error) {
            console.log(error);
        }
      }
      
    }
    
  }


  const handleInfo = (algoId) => {
    navigate('/algorithms/'+props.userId+'/'+algoId); 
  }
  const handleUpdate = (algoId) => {
    navigate('/algorithms/update/'+props.userId+'/'+algoId); 
  }

  const handleDelete = (algoId) => {
    navigate('/algorithms/delete/'+props.userId+'/'+algoId); 
  }


  const handleDeleteModal = async (event) => {
    event.preventDefault();
    const response = (await axios.delete(APIEndpoint + '/users/'+props.userId+'/customStatistics/'+props.algoId
      , {headers: {
        Authorization: BearerAuth(accessToken)
      }}));
    hideModal();
    props.onChange(props.algoId)
  }

  if (isLoading) return "Loading..."

  return (
    <div className='w-full'>        
        <div className='w-full border-b-2 flex flex-row justify-between'>
          <button onClick={() => handleInfo(props.algoId)}  className='btn hover:underline text-left text-xl font-bold pb-2'>{algorithm.name}</button>
          <div className='flex flex-row flex-nowrap items-center'>
            <button className='' onClick={() => handleImpression(true, false)}><img className='w-5' src={process.env.PUBLIC_URL + '/icons/thumbs-up.svg'}/></button> 
            <p className='px-1'>{impressionsPos}</p> 
            <button selected={negIsSet} onClick={() => handleImpression(false, false)}><img className='w-5' src={process.env.PUBLIC_URL + '/icons/thumbs-down.svg'}/></button>
            <p className='px-1'>{impressionsNeg}</p> 
          </div>
        </div>
        
        {!props.personal ?
        <div className='border-b-2 mt-2 flex flex-row'>
          <p className='pr-3'>Author: </p> 
          <a className='font-bold' href={'/user/'+props.userId}><UserGet userId={props.userId} noAlgorithms={true}/></a>
        </div>
          : null
        }

        {/* <p>{algorithm.formula && FormulaSecondHalf(algorithm.formula)}</p> */}
        <div className='border-b-2'>
          <p className='mt-2 font-bold'>Used stats include:</p>
          {algoStats.slice(0, 3).map((algoStat) => (
            <div key={algoStat * props.algoId}>
                <p>{props.sTypes[algoStat]}</p>
            </div>
          ))}
        </div>

        {props.personal && props.personal == true &&
          <div className='mt-1'>
            <Modal show={modalIsShowing} handleClose={handleDeleteModal}>
              <p>Modal</p>
            </Modal>
            <button onClick={() => handleUpdate(props.algoId)}><img className='sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-pencil-24.png'}/></button>
            <button onClick={() => handleDelete(props.algoId)}><img className='sepia hover:sepia-0 duration-75' src={process.env.PUBLIC_URL + '/icons/icons8-trash-24.png'}/></button>
          </div>
        }
    </div>
  );
}