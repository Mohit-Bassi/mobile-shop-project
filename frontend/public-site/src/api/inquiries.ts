import { apiClient } from './client';
import type { SubmitInquiryRequest } from '../types/api';

export async function submitInquiry(request: SubmitInquiryRequest): Promise<void> {
  await apiClient.post('/inquiries', request);
}
