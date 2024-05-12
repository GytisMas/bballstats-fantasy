export default function HeaderFields(props) {
    return <>
        <a href='/home' className={(props.footer ? "mx-2 text-md h-8 " : "text-xl font-bold h-16 ") + "duration-75 brightness-95 hover:brightness-100 text-white inline"} >
            <img className="h-full" src={process.env.PUBLIC_URL + '/icons/home.svg'}/>
        </a>
        <a href='/algorithms' className={(props.footer ? "mx-2 text-md " : "text-xl font-bold ") + "duration-75 brightness-95 hover:brightness-100 text-white inline"} >
            Algorithms
        </a>
        <a href='/players' className={(props.footer ? "mx-2 text-md " : "text-xl font-bold ") + "duration-75 brightness-95 hover:brightness-100 text-white inline"} >
            Players
        </a>
        <a href='/fantasy/leagues' className={(props.footer ? "mx-2 text-md " : "text-xl font-bold ") + "duration-75 brightness-95 hover:brightness-100 text-white inline"} >
            Fantasy Leagues
        </a>
        {props.roles &&
        <a href='/profile' className={(props.footer ? "mx-2 text-md " : "text-xl font-bold ") + "duration-75 brightness-95 hover:brightness-100 text-white inline"} >
            Profile
        </a>
        }
        
        
        {props.roles && props.roles.includes('Moderator') &&
            <a href='/stats' className={(props.footer ? "mx-2 text-md " : "text-xl font-bold ") + "duration-75 brightness-95 hover:brightness-100 text-white inline"} >
                Stats
            </a>
        }

        {props.roles && props.roles.includes('Admin') &&
            <a href='/users' className={(props.footer ? "mx-2 text-md " : "text-xl font-bold ") + "duration-75 brightness-95 hover:brightness-100 text-white inline"} >
                Users
            </a>
        }
        
        {props.roles &&
            <a href='/logout' className={(props.footer ? "mx-2 text-md " : "text-xl font-bold ") + "duration-75 brightness-95 hover:brightness-100 text-white inline"} >
                Logout
            </a>
        }
        {!props.roles &&
            <a href='/login' className={(props.footer ? "mx-2 text-md " : "text-xl font-bold ") + "duration-75 brightness-95 hover:brightness-100 text-white inline"} >
                Login
            </a>
        }
        {!props.roles &&
            <a href='/register' className={(props.footer ? "mx-2 text-md " : "text-xl font-bold ") + "duration-75 brightness-95 hover:brightness-100 text-white inline"} >
                Register
            </a>
        }
    </>
}