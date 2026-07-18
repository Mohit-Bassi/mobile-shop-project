export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export type ConditionGrade = 'New' | 'LikeNew' | 'Good' | 'Fair';
export type ListingStatus = 'Active' | 'SoldOut' | 'Draft';

export interface MobileListItem {
  mobileId: number;
  brand: string;
  model: string;
  storage?: string | null;
  color?: string | null;
  conditionGrade: ConditionGrade;
  price: number;
  status: ListingStatus;
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
  status: ListingStatus;
  createdAtUtc: string;
  updatedAtUtc: string;
  imageIds: number[];
}

export interface MobileRequest {
  brand: string;
  model: string;
  storage?: string;
  color?: string;
  conditionGrade: ConditionGrade;
  price: number;
  description?: string;
  specsJson?: string;
  status: ListingStatus;
}

export interface AccessoryListItem {
  accessoryId: number;
  name: string;
  categoryId: number;
  categoryName: string;
  price: number;
  status: ListingStatus;
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
  status: ListingStatus;
  compatibleMobiles: CompatibleMobile[];
  imageIds: number[];
}

export interface AccessoryRequest {
  name: string;
  categoryId: number;
  price: number;
  description?: string;
  status: ListingStatus;
  compatibleMobiles: CompatibleMobile[];
}

export interface Category {
  categoryId: number;
  name: string;
  slug: string;
  displayOrder: number;
}

export interface CategoryRequest {
  name: string;
  slug: string;
  displayOrder: number;
  isActive: boolean;
}

export interface RepairService {
  repairServiceId: number;
  title: string;
  description?: string | null;
  priceFrom?: number | null;
  estimatedTurnaround?: string | null;
  displayOrder: number;
}

export interface RepairServiceRequest {
  title: string;
  description?: string;
  priceFrom?: number;
  estimatedTurnaround?: string;
  isActive: boolean;
  displayOrder: number;
}

export type InquiryListingType = 'Mobile' | 'Accessory' | 'RepairService' | 'General';
export type InquiryStatus = 'New' | 'Contacted' | 'Closed';

export interface Inquiry {
  inquiryId: number;
  listingType: InquiryListingType;
  listingId: number | null;
  customerName: string;
  customerPhone: string;
  customerEmail?: string | null;
  message?: string | null;
  status: InquiryStatus;
  createdAtUtc: string;
}

export interface DashboardSummary {
  activeMobiles: number;
  activeAccessories: number;
  newInquiries: number;
  totalInquiries: number;
}

export interface ImageUploadResult {
  imageId: number;
  isPrimary: boolean;
  displayOrder: number;
}
