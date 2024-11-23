import PropTypes from "prop-types";
const FallbackComponent = ({ error, resetErrorBoundary }) => {
  FallbackComponent.propTypes = {
    error: PropTypes.object.isRequired,
    resetErrorBoundary: PropTypes.func.isRequired,
  };
  console.log(error);

  return (
    <div role="alert">
      <p>Something went wrong:</p>
      <pre>{error.message}</pre>
      <button onClick={resetErrorBoundary}>Try again</button>
    </div>
  );
};
export default FallbackComponent;
