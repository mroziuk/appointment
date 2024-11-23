import axios from 'axios';
import { useAuth } from "../../provider/authProvider.jsx";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { baseUrl } from "../../helpers/api.js";
import useTitle from "../../components/hooks/useTitle.js";
const Login = () => {
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [SignInDto, setSignInDto] = useState({ login: "", password: "" });
  const { setToken } = useAuth();
  const navigate = useNavigate();
  useTitle("Login");
  const handleChange = (e) => {
    setSignInDto({
      ...SignInDto,
      [e.target.name]: e.target.value,
    });
  };

  const onButtonClick = async (e) => {
    e.preventDefault();
    setEmailError("");
    setPasswordError("");

    if ("" === SignInDto.login) {
      setEmailError("Please enter your email");
      return;
    }

    if (!/^[\w-.]+@([\w-]+\.)+[\w-]{2,5}$/.test(SignInDto.login)) {
      setEmailError("Please enter a valid email");
      return;
    }

    if ("" === SignInDto.password) {
      setPasswordError("Please enter a password");
      return;
    }
    if (SignInDto.password.length < 7) {
      setPasswordError("The password must be 8 characters or longer");
      return;
    }

    try {
      const response = await axios({
        method: "post",
        url: baseUrl + "/identity/login",
        data: SignInDto,
        withCredentials: true,
      });
      setToken(response.data);
      navigate("/", { replace: true });
    } catch (error) {
      console.error("Error logging in", error);
    }
  };

  return (
    <div className={"loginContainer"}>
      <div className={"titleContainer"}>
        <div>Login</div>
      </div>
      <br />
      <div className={"inputContainer"}>
        <input
          value={SignInDto.login}
          type="login"
          name="login"
          placeholder="Enter your email here"
          onChange={handleChange}
          className={"inputBox"}
        />
        <label className="errorLabel">{emailError}</label>
      </div>
      <br />
      <div className={"inputContainer"}>
        <input
          value={SignInDto.password}
          type="password"
          name="password"
          placeholder="Enter your password here"
          onChange={handleChange}
          className={"inputBox"}
        />
        <label className="errorLabel">{passwordError}</label>
      </div>
      <br />
      <div className={"inputContainer"}>
        <input
          className={"inputButton"}
          type="button"
          onClick={onButtonClick}
          value={"Log in"}
        />
      </div>
    </div>
  );
};

export default Login;
