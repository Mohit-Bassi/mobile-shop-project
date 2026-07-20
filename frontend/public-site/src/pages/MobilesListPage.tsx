import { useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  Box,
  Card,
  CardActionArea,
  CardContent,
  Chip,
  CircularProgress,
  Grid,
  MenuItem,
  Pagination,
  Paper,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { useMobiles } from '../hooks/useMobiles';
import ProductImage from '../components/ProductImage';
import type { MobileFilters } from '../api/mobiles';

const CONDITIONS = ['New', 'LikeNew', 'Good', 'Fair'];
const SORT_OPTIONS = [
  { value: 'createdat_desc', label: 'Newest first' },
  { value: 'price_asc', label: 'Price: low to high' },
  { value: 'price_desc', label: 'Price: high to low' },
];

export default function MobilesListPage() {
  const [filters, setFilters] = useState<MobileFilters>({ page: 1, pageSize: 12, sort: 'createdat_desc' });
  const { data, isLoading, isError } = useMobiles(filters);

  const updateFilter = (patch: Partial<MobileFilters>) => setFilters((f) => ({ ...f, ...patch, page: 1 }));

  return (
    <Box>
      <Typography variant="h5" component="h1" gutterBottom>
        Mobiles
      </Typography>

      <Paper variant="outlined" sx={{ p: 2, mb: 3, borderRadius: 3 }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <TextField
            label="Brand"
            size="small"
            fullWidth
            value={filters.brand ?? ''}
            onChange={(e) => updateFilter({ brand: e.target.value || undefined })}
          />
          <TextField
            select
            label="Condition"
            size="small"
            fullWidth
            value={filters.condition ?? ''}
            onChange={(e) => updateFilter({ condition: e.target.value || undefined })}
          >
            <MenuItem value="">Any</MenuItem>
            {CONDITIONS.map((c) => (
              <MenuItem key={c} value={c}>
                {c}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            select
            label="Sort"
            size="small"
            fullWidth
            value={filters.sort}
            onChange={(e) => updateFilter({ sort: e.target.value })}
          >
            {SORT_OPTIONS.map((opt) => (
              <MenuItem key={opt.value} value={opt.value}>
                {opt.label}
              </MenuItem>
            ))}
          </TextField>
        </Stack>
      </Paper>

      {isLoading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}>
          <CircularProgress />
        </Box>
      )}

      {isError && <Typography color="error">Failed to load mobiles. Please try again.</Typography>}

      {data && data.items.length === 0 && (
        <Typography color="text.secondary">No mobiles match your filters right now.</Typography>
      )}

      {data && data.items.length > 0 && (
        <>
          <Grid container spacing={2}>
            {data.items.map((mobile) => (
              <Grid key={mobile.mobileId} size={{ xs: 6, sm: 4, md: 3 }}>
                <Card sx={{ height: '100%' }}>
                  <CardActionArea component={RouterLink} to={`/mobiles/${mobile.mobileId}`} sx={{ height: '100%', alignItems: 'stretch' }}>
                    <ProductImage
                      imageId={mobile.primaryImageId}
                      variant="medium"
                      alt={`${mobile.brand} ${mobile.model}`}
                      rounded={false}
                    />
                    <CardContent>
                      <Typography variant="subtitle2" noWrap>
                        {mobile.brand} {mobile.model}
                      </Typography>
                      <Typography variant="body2" color="text.secondary" gutterBottom>
                        {mobile.storage ?? '—'}
                      </Typography>
                      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mt: 1 }}>
                        <Typography variant="subtitle1" sx={{ fontWeight: 800, color: 'secondary.dark' }}>
                          ₹{mobile.price.toLocaleString()}
                        </Typography>
                        <Chip label={mobile.conditionGrade} size="small" color="primary" variant="outlined" />
                      </Stack>
                    </CardContent>
                  </CardActionArea>
                </Card>
              </Grid>
            ))}
          </Grid>

          {data.totalPages > 1 && (
            <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
              <Pagination
                count={data.totalPages}
                page={filters.page ?? 1}
                onChange={(_, page) => setFilters((f) => ({ ...f, page }))}
                color="primary"
              />
            </Box>
          )}
        </>
      )}
    </Box>
  );
}
