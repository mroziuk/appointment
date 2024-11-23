import { useNavigate } from "react-router-dom";
import { useAuth } from "../../provider/authProvider.jsx";

const Logout = () => {
  const { setToken } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await setToken();
    navigate("/", { replace: true });
  };

  handleLogout();
};

export default Logout;