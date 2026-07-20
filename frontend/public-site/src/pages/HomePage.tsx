import { Link as RouterLink } from 'react-router-dom';
import { Box, Button, Card, CardActionArea, CardContent, Grid, Typography } from '@mui/material';
import PhoneIphoneIcon from '@mui/icons-material/PhoneIphone';
import HeadphonesIcon from '@mui/icons-material/Headphones';
import BuildIcon from '@mui/icons-material/Build';

const SECTIONS = [
  { to: '/mobiles', icon: PhoneIphoneIcon, title: 'Browse Mobiles', description: 'Quality second-hand phones, checked and graded.' },
  { to: '/accessories', icon: HeadphonesIcon, title: 'Shop Accessories', description: 'Cases, chargers, earphones, and more.' },
  { to: '/repairs', icon: BuildIcon, title: 'Repair Services', description: 'Screen, battery, and water damage repairs for any brand.' },
];

export default function HomePage() {
  return (
    <Box>
      <Box
        sx={{
          textAlign: 'center',
          py: { xs: 5, sm: 8 },
          px: 2,
          mb: 5,
          borderRadius: 4,
          bgcolor: 'primary.main',
          backgroundImage: 'linear-gradient(135deg, #1E3A5F 0%, #16304F 60%, #122840 100%)',
          color: 'primary.contrastText',
        }}
      >
        <Typography
          variant="h3"
          component="h1"
          sx={{ mb: 2, textWrap: 'balance', fontSize: { xs: '1.9rem', sm: '2.6rem' } }}
        >
          Buy, Sell &amp; Repair Mobile Phones
        </Typography>
        <Typography variant="body1" sx={{ maxWidth: 480, mx: 'auto', opacity: 0.85 }}>
          Browse our current inventory of second-hand phones and accessories, or bring your device in for repair.
          Visit us in person &mdash; all transactions happen at our shop.
        </Typography>
        <Button
          component={RouterLink}
          to="/mobiles"
          variant="contained"
          color="secondary"
          size="large"
          sx={{ mt: 3.5 }}
        >
          View Available Mobiles
        </Button>
      </Box>

      <Grid container spacing={2.5}>
        {SECTIONS.map(({ to, icon: Icon, title, description }) => (
          <Grid key={to} size={{ xs: 12, sm: 4 }}>
            <Card>
              <CardActionArea component={RouterLink} to={to} sx={{ height: '100%' }}>
                <CardContent sx={{ textAlign: 'center', py: 4.5 }}>
                  <Box
                    sx={{
                      display: 'inline-flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      width: 56,
                      height: 56,
                      borderRadius: '50%',
                      bgcolor: 'rgba(30,58,95,0.08)',
                      mb: 1.5,
                    }}
                  >
                    <Icon color="primary" sx={{ fontSize: 28 }} />
                  </Box>
                  <Typography variant="h6" gutterBottom>
                    {title}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {description}
                  </Typography>
                </CardContent>
              </CardActionArea>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}
