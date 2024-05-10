import { useAuth } from "../../provider/Authentication";
import {BearerAuth, FormulaFirstHalf, FormulaSecondHalf} from '../../components/Helpers';
import { useParams } from "react-router-dom";
import axios from 'axios';
import { APIEndpoint } from "../../components/Helpers";
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from 'react';
import * as math from 'mathjs'

export default function AlgorithmFullPage(props) {
    let params = useParams();
    const navigate = useNavigate();
    const [user, setUser] = useState([]);
    const [algorithm, setAlgorithm] = useState([]);
    const [formula, setFormula] = useState([]);
    const [algoStats, setAlgoStats] = useState([]);
    const [playerEntries, setPlayerEntries] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isLoadingPl, setIsLoadingPl] = useState(true);
    const { accessToken, currentUserRoles } = useAuth();
    const [valueWarning, setValueWarning] = useState(false);

    useEffect(() => {
      const loadAlgoStats = async () => {
        const response = (await axios.get(APIEndpoint + '/users/' 
            + params.userId + '/customStatistics/' + params.algoId,
        ));
        setAlgorithm(response.data);
        const userResponse = await axios.get(APIEndpoint + '/users/' + params.userId);
        setUser(userResponse.data);
        
        const statIds = response.data.statIds;
        const statResponse = 
            (await axios.get(APIEndpoint + '/statistics/'))
                .data
                .filter((stat) => response.data.statIds.includes(stat.id))
                .sort(function(a, b){return statIds.indexOf(a.id) - statIds.indexOf(b.id)})
                .map((stat) => {return {id: stat.id, displayName: stat.displayName ?? stat.name}})
                ;
        setAlgoStats(statResponse)

        let formulaStatsArr = FormulaFirstHalf(response.data.formula)
            .split(' ')
            .map((elem) => Number(elem));
        const formulaLoc = FormulaSecondHalf(response.data.formula)
        setFormula(formulaLoc)

        setAlgoStats(statResponse);
        let playerList = []
        setIsLoading(false)
        const teams = (await axios.get(APIEndpoint + "/teams")).data;
        let hasInvalidValues = false;
        const allPlayerStats = (await axios.get(APIEndpoint + '/playerStatistics/', { params: {"statisticIds": statIds.toString()} }))
            .data
        for (let i = 0; i < teams.length; i++) {
            const players = (await axios.get(APIEndpoint + '/teams/'+teams[i].id+'/players')).data
            
            for (let j = 0; j < players.length; j++) {
                // const playerStats = (await axios.get(APIEndpoint + '/teams/'+teams[i].id+'/players/'+players[j].id+'/playerStatistics/', { params: {"statisticIds": statIds.toString()} }))
                //     .data
                const playerStats = allPlayerStats.filter(ps => ps.player == players[j].id)
                if (i == 0 && j == 0) {
                    console.log(allPlayerStats.filter(ps => ps.player == players[j].id))
                    console.log(playerStats)
                }
                const playerStatIds = playerStats.map((elem) => elem.statType);
                let hasAllStats = true
                let statIndexes = []
                for (let k = 0; k < formulaStatsArr.length; k++) {
                    const statIndex = playerStatIds.indexOf(formulaStatsArr[k])
                    if (statIndex !== -1) {
                        statIndexes.push(statIndex)
                    } else {            
                        hasAllStats = false;
                        break;
                    }
                }
                if (hasAllStats) {
                    let tempFormula = formulaLoc
                    for (let k = 0; k < formulaStatsArr.length; k++) {
                        tempFormula = tempFormula.replaceAll("s"+(k+1), playerStats[statIndexes[k]].value)
                    }
                    const score = math.evaluate(tempFormula)
                    if (!Number.isFinite(score)) {
                        hasInvalidValues = true;
                        continue;
                    }

                    playerList.push(
                        {
                            player: players[j].name,
                            team: teams[i].name,
                            rating: score
                        }
                    )
                }
            }
        }
        setValueWarning(hasInvalidValues);
        setPlayerEntries(playerList.sort(function(a, b){return b.rating - a.rating}))
        setIsLoadingPl(false)
      }

      const parsePlayerData = async () => {

      }

      loadAlgoStats();
    }, []);

    return (
        <div className='w-full flex flex-col items-center'>    
            {!isLoading ?    
                <div className='max-w-xl bg-white px-10 border-2 rounded-3xl flex flex-col items-center justify-center'>
                    <p className='text-center font-bold pt-6 pb-3'>{algorithm.name}</p>
                    <p className='text-center font-bold pt-3'>Author</p>
                    <p className='text-center pb-3'>{user.username}</p>
                    <p className='text-center font-bold pt-3'>Formula</p>
                    <p className='text-center pb-3'>{formula}</p>
                    <p className='text-center font-bold pt-3'>Uses these stats</p>
                    <div className="pb-6 w-full flex flex-col justify-start">
                    {algoStats.map((algoStat) => (
                        <p key={algoStat.id} className='text-center'>{algoStat.displayName}</p>
                    ))}
                    </div>
                </div>
                : <p>Loading...</p>
            }
            {!isLoadingPl ?
                <div className="max-w-6xl flex flex-col my-5 justify-center items-center bg-white px-10 border-2 rounded-3xl">
                    <div className="h-5 bg-slate-400"/>
                    {valueWarning && <p>Some calculated rating values were invalid: those players were skipped.</p>}
                    <div className="h-5 bg-slate-400"/>
                    <table>
                        <thead>
                            <tr>
                                <th className='p-2 border-2 text-left'>#</th>
                                <th className='p-2 border-2 text-left'>Player Name</th>
                                <th className='p-2 border-2 text-left'>Team Name</th>
                                <th className='p-2 border-2 text-left'>Rating</th>
                            </tr>
                        </thead>
                        <tbody>
                        {playerEntries.map((elem, i) => (
                        <tr key={elem.player}>
                            <td className='p-2 border-2 text-right'>{i+1}</td>
                            <td className='p-2 border-2 text-left'>{elem.player}</td>
                            <td className='p-2 border-2 text-left'>{elem.team}</td>
                            <td className='p-2 border-2 text-right'>{elem.rating.toFixed(2)}</td>
                        </tr>
                        ))}
                        </tbody>
                    </table>
                    <div className="h-5 bg-slate-400"/>
                </div>
                : <p>Loading...</p>
            }
            
        </div>
    );
}