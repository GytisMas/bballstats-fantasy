import AuthProvider from "./provider/Authentication";
import Routes from "./routes";
import axios from 'axios';

function App() {
  axios.defaults.withCredentials = true;
  return (
    <AuthProvider>
      <Routes />
    </AuthProvider>
  );
}

export default App;