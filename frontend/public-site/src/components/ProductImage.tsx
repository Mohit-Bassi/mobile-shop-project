import { Box } from '@mui/material';
import PhoneIphoneIcon from '@mui/icons-material/PhoneIphone';
import { imageUrl } from '../api/client';

interface ProductImageProps {
  imageId: number | null;
  variant?: 'thumbnail' | 'medium' | 'full';
  alt: string;
  aspectRatio?: string;
}

export default function ProductImage({ imageId, variant = 'medium', alt, aspectRatio = '4 / 3' }: ProductImageProps) {
  if (imageId === null) {
    return (
      <Box
        sx={{
          aspectRatio,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          bgcolor: 'grey.100',
          borderRadius: 1,
          color: 'grey.400',
        }}
      >
        <PhoneIphoneIcon sx={{ fontSize: 48 }} />
      </Box>
    );
  }

  return (
    <Box
      component="img"
      src={imageUrl(imageId, variant)}
      alt={alt}
      loading="lazy"
      sx={{ width: '100%', aspectRatio, objectFit: 'cover', borderRadius: 1, display: 'block' }}
    />
  );
}
