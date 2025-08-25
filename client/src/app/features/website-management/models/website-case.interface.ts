export interface WebsiteCase {
  id: number;
  title: string;
  shortDescription: string;
  fullDescription: string;
  mainImageUrl: string;
  materials: string;
  procedure: string;
  results: string;
  patientInfo: string;
  isActive: boolean;
  order: number;
  createdAt: string;
  updatedAt: string;
  images: WebsiteCaseImage[];
}

export interface WebsiteCaseImage {
  id: number;
  imageUrl: string;
  altText: string;
  caption: string;
  order: number;
  isMainImage: boolean;
  createdAt: string;
}

export interface CreateWebsiteCaseDto {
  title: string;
  shortDescription: string;
  fullDescription: string;
  mainImageUrl: string;
  materials: string;
  procedure: string;
  results: string;
  patientInfo: string;
  isActive: boolean;
  order: number;
  images?: CreateWebsiteCaseImageDto[];
}

export interface UpdateWebsiteCaseDto {
  title: string;
  shortDescription: string;
  fullDescription: string;
  mainImageUrl: string;
  materials: string;
  procedure: string;
  results: string;
  patientInfo: string;
  isActive: boolean;
  order: number;
  images?: CreateWebsiteCaseImageDto[];
}

export interface CreateWebsiteCaseImageDto {
  imageUrl: string;
  altText: string;
  caption: string;
  order: number;
  isMainImage: boolean;
}

export interface WebsiteCaseAdmin {
  id: number;
  title: string;
  shortDescription: string;
  mainImageUrl: string;
  isActive: boolean;
  order: number;
  createdAt: string;
  updatedAt: string;
  imageCount: number;
}

export interface ReorderItem {
  id: number;
  order: number;
}