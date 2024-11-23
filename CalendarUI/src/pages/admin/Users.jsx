import { Box, Typography, useTheme } from "@mui/material";
import { tokens } from "../../theme";
import { DataGrid } from "@mui/x-data-grid";
import { useState, useEffect } from "react";
import AdminPanelSettingsOutlinedIcon from "@mui/icons-material/AdminPanelSettingsOutlined";
import LockOpenOutlinedIcon from "@mui/icons-material/LockOpenOutlined";
import SecurityOutlinedIcon from "@mui/icons-material/SecurityOutlined";
import Header from "../../components/Header";
import api from "../../helpers/api";

const Users = () => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  // fetching data from api
  const [isFetching, setIsFetching] = useState(false);
  const [userData, setUserData] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchUsers() {
      setIsFetching(true);
      try {
        const response = await api("/users/all", {
          method: "GET",
          withCredentials: true,
        });
        const data = response.data;
        setUserData(data.items);
        if (!response.ok) {
          throw new Error(data.message);
        }
      } catch (error) {
        setError(error);
      }
      setIsFetching(false);
    }
    fetchUsers();
  }, []);

  const columns = [
    { field: "id", headerName: "ID", flex: 0.5 },
    {
      field: "firstName",
      headerName: "First Name",
      flex: 1,
      cellClassName: "name-column",
    },
    { field: "lastName", headerName: "Last Name", flex: 1 },
    { field: "email", headerName: "Email", flex: 1 },
    {
      field: "Role",
      headerName: "Role",
      flex: 1,
      renderCell: ({ row: { role } }) => {
        return (
          <Box
            width="60%"
            m="0 auto"
            p="5px"
            display="flex"
            justifyContent="space-between"
            backgroundColor={
              role === "superadmin" || role === "admin"
                ? colors.greenAccent[600]
                : colors.greenAccent[700]
            }
            borderRadius="5px"
          >
            {role === "superadmin" && <AdminPanelSettingsOutlinedIcon />}
            {role === "admin" && <SecurityOutlinedIcon />}
            {role === "user" && <LockOpenOutlinedIcon />}
            <Typography color={colors.grey[100]} sx={{ ml: "5px" }}>
              {role}
            </Typography>
          </Box>
        );
      },
    },
  ];

  return (
    <Box m="20px">
      <Header title="Team" description="Manage your team" />
      <Box
        m="40px 0 0 0"
        height="75vh"
        sx={{
          "& .MuiDataGrid-root": { border: "none" },
          "& .MuiDataGrid-cell": { borderBottom: "none" },
          "& .name-column": {
            color: colors.greenAccent[300],
          },
        }}
      >
        <DataGrid
          rows={userData}
          columns={columns}
          loading={isFetching}
          error={error}
          autoHeight
          disableColumnMenu
          disableSelectionOnClick
        />
      </Box>
    </Box>
  );
};

export default Users;