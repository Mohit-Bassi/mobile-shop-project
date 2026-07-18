import { Box, Card, CardContent, CircularProgress, Grid, Typography } from '@mui/material';
import { useDashboardSummary } from '../hooks/useDashboard';

const TILES = [
  { key: 'activeMobiles' as const, label: 'Active Mobiles' },
  { key: 'activeAccessories' as const, label: 'Active Accessories' },
  { key: 'newInquiries' as const, label: 'New Inquiries' },
  { key: 'totalInquiries' as const, label: 'Total Inquiries' },
];

export default function DashboardPage() {
  const { data, isLoading } = useDashboardSummary();

  return (
    <Box>
      <Typography variant="h5" sx={{ fontWeight: 700, mb: 3 }}>
        Dashboard
      </Typography>

      {isLoading && <CircularProgress />}

      {data && (
        <Grid container spacing={2}>
          {TILES.map((tile) => (
            <Grid key={tile.key} size={{ xs: 6, sm: 3 }}>
              <Card variant="outlined">
                <CardContent>
                  <Typography variant="body2" color="text.secondary">
                    {tile.label}
                  </Typography>
                  <Typography variant="h4" sx={{ fontWeight: 700 }}>
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
