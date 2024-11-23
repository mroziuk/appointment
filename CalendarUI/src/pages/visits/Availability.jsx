import ListView from "../../components/ListView";

const Availability = () => {
  return (
    <div>
      <h1>Availability</h1>
      <ListView
        schema={{
          id: { type: "number" },
          userId: { type: "number", filter: "userId" },
          start: { type: "time", filter: "start" },
          end: { type: "time", filter: "end" },
          activeFrom: { type: "date", filter: "activeFrom" },
          activeTo: { type: "date", filter: "activeTo" },
        }}
        getUrl="/availability/all"
        deleteUrl="/availability"
        formUrl={"/availabilityForm"}
      />
    </div>
  );
};

export default Availability;
