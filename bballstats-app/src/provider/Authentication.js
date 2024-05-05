import axios from "axios";
import { createContext, useContext, useEffect, useMemo, useState } from "react";

const AuthContext = createContext();

const AuthProvider = ({ children }) => {
  // State to hold the authentication token
  const [accessToken, setAccessToken] = useState(localStorage.getItem("accessToken"));
  const [refreshToken, setRefreshToken] = useState(localStorage.getItem("refreshToken"));
  const [currentUserRoles, setCurrentUserRoles] = useState(JSON.parse(localStorage.getItem("currRoles") || null));

  // Function to set the authentication token
  const setTokens = (newAToken, newRToken, newRoles) => {
    setAccessToken(newAToken);
    setRefreshToken(newRToken);
    setCurrentUserRoles(JSON.stringify(newRoles));
  };

  useEffect(() => {
    let none = true
    if (accessToken) {
      axios.defaults.headers.common["Authorization"] = "Bearer " + accessToken;
      localStorage.setItem('accessToken', accessToken);
      none = false;
    } 
    if (refreshToken) {
      localStorage.setItem('refreshToken', refreshToken);
      none = false;
    } 
    if (currentUserRoles) {
      localStorage.setItem('currRoles', JSON.stringify(currentUserRoles));
      none = false;
    } 
    if (none) {
      delete axios.defaults.headers.common["Authorization"];
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('currRoles')
    }
  }, [accessToken, refreshToken, currentUserRoles]);

  // Memoized value of the authentication context
  const contextValue = useMemo(
    () => ({
      accessToken: accessToken,
      refreshToken: refreshToken,
      currentUserRoles: currentUserRoles,
      setTokens: setTokens,
    }),
    [accessToken, refreshToken, currentUserRoles]
  );

  // Provide the authentication context to the children components
  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};

export const useAuth = () => {
  return useContext(AuthContext);
};

export default AuthProvider;