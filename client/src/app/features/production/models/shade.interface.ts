export interface Shade {
  id: number;
  color?: string;
  scaleId: number;
}

export interface CreateShadeDto {
  color?: string;
  scaleId: number;
}

export interface UpdateShadeDto {
  color?: string;
  scaleId: number;
}