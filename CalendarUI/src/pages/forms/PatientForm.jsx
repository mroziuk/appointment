import FormView from "../../components/FormView";

const PatientForm = () => {
  return (
    <FormView
      apiUrl={"patients"}
      schema={[
        { name: "FirstName", type: "text" },
        { name: "LastName", type: "text" },
        { name: "Phone", type: "text" },
        { name: "Email", type: "text" },
      ]}
    />
  );
};

export default PatientForm;
