import { useNavigate } from "react-router-dom";

const Home = () => {
  const navigate = useNavigate();

  const onButtonClick = () => {
    navigate("/login");
  };

  return (
    <div className="mainContainer login-main">
      <div className={"titleContainer"}>
        <h1>Welcome!</h1>
      </div>
      <div>This is the home page.</div>
      <div className={"buttonContainer"}>
        <input
          className={"inputButton"}
          type="button"
          onClick={onButtonClick}
          value="Log in"
        />
      </div>
    </div>
  );
};

export default Home;
