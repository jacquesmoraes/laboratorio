export interface Sector {
  id: number;
  name: string;
  description?: string;
  
}

// Interface para a resposta da API
export interface SectorApiResponse {
  sectorId: number;
  name: string;
}