import { useState } from 'react';
import { Link as RouterLink, useSearchParams } from 'react-router-dom';
import {
  Box,
  Card,
  CardActionArea,
  CardContent,
  CircularProgress,
  Grid,
  MenuItem,
  Pagination,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { useAccessories } from '../hooks/useAccessories';
import { useCategories } from '../hooks/useCategories';
import ProductImage from '../components/ProductImage';
import type { AccessoryFilters } from '../api/accessories';

export default function AccessoriesListPage() {
  const [searchParams] = useSearchParams();
  const [filters, setFilters] = useState<AccessoryFilters>({
    page: 1,
    pageSize: 12,
    categoryId: searchParams.get('categoryId') ? Number(searchParams.get('categoryId')) : undefined,
    compatibleBrand: searchParams.get('compatibleBrand') ?? undefined,
    compatibleModel: searchParams.get('compatibleModel') ?? undefined,
  });

  const { data, isLoading, isError } = useAccessories(filters);
  const { data: categories } = useCategories();

  const updateFilter = (patch: Partial<AccessoryFilters>) => setFilters((f) => ({ ...f, ...patch, page: 1 }));

  return (
    <Box>
      <Typography variant="h5" component="h1" sx={{ fontWeight: 700 }} gutterBottom>
        Accessories
      </Typography>

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 3 }}>
        <TextField
          select
          label="Category"
          size="small"
          fullWidth
          value={filters.categoryId ?? ''}
          onChange={(e) => updateFilter({ categoryId: e.target.value ? Number(e.target.value) : undefined })}
        >
          <MenuItem value="">All categories</MenuItem>
          {categories?.map((c) => (
            <MenuItem key={c.categoryId} value={c.categoryId}>
              {c.name}
            </MenuItem>
          ))}
        </TextField>
        <TextField
          label="Compatible brand"
          size="small"
          fullWidth
          value={filters.compatibleBrand ?? ''}
          onChange={(e) => updateFilter({ compatibleBrand: e.target.value || undefined })}
        />
        <TextField
          label="Compatible model"
          size="small"
          fullWidth
          value={filters.compatibleModel ?? ''}
          onChange={(e) => updateFilter({ compatibleModel: e.target.value || undefined })}
        />
      </Stack>

      {isLoading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}>
          <CircularProgress />
        </Box>
      )}

      {isError && <Typography color="error">Failed to load accessories. Please try again.</Typography>}

      {data && data.items.length === 0 && (
        <Typography color="text.secondary">No accessories match your filters right now.</Typography>
      )}

      {data && data.items.length > 0 && (
        <>
          <Grid container spacing={2}>
            {data.items.map((accessory) => (
              <Grid key={accessory.accessoryId} size={{ xs: 6, sm: 4, md: 3 }}>
                <Card variant="outlined" sx={{ height: '100%' }}>
                  <CardActionArea component={RouterLink} to={`/accessories/${accessory.accessoryId}`}>
                    <ProductImage imageId={accessory.primaryImageId} variant="medium" alt={accessory.name} />
                    <CardContent>
                      <Typography variant="subtitle2" noWrap>
                        {accessory.name}
                      </Typography>
                      <Typography variant="body2" color="text.secondary" gutterBottom>
                        {accessory.categoryName}
                      </Typography>
                      <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                        ₹{accessory.price.toLocaleString()}
                      </Typography>
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
