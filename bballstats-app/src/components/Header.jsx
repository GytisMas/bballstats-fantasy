import '../App.css';
import HeaderFields from './HeaderFields';


export default function Header(props) {
    return <header className='drop-shadow-lg Header fixed flex-row flex w-full items-center justify-evenly border-black border-b-2'>
        <HeaderFields roles={props.roles}/>
    </header>;
}