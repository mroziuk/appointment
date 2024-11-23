import { useState, useEffect } from "react";
import { TextField, Select, MenuItem, Button, Box } from "@mui/material";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import api from "../helpers/api";
import { useLocation, useNavigate } from "react-router-dom";
import PropTypes from "prop-types";
import dayjs from "dayjs";
import FallbackComponent from "./FallbackComponent";
import { ErrorBoundary } from "react-error-boundary";
import { Toaster, toast } from "sonner";
const SelectFromAPIField = ({ name, onChange, settings, value }) => {
  SelectFromAPIField.propTypes = {
    name: PropTypes.string.isRequired,
    onChange: PropTypes.func.isRequired,
    settings: PropTypes.shape({
      url: PropTypes.string.isRequired,
      valueName: PropTypes.string.isRequired,
      labelName: PropTypes.string.isRequired,
    }),
    value: PropTypes.string,
  };
  const [options, setOptions] = useState([]);
  const [error, setError] = useState(null);
  //if (error) throw new Error(error);
  useEffect(() => {
    const parseData = (data) => {
      return data.map((item) => ({
        value: item[settings.valueName],
        label: item[settings.labelName],
      }));
    };
    const fetchOptions = async () => {
      const response = await api(settings.url, {
        method: "GET",
        withCredentials: true,
      });
      if (!response.status === 200) {
        setError("Something went wrong!");
        toast.error(error);
      }
      setOptions(parseData(response.data.items));
    };
    fetchOptions();
  }, [settings]);
  return (
    <Select
      sx={{ width: "300px", margin: "5px 20px" }}
      label={name}
      value={value || ""}
      onChange={(event) => onChange(event.target.value)}
    >
      {options.map((option) => (
        <MenuItem key={option.value} value={option.value}>
          {option.label}
        </MenuItem>
      ))}
    </Select>
  );
};
const SelectField = ({ name, settings, onChange, value }) => {
  SelectField.propTypes = {
    name: PropTypes.string.isRequired,
    settings: PropTypes.shape({
      options: PropTypes.array.isRequired,
      valueName: PropTypes.string.isRequired,
      labelName: PropTypes.string.isRequired,
    }),
    onChange: PropTypes.func.isRequired,
    value: PropTypes.string,
  };
  return (
    <Select
      sx={{ width: "300px", margin: "5px 20px" }}
      label={name}
      value={value || ""}
      onChange={(event) => onChange(event.target.value)}
    >
      {settings.options.map((option) => (
        <MenuItem
          key={option[settings.valueName]}
          value={option[settings.valueName]}
        >
          {option[settings.labelName]}
        </MenuItem>
      ))}
    </Select>
  );
};
// eslint-disable-next-line
const FormView = ({ apiUrl, schema }) => {
  const [error, setError] = useState(null);
  if (error) {
    throw new Error(error);
  }
  const [formData, setFormData] = useState({});
  const location = useLocation();
  const queryParams = new URLSearchParams(location.search);
  const navigate = useNavigate();
  const id = queryParams.get("id");
  useEffect(() => {
    const fetchData = async () => {
      if (!id) {
        return;
      }
      try {
        const response = await api.get(`${apiUrl}/${id}`);
        const initData = response.data;
        // capitalize the first letter of each key
        const formattedData = Object.keys(initData).reduce((acc, key) => {
          const formattedKey = key.charAt(0).toUpperCase() + key.slice(1);
          acc[formattedKey] = initData[key];
          return acc;
        }, {});
        console.log(formattedData);
        setFormData(formattedData);
      } catch (error) {
        setError(error.data.message);
      }
    };

    fetchData();
  }, [id, apiUrl]);
  const handleFieldChange = (fieldName, value) => {
    setFormData((prevData) => ({
      ...prevData,
      [fieldName]: value,
    }));
  };
  const patchData = async (id, data) => {
    api
      .patch(`${apiUrl}/${id}`, data)
      .then((response) => {
        if (response.status !== 204) throw new Error("Something went wrong!");
        toast.success("Data updated successfully!");
      })
      .catch((error) => {
        toast.error(error.response.data.Message);
        setError(error.response.data.Message);
      });
  };
  const postData = async (data) => {
    api
      .post(`${apiUrl}`, data)
      .then((response) => {
        if (response.status !== 201) throw new Error("Something went wrong!");
        toast.success("Data created successfully!");
        navigate("/" + apiUrl);
      })
      .catch((error) => {
        toast.error(error.response.data.Message);
      });
  };
  const handleSubmit = async (event) => {
    event.preventDefault();
    id ? await patchData(id, formData) : await postData(formData);
  };
  return (
    <ErrorBoundary FallbackComponent={FallbackComponent}>
      <Toaster richColors />
      <form onSubmit={handleSubmit}>
        {/* eslint-disable-next-line */}
        {schema.map((field) => {
          const { name, type, options } = field;
          let fieldComponent;

          switch (type) {
            case "text":
            case "string":
              fieldComponent = (
                <TextField
                  label={name}
                  value={formData[name] || ""}
                  onChange={(event) =>
                    handleFieldChange(name, event.target.value)
                  }
                />
              );
              break;
            case "date":
              fieldComponent = (
                <LocalizationProvider
                  dateAdapter={AdapterDayjs}
                  adapterLocale="de"
                >
                  <DatePicker
                    sx={{ width: "300px", margin: "5px 20px" }}
                    label={name}
                    value={dayjs(formData[name]) || null}
                    onChange={(value) => handleFieldChange(name, value)}
                    renderInput={(params) => <TextField {...params} />}
                  />
                </LocalizationProvider>
              );
              break;
            case "datetime":
              fieldComponent = (
                <LocalizationProvider
                  dateAdapter={AdapterDayjs}
                  adapterLocale="de"
                >
                  <DatePicker
                    sx={{ width: "300px", margin: "5px 20px" }}
                    label={name}
                    value={dayjs(formData[name]) || null}
                    onChange={(value) => handleFieldChange(name, value)}
                    renderInput={(params) => <TextField {...params} />}
                    showTimeInput
                  />
                </LocalizationProvider>
              );
              break;
            case "selectAPI":
              fieldComponent = (
                <SelectFromAPIField
                  name={name}
                  onChange={(value) => handleFieldChange(name, value)}
                  settings={options}
                  value={formData[name] || formData[name + "Id"] || ""}
                />
              );
              break;
            case "select":
              fieldComponent = (
                <SelectField
                  name={name}
                  onChange={(value) => handleFieldChange(name, value)}
                  settings={options}
                  value={formData[name] || ""}
                />
              );
              break;
            case "number":
              fieldComponent = (
                <TextField
                  label={name}
                  type="number"
                  value={formData[name] || ""}
                  onChange={(event) =>
                    handleFieldChange(name, event.target.value)
                  }
                />
              );
              break;
            case "boolean":
              fieldComponent = (
                <div>
                  <label htmlFor={name}>{name}</label>
                  <input
                    type="checkbox"
                    id={name}
                    checked={formData[name] || false}
                    onChange={(event) =>
                      handleFieldChange(name, event.target.checked)
                    }
                  />
                </div>
              );
              break;
            case "time":
              fieldComponent = (
                <TextField
                  sx={{ width: "300px", margin: "5px 20px" }}
                  label={name}
                  type="time"
                  value={formData[name] || ""}
                  onChange={(event) =>
                    handleFieldChange(name, event.target.value)
                  }
                />
              );
              break;

            default:
              fieldComponent = null;
          }

          return <div key={name}>{fieldComponent}</div>;
        })}
        <Box
          sx={{
            width: "300px",
            display: "flex",
            justifyContent: "space-around",
            margin: "5px 20px",
          }}
        >
          <Button
            onClick={() => navigate("/" + apiUrl)}
            variant="contained"
            color="info"
          >
            Cancel
          </Button>
          <Button type="submit" variant="contained" color="secondary">
            Submit
          </Button>
        </Box>
      </form>
    </ErrorBoundary>
  );
};

export default FormView;
