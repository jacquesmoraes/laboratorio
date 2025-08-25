export interface WebsiteWorkType {
  id: number;
  workTypeId: number;
  workTypeName: string;
  workTypeDescription: string;
  workSectionName: string;
  imageUrl: string;
  isActive: boolean;
  order: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateWebsiteWorkTypeDto {
  workTypeId: number;
    imageUrl: string;
  isActive: boolean;
  order: number;
}

export interface UpdateWebsiteWorkTypeDto {
  workTypeId: number;
   imageUrl: string;
  isActive: boolean;
  order: number;
}

export interface ReorderItem {
  id: number;
  order: number;
}