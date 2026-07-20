import { Box, Card, CardContent, CircularProgress, Grid, Paper, Typography } from '@mui/material';
import { useRepairServices } from '../hooks/useRepairServices';
import InquiryForm from '../components/InquiryForm';

export default function RepairServicesPage() {
  const { data: services, isLoading, isError } = useRepairServices();

  return (
    <Box>
      <Typography variant="h5" component="h1" gutterBottom>
        Repair Services
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        We repair phones of any brand or model. Bring your device in, or send an inquiry below.
      </Typography>

      {isLoading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}>
          <CircularProgress />
        </Box>
      )}

      {isError && <Typography color="error">Failed to load repair services.</Typography>}

      {services && (
        <Grid container spacing={2} sx={{ mb: 4 }}>
          {services.map((service) => (
            <Grid key={service.repairServiceId} size={{ xs: 12, sm: 6, md: 4 }}>
              <Card sx={{ height: '100%' }}>
                <CardContent sx={{ p: 3 }}>
                  <Typography variant="h6" gutterBottom>
                    {service.title}
                  </Typography>
                  {service.description && (
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 1.5 }}>
                      {service.description}
                    </Typography>
                  )}
                  <Typography variant="subtitle1" sx={{ fontWeight: 800, color: 'secondary.dark' }}>
                    {service.priceFrom ? `From ₹${service.priceFrom.toLocaleString()}` : 'Contact for quote'}
                  </Typography>
                  {service.estimatedTurnaround && (
                    <Typography variant="caption" color="text.secondary">
                      Estimated time: {service.estimatedTurnaround}
                    </Typography>
                  )}
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      <Paper variant="outlined" sx={{ p: 2.5, maxWidth: 480, borderRadius: 3 }}>
        <Typography variant="h6" gutterBottom>
          Ask about a repair
        </Typography>
        <InquiryForm listingType="RepairService" />
      </Paper>
    </Box>
  );
}
