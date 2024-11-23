import { useEffect, useState } from "react";
import { Sidebar as ProSidebar, Menu, MenuItem } from "react-pro-sidebar";
import { Box, IconButton, Typography, useTheme } from "@mui/material";
import { Link } from "react-router-dom";
import { tokens } from "../theme";
import HomeOutlinedIcon from "@mui/icons-material/HomeOutlined";
import PeopleOutlinedIcon from "@mui/icons-material/PeopleOutlined";
import ContactsOutlinedIcon from "@mui/icons-material/ContactsOutlined";
import ReceiptOutlinedIcon from "@mui/icons-material/ReceiptOutlined";
import CalendarTodayOutlinedIcon from "@mui/icons-material/CalendarTodayOutlined";
import DoorBackOutlinedIcon from "@mui/icons-material/DoorBackOutlined";
import WorkHistoryIcon from "@mui/icons-material/WorkHistory";
import MenuOutlinedIcon from "@mui/icons-material/MenuOutlined";
import { getUserMe } from "../helpers/users";

// eslint-disable-next-line react/prop-types
const Item = ({ title, to, icon, selected, setSelected }) => {
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  return (
    <MenuItem
      component={<Link to={to} />}
      active={selected === title}
      style={{ color: colors.grey[100] }}
      onClick={() => setSelected(title)}
      icon={icon}
    >
      <Typography>{title}</Typography>
    </MenuItem>
  );
};

const Sidebar = () => {
  const [userData, setUserData] = useState(null);

  useEffect(() => {
    getUserMe().then((data) => {
      setUserData(data);
    });
  }, []);

  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  const [collapsed, setCollapsed] = useState(false);
  const [selected, setSelected] = useState("Dashboard");
  return (
    <Box
      sx={{
        "& .ps-sidebar-container": {
          background: `${colors.primary[400]} !important`,
        },
        "& .ps-menuitem-root": {
          backgroundColor: "transparent !important",
        },
        "& .ps-menu-button": {
          padding: "5px 35px 5px 20px !important",
          background: `${colors.primary[400]} !important`,
        },
        "& .ps-menu-button:hover": {
          color: "#868dfb !important",
        },
        "& .ps-menu-button.active": {
          color: "#6870fa !important",
        },
      }}
    >
      <ProSidebar collapsed={collapsed}>
        <Menu iconShape="square">
          {/* logo and menu icon */}
          <MenuItem
            onClick={() => setCollapsed(!collapsed)}
            icon={collapsed ? <MenuOutlinedIcon /> : undefined}
            style={{ margin: "10px 0 20px 0", color: colors.grey[100] }}
          >
            {!collapsed && (
              <Box
                display="flex"
                flexDirection="row"
                justifyContent="space-between"
                alignItems="center"
                ml="10px"
              >
                <Typography variant="h3" color={colors.grey[100]}>
                  APPOINTMENT
                </Typography>
                <IconButton onClick={() => setCollapsed(!collapsed)}>
                  <MenuOutlinedIcon />
                </IconButton>
              </Box>
            )}
          </MenuItem>
          {/* user */}
          {!collapsed && (
            <Box mb="25px">
              <Box display="flex" justifyContent="center" alignItems="center">
                <img
                  alt="profile-user"
                  width="100px"
                  height="100px"
                  src="images/user.png"
                  style={{
                    cursor: "pointer",
                    borderRadius: "50%",
                    objectFit: "cover",
                  }}
                />
              </Box>
              <Box textAlign="center">
                <Typography
                  variant="h2"
                  color={colors.grey[100]}
                  fontWeight="bold"
                  sx={{ m: "10px 0px 0px 0px" }}
                >
                  {userData && userData.firstName + " " + userData.lastName}
                </Typography>
                <Typography variant="h5" color={colors.greenAccent[500]}>
                  {userData && userData.email}
                </Typography>
              </Box>
            </Box>
          )}
          {/* Menu items */}
          <Box paddingLeft={collapsed ? undefined : "10%"}></Box>
          <Item
            title="Dashboard"
            to="/"
            icon={<HomeOutlinedIcon />}
            selected={selected}
            setSelected={setSelected}
          ></Item>
          <Typography
            variant="h6"
            color={colors.grey[300]}
            sx={{ m: "15px 0 5px 20px" }}
          >
            Lists
          </Typography>
          <Item
            title="Users"
            to="/users"
            icon={<PeopleOutlinedIcon />}
            selected={selected}
            setSelected={setSelected}
          ></Item>
          <Item
            title="Availabilities"
            to="/availability"
            icon={<WorkHistoryIcon />}
            selected={selected}
            setSelected={setSelected}
          />
          <Item
            title="Patients"
            to="/patients"
            icon={<ContactsOutlinedIcon />}
            selected={selected}
            setSelected={setSelected}
          ></Item>
          <Item
            title="Visits"
            to="/visits"
            icon={<ReceiptOutlinedIcon />}
            selected={selected}
            setSelected={setSelected}
          ></Item>
          <Item
            title="Rooms"
            to="/rooms"
            icon={<DoorBackOutlinedIcon />}
            selected={selected}
            setSelected={setSelected}
          ></Item>
          <Typography
            variant="h6"
            color={colors.grey[300]}
            sx={{ m: "15px 0 5px 20px" }}
          >
            Pages
          </Typography>
          <Item
            title="Calendar"
            to="/calendar"
            icon={<CalendarTodayOutlinedIcon />}
            selected={selected}
            setSelected={setSelected}
          ></Item>
        </Menu>
      </ProSidebar>
    </Box>
  );
};
export default Sidebar;
