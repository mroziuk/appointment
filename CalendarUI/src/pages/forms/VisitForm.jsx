import FormView from "../../components/FormView";

const VisitForm = () => {
  return (
    <FormView
      apiUrl={"visits"}
      schema={[
        {
          name: "Patient",
          type: "selectAPI",
          options: {
            url: "/patients/all",
            valueName: "id",
            labelName: "firstName",
          },
        },
        {
          name: "Room",
          type: "selectAPI",
          options: { url: "/rooms/all", valueName: "id", labelName: "name" },
        },
        {
          name: "Therapist",
          type: "selectAPI",
          options: { url: "/users/all", valueName: "id", labelName: "email" },
        },
        { name: "StartDate", type: "datetime" },
        { name: "EndDate", type: "datetime" },
        { name: "IsFirstVisit", type: "boolean" },
        { name: "IsRecurring", type: "boolean" },
      ]}
    />
  );
};
export default VisitForm;
