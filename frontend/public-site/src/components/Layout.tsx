import { useState } from 'react';
import { Link as RouterLink, Outlet, useLocation } from 'react-router-dom';
import {
  AppBar,
  Box,
  Container,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Toolbar,
  Typography,
  Stack,
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import PhoneIphoneIcon from '@mui/icons-material/PhoneIphone';

const NAV_LINKS = [
  { label: 'Mobiles', to: '/mobiles' },
  { label: 'Accessories', to: '/accessories' },
  { label: 'Repairs', to: '/repairs' },
];

export default function Layout() {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const location = useLocation();

  return (
    <>
      <AppBar position="sticky" color="primary" enableColorOnDark>
        <Toolbar>
          <IconButton
            color="inherit"
            edge="start"
            sx={{ mr: 1, display: { sm: 'none' } }}
            onClick={() => setDrawerOpen(true)}
            aria-label="open navigation menu"
          >
            <MenuIcon />
          </IconButton>
          <PhoneIphoneIcon sx={{ mr: 1 }} />
          <Typography
            variant="h6"
            component={RouterLink}
            to="/"
            sx={{ flexGrow: 1, color: 'inherit', textDecoration: 'none', fontWeight: 600 }}
          >
            Mobile Shop
          </Typography>
          <Stack direction="row" spacing={1} sx={{ display: { xs: 'none', sm: 'flex' } }}>
            {NAV_LINKS.map((link) => (
              <Box
                key={link.to}
                component={RouterLink}
                to={link.to}
                sx={{
                  color: 'inherit',
                  textDecoration: 'none',
                  px: 1.5,
                  py: 1,
                  borderRadius: 1,
                  fontWeight: location.pathname.startsWith(link.to) ? 700 : 400,
                  bgcolor: location.pathname.startsWith(link.to) ? 'rgba(255,255,255,0.15)' : 'transparent',
                }}
              >
                {link.label}
              </Box>
            ))}
          </Stack>
        </Toolbar>
      </AppBar>

      <Drawer anchor="left" open={drawerOpen} onClose={() => setDrawerOpen(false)}>
        <Box sx={{ width: 240 }} role="presentation" onClick={() => setDrawerOpen(false)}>
          <List>
            {NAV_LINKS.map((link) => (
              <ListItemButton key={link.to} component={RouterLink} to={link.to}>
                <ListItemText primary={link.label} />
              </ListItemButton>
            ))}
          </List>
        </Box>
      </Drawer>

      <Container maxWidth="lg" sx={{ py: 3 }}>
        <Outlet />
      </Container>

      <Box component="footer" sx={{ py: 3, textAlign: 'center', color: 'text.secondary' }}>
        <Typography variant="body2">
          Mobile Shop &mdash; visit us in person to buy, sell, or repair your device.
        </Typography>
      </Box>
    </>
  );
}
