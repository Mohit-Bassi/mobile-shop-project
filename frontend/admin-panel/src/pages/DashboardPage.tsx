import { Box, Card, CardContent, CircularProgress, Grid, Stack, Typography } from '@mui/material';
import PhoneIphoneIcon from '@mui/icons-material/PhoneIphone';
import HeadphonesIcon from '@mui/icons-material/Headphones';
import MarkEmailUnreadIcon from '@mui/icons-material/MarkEmailUnread';
import MailIcon from '@mui/icons-material/Mail';
import { useDashboardSummary } from '../hooks/useDashboard';

const TILES = [
  { key: 'activeMobiles' as const, label: 'Active Mobiles', icon: PhoneIphoneIcon, accent: false },
  { key: 'activeAccessories' as const, label: 'Active Accessories', icon: HeadphonesIcon, accent: false },
  { key: 'newInquiries' as const, label: 'New Inquiries', icon: MarkEmailUnreadIcon, accent: true },
  { key: 'totalInquiries' as const, label: 'Total Inquiries', icon: MailIcon, accent: false },
];

export default function DashboardPage() {
  const { data, isLoading } = useDashboardSummary();

  return (
    <Box>
      <Typography variant="h5" sx={{ mb: 3 }}>
        Dashboard
      </Typography>

      {isLoading && <CircularProgress />}

      {data && (
        <Grid container spacing={2.5}>
          {TILES.map((tile) => (
            <Grid key={tile.key} size={{ xs: 6, sm: 3 }}>
              <Card>
                <CardContent sx={{ p: 2.5 }}>
                  <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center', mb: 1.5 }}>
                    <Box
                      sx={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        width: 40,
                        height: 40,
                        borderRadius: 2,
                        bgcolor: tile.accent ? 'rgba(199,123,44,0.12)' : 'rgba(30,58,95,0.08)',
                        color: tile.accent ? 'secondary.dark' : 'primary.main',
                      }}
                    >
                      <tile.icon fontSize="small" />
                    </Box>
                    <Typography variant="body2" color="text.secondary">
                      {tile.label}
                    </Typography>
                  </Stack>
                  <Typography variant="h4" sx={{ fontWeight: 800, color: tile.accent ? 'secondary.dark' : 'text.primary' }}>
                    {data[tile.key]}
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
    </Box>
  );
}
