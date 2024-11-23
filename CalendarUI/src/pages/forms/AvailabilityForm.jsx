import FormView from "../../components/FormView";

const AvailabilityForm = () => {
  const daysOfWeek = [
    { name: "Monday", value: 0 },
    { name: "Tuesday", value: 1 },
    { name: "Wednesday", value: 2 },
    { name: "Thursday", value: 3 },
    { name: "Friday", value: 4 },
    { name: "Saturday", value: 5 },
    { name: "Sunday", value: 6 },
  ];
  return (
    <FormView
      getUrl="/availabilities/all"
      deleteUrl="/availabilities"
      formUrl="/availability"
      apiUrl={"availability"}
      schema={[
        {
          name: "UserId",
          type: "selectAPI",
          options: { url: "/users/all", valueName: "id", labelName: "email" },
        },
        {
          name: "DayOfWeek",
          type: "select",
          options: {
            options: daysOfWeek,
            valueName: "value",
            labelName: "name",
            value: 3,
          },
        },
        { name: "Start", type: "time" },
        { name: "End", type: "time" },
        { name: "ActiveFrom", type: "date" },
        { name: "ActiveTo", type: "date" },
      ]}
    />
  );
};
export default AvailabilityForm;
