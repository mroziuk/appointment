import ListView from "../../components/ListView";

const Rooms = () => {
  return (
    <div>
      <h1>Rooms</h1>
      <ListView
        getUrl="/rooms/all"
        deleteUrl="/rooms"
        formUrl="/room"
        schema={{
          id: { type: "number" },
          name: { type: "string", filter: "name" },
        }}
      />
    </div>
  );
};
export default Rooms;
