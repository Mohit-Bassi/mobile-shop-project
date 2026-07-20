import { Box } from '@mui/material';
import PhoneIphoneIcon from '@mui/icons-material/PhoneIphone';
import { imageUrl } from '../api/client';

interface ProductImageProps {
  imageId: number | null;
  variant?: 'thumbnail' | 'medium' | 'full';
  alt: string;
  aspectRatio?: string;
  rounded?: boolean;
}

export default function ProductImage({ imageId, variant = 'medium', alt, aspectRatio = '4 / 3', rounded = true }: ProductImageProps) {
  const borderRadius = rounded ? 2 : 0;

  if (imageId === null) {
    return (
      <Box
        sx={{
          aspectRatio,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          background: 'linear-gradient(180deg, #EDEAE2 0%, #E2DED2 100%)',
          borderRadius,
          color: '#B7AF9E',
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
      sx={{ width: '100%', aspectRatio, objectFit: 'cover', borderRadius, display: 'block' }}
    />
  );
}
