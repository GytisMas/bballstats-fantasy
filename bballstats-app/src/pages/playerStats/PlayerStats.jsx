import PlayerStatsGet from "./PlayerStatsGet";
import {FormContainerStyle, FormMemberStyle, FormSumbitStyle} from '../../components/Helpers';
import { useParams } from "react-router-dom";

export default function PlayerStats() {
    let params = useParams();
    return (
        <div>
            <PlayerStatsGet teamId={params.teamId} playerId={params.playerId}/>
        </div>
    );
}