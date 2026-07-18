import { useState } from 'react';
import { Link as RouterLink, Outlet, useLocation, useNavigate } from 'react-router-dom';
import {
  AppBar,
  Box,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import DashboardIcon from '@mui/icons-material/Dashboard';
import PhoneIphoneIcon from '@mui/icons-material/PhoneIphone';
import HeadphonesIcon from '@mui/icons-material/Headphones';
import CategoryIcon from '@mui/icons-material/Category';
import BuildIcon from '@mui/icons-material/Build';
import MailIcon from '@mui/icons-material/Mail';
import LogoutIcon from '@mui/icons-material/Logout';
import { useLogout } from '../hooks/useAuth';

const DRAWER_WIDTH = 220;

const NAV_ITEMS = [
  { to: '/', label: 'Dashboard', icon: DashboardIcon },
  { to: '/mobiles', label: 'Mobiles', icon: PhoneIphoneIcon },
  { to: '/accessories', label: 'Accessories', icon: HeadphonesIcon },
  { to: '/categories', label: 'Categories', icon: CategoryIcon },
  { to: '/repair-services', label: 'Repair Services', icon: BuildIcon },
  { to: '/inquiries', label: 'Inquiries', icon: MailIcon },
];

export default function AdminLayout() {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();
  const logoutMutation = useLogout();

  const handleLogout = () => {
    logoutMutation.mutate(undefined, { onSettled: () => navigate('/login') });
  };

  const drawerContent = (
    <Box sx={{ width: DRAWER_WIDTH }} role="presentation">
      <Toolbar />
      <List>
        {NAV_ITEMS.map(({ to, label, icon: Icon }) => (
          <ListItemButton
            key={to}
            component={RouterLink}
            to={to}
            selected={location.pathname === to}
            onClick={() => setDrawerOpen(false)}
          >
            <ListItemIcon>
              <Icon />
            </ListItemIcon>
            <ListItemText primary={label} />
          </ListItemButton>
        ))}
        <ListItemButton onClick={handleLogout}>
          <ListItemIcon>
            <LogoutIcon />
          </ListItemIcon>
          <ListItemText primary="Log out" />
        </ListItemButton>
      </List>
    </Box>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton
            color="inherit"
            edge="start"
            sx={{ mr: 2 }}
            onClick={() => setDrawerOpen(true)}
            aria-label="open navigation menu"
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap>
            Mobile Shop Admin
          </Typography>
        </Toolbar>
      </AppBar>

      <Drawer
        variant="temporary"
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
        ModalProps={{ keepMounted: true }}
        sx={{ '& .MuiDrawer-paper': { width: DRAWER_WIDTH } }}
      >
        {drawerContent}
      </Drawer>

      <Box component="main" sx={{ flexGrow: 1, p: 3, width: '100%' }}>
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  );
}
