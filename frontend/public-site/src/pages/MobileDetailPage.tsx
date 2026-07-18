import { useParams, Link as RouterLink } from 'react-router-dom';
import { Box, Breadcrumbs, Chip, CircularProgress, Divider, Grid, Link, Paper, Stack, Typography } from '@mui/material';
import { useMobile } from '../hooks/useMobiles';
import ProductImage from '../components/ProductImage';
import InquiryForm from '../components/InquiryForm';

export default function MobileDetailPage() {
  const { id } = useParams();
  const mobileId = Number(id);
  const { data: mobile, isLoading, isError } = useMobile(mobileId);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (isError || !mobile) {
    return <Typography color="error">This listing could not be found.</Typography>;
  }

  const primaryImageId = mobile.imageIds[0] ?? null;

  return (
    <Box>
      <Breadcrumbs sx={{ mb: 2 }}>
        <Link component={RouterLink} to="/mobiles" underline="hover">
          Mobiles
        </Link>
        <Typography color="text.primary">
          {mobile.brand} {mobile.model}
        </Typography>
      </Breadcrumbs>

      <Grid container spacing={4}>
        <Grid size={{ xs: 12, sm: 6 }}>
          <ProductImage imageId={primaryImageId} variant="full" alt={`${mobile.brand} ${mobile.model}`} aspectRatio="1 / 1" />
          {mobile.imageIds.length > 1 && (
            <Stack direction="row" spacing={1} sx={{ mt: 1, overflowX: 'auto' }}>
              {mobile.imageIds.map((imgId) => (
                <Box key={imgId} sx={{ width: 64, flexShrink: 0 }}>
                  <ProductImage imageId={imgId} variant="thumbnail" alt="" aspectRatio="1 / 1" />
                </Box>
              ))}
            </Stack>
          )}
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            {mobile.brand} {mobile.model}
          </Typography>
          <Stack direction="row" spacing={1} sx={{ my: 1 }}>
            <Chip label={mobile.conditionGrade} color="primary" size="small" />
            {mobile.storage && <Chip label={mobile.storage} size="small" />}
            {mobile.color && <Chip label={mobile.color} size="small" />}
          </Stack>
          <Typography variant="h4" color="primary" sx={{ fontWeight: 700, my: 2 }}>
            ₹{mobile.price.toLocaleString()}
          </Typography>
          {mobile.description && (
            <Typography variant="body1" sx={{ mb: 3 }}>
              {mobile.description}
            </Typography>
          )}

          <Divider sx={{ my: 3 }} />

          <Paper variant="outlined" sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>
              Interested in this phone?
            </Typography>
            <InquiryForm listingType="Mobile" listingId={mobile.mobileId} />
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}
