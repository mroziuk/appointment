/* eslint-disable react/prop-types */
import api from "../helpers/api";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import {
  Card,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  TablePagination,
  Button,
} from "@mui/material";
const ListView = ({ schema, getUrl, deleteUrl, formUrl }) => {
  const [error, setError] = useState(null);
  if (error) {
    throw new Error(error);
  }
  let navigate = useNavigate();
  const [data, setData] = useState([]);
  const [count, setCount] = useState(0);
  const [controller, setController] = useState({
    page: 0,
    pageSize: 10,
  });
  const [filters, setFilters] = useState({});
  useEffect(() => {
    const fetchData = async (page, size) => {
      try {
        const keys = Object.keys(schema);
        var query = "";
        keys.forEach((k) => {
          if (schema[k].filter && filters[k]) {
            query += `&${schema[k].filter}=${filters[k]}`;
          }
        });
        const response = await api(
          `${getUrl}?page=${page + 1}&pageSize=${size}${query}`,
          {
            method: "GET",
            withCredentials: true,
          }
        );
        setData(response.data.items);
        setCount(response.data.totalCount);
      } catch (error) {
        setError(error.data.message);
      }
    };
    fetchData(controller.page, controller.pageSize);
  }, [controller, getUrl, filters, schema]);

  const handlePageChange = (event, newPage) => {
    setController({
      ...controller,
      page: newPage,
    });
  };

  const handleChangeRowsPerPage = (event) => {
    setController({
      ...controller,
      rowsPerPage: parseInt(event.target.value, 10),
      page: 0,
    });
  };

  const renderHeader = () => {
    return (
      <TableRow>
        {Object.keys(schema).map((key, i) => (
          <TableCell key={i}>
            {key}
            {schema[key].filter && (
              <input
                type="text"
                placeholder={`Search ${key}`}
                onChange={(e) => {
                  setFilters({
                    ...filters,
                    [key]: e.target.value,
                  });
                }}
              />
            )}
          </TableCell>
        ))}
        <TableCell></TableCell>
        <TableCell></TableCell>
      </TableRow>
    );
  };

  const handleDelete = async (item) => {
    const confirmed = window.confirm(
      "Are you sure you want to delete this item?"
    );
    if (confirmed) {
      try {
        await api(`${deleteUrl}/${item.id}`, {
          method: "DELETE",
          withCredentials: true,
        });
        setController({
          ...controller,
          page: 0,
        });
      } catch (error) {
        setError(error.data.message);
      }
    }
  };
  const handleEdit = (item) => {
    navigate(`${formUrl}?id=${item.id}/`);
  };

  const renderItem = (item) => {
    return (
      <TableRow key={item.id}>
        {Object.keys(schema).map((key, i) => (
          <TableCell key={i}>{item[key]}</TableCell>
        ))}
        <TableCell>
          <Button variant="contained" onClick={() => handleDelete(item)}>
            Delete
          </Button>
        </TableCell>
        <TableCell>
          <Button variant="contained" onClick={() => handleEdit(item)}>
            Edit
          </Button>
        </TableCell>
      </TableRow>
    );
  };

  return (
    <div className="table-container">
      <Card>
        <Table>
          <TableHead>{renderHeader()}</TableHead>
          <TableBody>
            {data.length === 0 && (
              <TableRow>
                <TableCell
                  align="center"
                  colSpan={Object.keys(schema).length}
                  key={"no-rows"}
                >
                  No data found
                </TableCell>
              </TableRow>
            )}
            {data.map((item) => renderItem(item))}
          </TableBody>
        </Table>
        <TablePagination
          component="div"
          onPageChange={handlePageChange}
          page={controller.page}
          count={count}
          rowsPerPage={controller.pageSize}
          onRowsPerPageChange={handleChangeRowsPerPage}
        />
      </Card>
      <Button
        color="secondary"
        variant="contained"
        onClick={() => {
          navigate(formUrl);
        }}
      >
        Add
      </Button>
    </div>
  );
};

export default ListView;
