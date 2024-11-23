import ListView from "../../components/ListView";

const Visits = () => {
  return (
    <div>
      <h1>Visits</h1>
      <ListView
        getUrl="/visits/all"
        deleteUrl="/visits"
        formUrl="/visit"
        schema={{
          id: { type: "number" },
          startDate: { type: "string" },
          endDate: { type: "string" },
        }}
      />
    </div>
  );
};
export default Visits;
