import { useParams, Link as RouterLink } from 'react-router-dom';
import { Box, Breadcrumbs, Chip, CircularProgress, Divider, Grid, Link, Paper, Stack, Typography } from '@mui/material';
import { useAccessory } from '../hooks/useAccessories';
import ProductImage from '../components/ProductImage';
import InquiryForm from '../components/InquiryForm';

export default function AccessoryDetailPage() {
  const { id } = useParams();
  const accessoryId = Number(id);
  const { data: accessory, isLoading, isError } = useAccessory(accessoryId);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (isError || !accessory) {
    return <Typography color="error">This item could not be found.</Typography>;
  }

  const primaryImageId = accessory.imageIds[0] ?? null;

  return (
    <Box>
      <Breadcrumbs sx={{ mb: 2 }}>
        <Link component={RouterLink} to="/accessories" underline="hover">
          Accessories
        </Link>
        <Typography color="text.primary">{accessory.name}</Typography>
      </Breadcrumbs>

      <Grid container spacing={4}>
        <Grid size={{ xs: 12, sm: 6 }}>
          <ProductImage imageId={primaryImageId} variant="full" alt={accessory.name} aspectRatio="1 / 1" />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            {accessory.name}
          </Typography>
          <Chip label={accessory.categoryName} size="small" sx={{ my: 1 }} />
          <Typography variant="h4" color="primary" sx={{ fontWeight: 700, my: 2 }}>
            ₹{accessory.price.toLocaleString()}
          </Typography>
          {accessory.description && (
            <Typography variant="body1" sx={{ mb: 2 }}>
              {accessory.description}
            </Typography>
          )}

          {accessory.compatibleMobiles.length > 0 && (
            <>
              <Typography variant="subtitle2" gutterBottom>
                Compatible with:
              </Typography>
              <Stack direction="row" spacing={1} useFlexGap sx={{ flexWrap: 'wrap', mb: 3 }}>
                {accessory.compatibleMobiles.map((cm, idx) => (
                  <Chip key={idx} label={`${cm.brand} ${cm.model}`} size="small" variant="outlined" />
                ))}
              </Stack>
            </>
          )}

          <Divider sx={{ my: 3 }} />

          <Paper variant="outlined" sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>
              Interested in this accessory?
            </Typography>
            <InquiryForm listingType="Accessory" listingId={accessory.accessoryId} />
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}
