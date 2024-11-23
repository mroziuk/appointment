/* eslint-disable react/prop-types */
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { useAuth } from "../provider/authProvider.jsx";
import { ProtectedRoute } from "./ProtectedRoute";
import Logout from "../pages/identity/Logout.jsx";
import Login from "../pages/identity/Login.jsx";
import Users from "../pages/admin/Users.jsx";
import Settings from "../pages/admin/Settings.jsx";
import Profile from "../pages/identity/Profile.jsx";
import Calendar from "../pages/visits/Calendar.jsx";
import Register from "../pages/identity/Register.jsx";
import Rooms from "../pages/visits/Rooms.jsx";
import Visits from "../pages/visits/Visits.jsx";
import Patients from "../pages/visits/Patients.jsx";
import Forgot from "../pages/identity/Forgot.jsx";
import Home from "../pages/Home.jsx";
import Availability from "../pages/visits/Availability.jsx";
import PatientForm from "../pages/forms/PatientForm.jsx";
import RoomForm from "../pages/forms/RoomForm.jsx";
import VisitForm from "../pages/forms/VisitForm.jsx";
import AvailabilityForm from "../pages/forms/AvailabilityForm.jsx";
const Routes = () => {
  const { token } = useAuth();
  const routesForAuthenticatedOnly = [
    {
      path: "/",
      element: <ProtectedRoute />,
      children: [
        { path: "/", element: <Calendar /> },
        { path: "/calendar", element: <Calendar /> },
        // views
        { path: "/visits", element: <Visits /> },
        { path: "/availability", element: <Availability /> },
        { path: "/rooms", element: <Rooms /> },
        { path: "/patients", element: <Patients /> },
        { path: "/users", element: <Users /> },
        // admin
        { path: "/settings", element: <Settings /> },
        { path: "/profile", element: <Profile /> },
        { path: "/logout", element: <Logout /> },
        // form pages
        { path: "/patient", element: <PatientForm /> },
        { path: "/room", element: <RoomForm /> },
        { path: "/visit", element: <VisitForm /> },
        { path: "/availabilityForm", element: <AvailabilityForm /> },
      ],
    },
  ];
  const routesForNotAuthenticatedOnly = [
    { path: "/", element: <Home /> },
    { path: "/login", element: <Login /> },
    { path: "/register", element: <Register /> },
    { path: "/Forgot", element: <Forgot /> },
  ];
  const router = createBrowserRouter([
    ...(!token ? routesForNotAuthenticatedOnly : []),
    ...routesForAuthenticatedOnly,
  ]);
  return <RouterProvider router={router}></RouterProvider>;
};

export default Routes;
