import ErrorBoundary from "../../components/ErrorBoundary";
import FormView from "../../components/FormView";

const RoomForm = () => {
  return (
    <ErrorBoundary>
      <FormView apiUrl={"rooms"} schema={[{ name: "Name", type: "text" }]} />
    </ErrorBoundary>
  );
};
export default RoomForm;
