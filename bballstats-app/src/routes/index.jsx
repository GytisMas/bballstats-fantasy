import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { useAuth } from "../provider/Authentication";
import { ProtectedRoute } from "./ProtectedRoute";
import Home from "../pages/Home";
import Stats from "../pages/statistics/Stats";
import Logout from "../pages/auth/Logout";
import Login from "../pages/auth/Login";
import UsersList from "../pages/users/UsersList";
import UserGet from "../pages/users/UserGet";
import PlayersList from "../pages/players/PlayersList";
import PlayerCreate from "../pages/players/PlayerCreate";
import PlayerUpdate from "../pages/players/PlayerUpdate";
import PlayerDelete from "../pages/players/PlayerDelete";
import AlgorithmsList from "../pages/ratingAlgorithms/AlgorithmsList";
import AlgorithmCreate from "../pages/ratingAlgorithms/AlgorithmCreate";
import AlgorithmUpdate from "../pages/ratingAlgorithms/AlgorithmUpdate";
import { RegularRoute } from "./RegularRoute";
import UserUpdate from "../pages/users/UserUpdate";
import UserDelete from "../pages/users/UserDelete";
import PlayerStats from "../pages/playerStats/PlayerStats";
import PlayerStatsCreate from "../pages/playerStats/PlayerStatsCreate";
import PlayerStatsUpdate from "../pages/playerStats/PlayerStatsUpdate";
import PlayerStatsDelete from "../pages/playerStats/PlayerStatsDelete";
import UserCreate from "../pages/users/UserCreate";
import { jwtDecode } from "jwt-decode";
import isTokenInvalid from "../components/Helpers";
import AlgorithmDelete from "../pages/ratingAlgorithms/AlgorithmDelete";
import StatisticCreate from "../pages/statistics/StatisticCreate";
import StatisticUpdate from "../pages/statistics/StatisticUpdate";
import StatisticDelete from "../pages/statistics/StatisticDelete";
import Register from "../pages/auth/Register";
import ChangePassword from "../pages/auth/ChangePassword";
import Dashboard from "../components/Dashboard";
import NotFound from "../components/NotFound";
import AlgorithmFullPage from "../pages/ratingAlgorithms/AlgorithmFullPage";
import FantasyTemplateCreate from "../pages/fantasy/FantasyTemplateCreate";
import LeagueCreate from "../pages/fantasy/LeagueCreate";
import LeagueParticipate from "../pages/fantasy/LeagueParticipate";
import Leagues from "../pages/fantasy/Leagues";
import LeagueGet from "../pages/fantasy/LeagueGet";
import LeagueParticipantGet from "../pages/fantasy/LeagueParticipantGet";
import FantasyTemplateView from "../pages/fantasy/FantasyTemplateView";

const Routes = () => {
  const { accessToken } = useAuth();
  const { refreshToken } = useAuth();
  const { currentUserRoles } = useAuth();

  // Define routes accessible only to authenticated users
  const routesForAuthenticatedOnly = [
    {
      path: "/",
      element: <ProtectedRoute />,
      children: [
        {
          path: "/",
          element: <AlgorithmsList/>,
        },
        {
          path: "/user/:userId",
          element: <UserGet personal={false} />,
        },
        {
          path: "/algorithms",
          element: <AlgorithmsList />,
        },
        {
          path: "/players",
          element: <PlayersList isModerator={currentUserRoles ? currentUserRoles.includes('Moderator') : false} />,
        },
        {
          path: "/fantasy/leagues",
          element: <Leagues/>,
        },
        {
          path: "/fantasy/leagues/:leagueId",
          element: <LeagueGet/>,
        },
        {
          path: "/fantasy/leagues/:leagueId/participants/:participantId",
          element: <LeagueParticipantGet/>,
        },
        {
          path: "/home",
          element: <Leagues/>,
        },
        {
          path: "/stats",
          element: <Stats isCurator={currentUserRoles ? currentUserRoles.includes('Moderator') : false}/>,
        },
        {
          path: "/profile",
          element: <UserGet userId={accessToken && jwtDecode(accessToken).sub} personal={true} />,
        },
        {
          path: "/stats/create",
          element: <StatisticCreate />,
        },
        {
          path: "/stats/update/:statId",
          element: <StatisticUpdate />,
        },
        {
          path: "/stats/delete/:statId",
          element: <StatisticDelete />,
        },
        {
          path: "/users",
          element: <UsersList isAdmin={currentUserRoles ? currentUserRoles.includes('Admin') : false}/>,
        },
        {
          path: "/users/create",
          element: <UserCreate />,
        },
        {
          path: "/users/update/:userId",
          element: <UserUpdate />,
        },
        {
          path: "/users/delete/:userId",
          element: <UserDelete />,
        },
        {
          path: "/players/create",
          element: <PlayerCreate />,
        },
        {
          path: "/players/statCreate/:teamId/:playerId",
          element: <PlayerStatsCreate />,
        },
        {
          path: "/players/statUpdate/:teamId/:playerId/:statId",
          element: <PlayerStatsUpdate />,
        },
        {
          path: "/players/statDelete/:teamId/:playerId/:statId",
          element: <PlayerStatsDelete />,
        },
        {
          path: "/algorithms/:userId/:algoId",
          element: <AlgorithmFullPage />,
        },
        {
          path: "/algorithms/create",
          element: <AlgorithmCreate />,
        },
        {
          path: "/algorithms/update/:userId/:algoId",
          element: <AlgorithmUpdate />,
        },
        {
          path: "/algorithms/delete/:userId/:algoId",
          element: <AlgorithmDelete />,
        },
        {
          path: "/players/:teamId/:playerId",
          element: <PlayerStats />,
        },
        {
          path: "/players/update/:teamId/:playerId",
          element: <PlayerUpdate />,
        },
        {
          path: "/players/delete/:teamId/:playerId",
          element: <PlayerDelete />,
        },
        {
          path: "/logout",
          element: <Logout/>,
        },
        {
          path: "/changePassword",
          element: <ChangePassword/>,
        },
        {
          path: "/fantasy/templates/create",
          element: <FantasyTemplateCreate />,
        },
        {
          path: "/fantasy/templates/:templateId",
          element: <FantasyTemplateView />,
        },
        {
          path: "/fantasy/leagues/create",
          element: <LeagueCreate />,
        },
        {
          path: "/fantasy/leagues/:leagueId/participate/",
          element: <LeagueParticipate />,
        },
        {
          path: "/fantasy/leagues/:leagueId/participate/:participantId",
          element: <LeagueParticipate />,
        },
      ],
    },
  ];

  // Define routes accessible only to non-authenticated users
  const routesForNotAuthenticatedOnly = [
    {
      path: "/",
      element: <RegularRoute />,
      children: [
        {
          path: "/",
          element: <Login/>,
        },
        {
          path: "/login",
          element: <Login/>,
        },
        {
          path: "/register",
          element: <Register/>,
        },
      ],
    }    
  ];

  // Combine and conditionally include routes based on authentication status
  const router = createBrowserRouter([
    ...(isTokenInvalid(refreshToken) ? routesForNotAuthenticatedOnly : []),
    ...routesForAuthenticatedOnly,
    {
      path: "*",
      element: <RegularRoute />,
      children: [
        {
          path: "*",
          element: <NotFound/>,
        },
      ]
    }
  ]);

  // Provide the router configuration using RouterProvider
  return <RouterProvider router={router} />;
};

export default Routes;