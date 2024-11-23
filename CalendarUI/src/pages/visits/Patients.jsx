import ListView from "../../components/ListView";

const Patients = () => {
  return (
    <div>
      <h1>Patients</h1>
      <ListView
        schema={{
          id: { type: "number" },
          firstName: { type: "string", filter: "firstName" },
          lastName: { type: "string", filter: "lastName" },
          phone: { type: "string", filter: "phone" },
          email: { type: "string", filter: "email" },
        }}
        getUrl="/patients/all"
        deleteUrl="/patients"
        formUrl="/patient"
      />
    </div>
  );
};

export default Patients;
