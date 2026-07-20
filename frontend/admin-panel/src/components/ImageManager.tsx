import { useRef } from 'react';
import { Alert, Box, Button, CircularProgress, IconButton, Stack, Tooltip, Typography } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import StarIcon from '@mui/icons-material/Star';
import StarBorderIcon from '@mui/icons-material/StarBorder';
import UploadIcon from '@mui/icons-material/Upload';
import { imageUrl } from '../api/client';
import { useDeleteImage, useSetPrimaryImage, useUploadImage } from '../hooks/useImages';
import type { ImageOwnerType } from '../api/images';

interface ImageManagerProps {
  ownerType: ImageOwnerType;
  ownerId: number;
  imageIds: number[];
  primaryImageId?: number | null;
  invalidateKey: unknown[];
}

export default function ImageManager({ ownerType, ownerId, imageIds, primaryImageId, invalidateKey }: ImageManagerProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const uploadMutation = useUploadImage(invalidateKey);
  const deleteMutation = useDeleteImage(invalidateKey);
  const setPrimaryMutation = useSetPrimaryImage(invalidateKey);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      uploadMutation.mutate({ ownerType, ownerId, file });
    }
    e.target.value = '';
  };

  return (
    <Box>
      <Typography variant="subtitle2" gutterBottom>
        Photos
      </Typography>
      <Stack direction="row" spacing={2} useFlexGap sx={{ flexWrap: 'wrap', mb: 2 }}>
        {imageIds.map((imageId) => (
          <Box key={imageId} sx={{ position: 'relative', width: 100 }}>
            <Box
              component="img"
              src={imageUrl(imageId, 'thumbnail')}
              alt=""
              sx={{ width: 100, height: 100, objectFit: 'cover', borderRadius: 1, border: '2px solid', borderColor: imageId === primaryImageId ? 'primary.main' : 'divider' }}
            />
            <Stack direction="row" sx={{ position: 'absolute', top: 2, right: 2 }}>
              <Tooltip title={imageId === primaryImageId ? 'Primary image' : 'Set as primary'}>
                <IconButton
                  size="small"
                  sx={{ bgcolor: 'background.paper' }}
                  onClick={() => setPrimaryMutation.mutate(imageId)}
                  disabled={imageId === primaryImageId}
                >
                  {imageId === primaryImageId ? <StarIcon fontSize="small" color="primary" /> : <StarBorderIcon fontSize="small" />}
                </IconButton>
              </Tooltip>
            </Stack>
            <IconButton
              size="small"
              sx={{ position: 'absolute', bottom: 2, right: 2, bgcolor: 'background.paper' }}
              onClick={() => deleteMutation.mutate(imageId)}
            >
              <DeleteIcon fontSize="small" color="error" />
            </IconButton>
          </Box>
        ))}
      </Stack>

      <input ref={fileInputRef} type="file" accept="image/png,image/jpeg,image/webp" hidden onChange={handleFileChange} />
      <Button
        variant="outlined"
        startIcon={uploadMutation.isPending ? <CircularProgress size={16} /> : <UploadIcon />}
        onClick={() => fileInputRef.current?.click()}
        disabled={uploadMutation.isPending}
      >
        Upload photo
      </Button>

      {uploadMutation.isError && (
        <Alert severity="error" sx={{ mt: 2 }}>
          Upload failed. Use a JPEG, PNG, or WebP file under 8MB.
        </Alert>
      )}
    </Box>
  );
}
