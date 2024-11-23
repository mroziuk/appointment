import { useEffect } from "react";
const globalTitle = "Appointment";
const useTitle = (title) => {
  useEffect(() => {
    document.title = `${globalTitle} - ${title}`;
  }, [title]);
};

export default useTitle;
