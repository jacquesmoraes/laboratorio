export interface UploadResponse {
    success: boolean;
    imageUrl: string;
    fileName: string;
    originalName: string;
    size: number;
  }
  
  export interface DeleteResponse {
    success: boolean;
  }
  
  export interface UploadProgress {
    progress: number;
    imageUrl?: string;
    fileName?: string;
    originalName?: string;
    size?: number;
  }
  
  export interface UploadError {
    message: string;
    status: number;
  }