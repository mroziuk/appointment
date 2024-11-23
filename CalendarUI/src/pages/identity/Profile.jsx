import FormView from "../../components/FormView";

const Profile = () => {
  return (
    <div>
      <FormView
        apiUrl="users"
        schema={[
          { name: "FirstName", type: "text" },
          { name: "LastName", type: "text" },
          { name: "Email", type: "text" },
          { name: "Role", type: "text" },
        ]}
      />
    </div>
  );
};
export default Profile;
