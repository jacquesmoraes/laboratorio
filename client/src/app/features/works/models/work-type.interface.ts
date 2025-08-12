export interface WorkType {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
  workSectionId: number;
  workSectionName?: string;
}

export interface WorkTypeApiResponse {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
  workSectionId: number;
  workSectionName: string;
}

export interface CreateWorkTypeDto {
  name: string;
  description?: string;
  isActive: boolean;
  workSectionId: number;
}

export interface UpdateWorkTypeDto {
  name: string;
  description?: string;
  isActive: boolean;
  workSectionId: number;
}