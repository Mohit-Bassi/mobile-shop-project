import { useState } from 'react';
import { Link as RouterLink, Outlet, useLocation, useNavigate } from 'react-router-dom';
import {
  AppBar,
  Box,
  Divider,
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

  const navList = (onNavigate?: () => void) => (
    <List sx={{ px: 1.5, py: 2 }}>
      {NAV_ITEMS.map(({ to, label, icon: Icon }) => {
        const selected = location.pathname === to;
        return (
          <ListItemButton
            key={to}
            component={RouterLink}
            to={to}
            selected={selected}
            onClick={onNavigate}
            sx={{
              borderRadius: 2,
              mb: 0.5,
              color: selected ? 'primary.main' : 'text.secondary',
              '&.Mui-selected': { bgcolor: 'rgba(30,58,95,0.08)' },
              '&.Mui-selected:hover': { bgcolor: 'rgba(30,58,95,0.12)' },
            }}
          >
            <ListItemIcon sx={{ color: 'inherit', minWidth: 40 }}>
              <Icon fontSize="small" />
            </ListItemIcon>
            <ListItemText primary={label} slotProps={{ primary: { sx: { fontWeight: selected ? 700 : 500, fontSize: '0.92rem' } } }} />
          </ListItemButton>
        );
      })}
    </List>
  );

  const drawerContent = (onNavigate?: () => void) => (
    <Box sx={{ width: DRAWER_WIDTH, height: '100%', display: 'flex', flexDirection: 'column' }} role="presentation">
      <Toolbar sx={{ minHeight: 68 }}>
        <Typography sx={{ fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700, color: 'primary.main' }}>
          Mobile Shop
        </Typography>
      </Toolbar>
      <Divider />
      <Box sx={{ flexGrow: 1 }}>{navList(onNavigate)}</Box>
      <Divider />
      <List sx={{ px: 1.5, py: 1.5 }}>
        <ListItemButton onClick={handleLogout} sx={{ borderRadius: 2, color: 'text.secondary' }}>
          <ListItemIcon sx={{ minWidth: 40 }}>
            <LogoutIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText primary="Log out" slotProps={{ primary: { sx: { fontSize: '0.92rem' } } }} />
        </ListItemButton>
      </List>
    </Box>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar
        position="fixed"
        sx={{ zIndex: (theme) => theme.zIndex.drawer + 1, width: { sm: `calc(100% - ${DRAWER_WIDTH}px)` }, ml: { sm: `${DRAWER_WIDTH}px` } }}
      >
        <Toolbar sx={{ minHeight: 68 }}>
          <IconButton
            color="inherit"
            edge="start"
            sx={{ mr: 2, display: { sm: 'none' } }}
            onClick={() => setDrawerOpen(true)}
            aria-label="open navigation menu"
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap>
            Admin Panel
          </Typography>
        </Toolbar>
      </AppBar>

      <Box component="nav" sx={{ width: { sm: DRAWER_WIDTH }, flexShrink: { sm: 0 } }}>
        <Drawer
          variant="temporary"
          open={drawerOpen}
          onClose={() => setDrawerOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{ display: { xs: 'block', sm: 'none' }, '& .MuiDrawer-paper': { width: DRAWER_WIDTH } }}
        >
          {drawerContent(() => setDrawerOpen(false))}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{ display: { xs: 'none', sm: 'block' }, '& .MuiDrawer-paper': { width: DRAWER_WIDTH, boxSizing: 'border-box' } }}
          open
        >
          {drawerContent()}
        </Drawer>
      </Box>

      <Box component="main" sx={{ flexGrow: 1, p: { xs: 2, sm: 3.5 }, width: { sm: `calc(100% - ${DRAWER_WIDTH}px)` } }}>
        <Toolbar sx={{ minHeight: 68 }} />
        <Outlet />
      </Box>
    </Box>
  );
}
