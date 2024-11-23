import { Typography, Box, useTheme } from "@mui/material";
import { tokens } from "../theme";
import PropTypes from "prop-types";

const Header = ({ title, description }) => {
  Header.propTypes = {
    title: PropTypes.string.isRequired,
    description: PropTypes.string.isRequired,
  };
  const theme = useTheme();
  const colors = tokens(theme.palette.mode);
  return (
    <Box mb="30px">
      <Typography
        variant="h2"
        color={colors.grey[100]}
        fontWeight="bold"
        sx={{ mb: "5px" }}
      >
        {title}
      </Typography>
      <Typography variant="h5" color={colors.greenAccent[400]}>
        {description}
      </Typography>
    </Box>
  );
};
export default Header;
