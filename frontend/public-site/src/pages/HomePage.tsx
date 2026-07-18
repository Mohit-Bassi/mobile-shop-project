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
      <Box sx={{ textAlign: 'center', py: { xs: 4, sm: 6 } }}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }} gutterBottom>
          Buy, Sell &amp; Repair Mobile Phones
        </Typography>
        <Typography variant="body1" color="text.secondary" sx={{ maxWidth: 480, mx: 'auto' }}>
          Browse our current inventory of second-hand phones and accessories, or bring your device in for repair.
          Visit us in person &mdash; all transactions happen at our shop.
        </Typography>
      </Box>

      <Grid container spacing={2}>
        {SECTIONS.map(({ to, icon: Icon, title, description }) => (
          <Grid key={to} size={{ xs: 12, sm: 4 }}>
            <Card variant="outlined">
              <CardActionArea component={RouterLink} to={to} sx={{ height: '100%' }}>
                <CardContent sx={{ textAlign: 'center', py: 4 }}>
                  <Icon color="primary" sx={{ fontSize: 40, mb: 1 }} />
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

      <Box sx={{ textAlign: 'center', mt: 4 }}>
        <Button component={RouterLink} to="/mobiles" variant="contained" size="large">
          View Available Mobiles
        </Button>
      </Box>
    </Box>
  );
}
