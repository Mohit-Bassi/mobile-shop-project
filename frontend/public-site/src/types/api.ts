export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export type ConditionGrade = 'New' | 'LikeNew' | 'Good' | 'Fair';

export interface MobileListItem {
  mobileId: number;
  brand: string;
  model: string;
  storage?: string | null;
  color?: string | null;
  conditionGrade: ConditionGrade;
  price: number;
  status: string;
  primaryImageId: number | null;
}

export interface MobileDetail {
  mobileId: number;
  brand: string;
  model: string;
  storage?: string | null;
  color?: string | null;
  conditionGrade: ConditionGrade;
  price: number;
  description?: string | null;
  specsJson?: string | null;
  status: string;
  createdAtUtc: string;
  updatedAtUtc: string;
  imageIds: number[];
}

export interface AccessoryListItem {
  accessoryId: number;
  name: string;
  categoryId: number;
  categoryName: string;
  price: number;
  status: string;
  primaryImageId: number | null;
}

export interface CompatibleMobile {
  brand: string;
  model: string;
}

export interface AccessoryDetail {
  accessoryId: number;
  name: string;
  categoryId: number;
  categoryName: string;
  price: number;
  description?: string | null;
  status: string;
  compatibleMobiles: CompatibleMobile[];
  imageIds: number[];
}

export interface Category {
  categoryId: number;
  name: string;
  slug: string;
  displayOrder: number;
}

export interface RepairService {
  repairServiceId: number;
  title: string;
  description?: string | null;
  priceFrom?: number | null;
  estimatedTurnaround?: string | null;
  displayOrder: number;
}

export type InquiryListingType = 'Mobile' | 'Accessory' | 'RepairService' | 'General';

export interface SubmitInquiryRequest {
  listingType: InquiryListingType;
  listingId?: number | null;
  customerName: string;
  customerPhone: string;
  customerEmail?: string | null;
  message?: string | null;
}
