import '../App.css';
import HeaderFields from './HeaderFields';


export default function Footer(props) {
    return <header className='drop-shadow-lg Header p-5 max-h-fit flex-col md:flex-row  flex-nowrap flex w-full items-center justify-center border-black border-b-2'>
        <HeaderFields roles={props.roles} footer={true}/>
    </header>;
}