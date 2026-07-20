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
        <Toolbar sx={{ minHeight: 68 }}>
          <IconButton
            color="inherit"
            edge="start"
            sx={{ mr: 1, display: { sm: 'none' } }}
            onClick={() => setDrawerOpen(true)}
            aria-label="open navigation menu"
          >
            <MenuIcon />
          </IconButton>
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              width: 36,
              height: 36,
              borderRadius: 2,
              bgcolor: 'rgba(255,255,255,0.12)',
              mr: 1.5,
            }}
          >
            <PhoneIphoneIcon fontSize="small" />
          </Box>
          <Typography
            component={RouterLink}
            to="/"
            sx={{
              flexGrow: 1,
              color: 'inherit',
              textDecoration: 'none',
              fontFamily: '"Plus Jakarta Sans", sans-serif',
              fontWeight: 700,
              fontSize: '1.15rem',
              letterSpacing: '-0.01em',
            }}
          >
            Mobile Shop
          </Typography>
          <Stack direction="row" spacing={0.5} sx={{ display: { xs: 'none', sm: 'flex' } }}>
            {NAV_LINKS.map((link) => {
              const active = location.pathname.startsWith(link.to);
              return (
                <Box
                  key={link.to}
                  component={RouterLink}
                  to={link.to}
                  sx={{
                    position: 'relative',
                    color: 'inherit',
                    textDecoration: 'none',
                    px: 1.75,
                    py: 1,
                    fontWeight: active ? 700 : 500,
                    opacity: active ? 1 : 0.85,
                    '&:hover': { opacity: 1 },
                    '&::after': {
                      content: '""',
                      position: 'absolute',
                      left: 14,
                      right: 14,
                      bottom: 6,
                      height: 2,
                      borderRadius: 1,
                      bgcolor: 'secondary.main',
                      opacity: active ? 1 : 0,
                      transition: 'opacity 0.15s ease',
                    },
                  }}
                >
                  {link.label}
                </Box>
              );
            })}
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

      <Container maxWidth="lg" sx={{ py: { xs: 3, sm: 4 }, minHeight: '70vh' }}>
        <Outlet />
      </Container>

      <Box component="footer" sx={{ mt: 4, borderTop: '1px solid', borderColor: 'divider', bgcolor: 'background.paper' }}>
        <Container maxWidth="lg" sx={{ py: 4 }}>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1} sx={{ justifyContent: 'space-between', alignItems: { sm: 'center' } }}>
            <Typography variant="subtitle2" sx={{ fontFamily: '"Plus Jakarta Sans", sans-serif' }}>
              Mobile Shop
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Visit us in person to buy, sell, or repair your device &mdash; no online payments.
            </Typography>
          </Stack>
        </Container>
      </Box>
    </>
  );
}
