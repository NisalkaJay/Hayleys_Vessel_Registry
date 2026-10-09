export interface Vessel {
  vesselId: number;
  vesselName: string;
  imoNumber: string;
  vesselTypeId: number;
  vesselTypeName?: string;
  flagCountry: string;
  grossTonnage: number;
  yearBuilt: number;
  isActive: boolean;
}

export interface VesselType {
  vesselTypeId: number;
  name: string;
}

export interface PagedResult {
  items: Vessel[];
  totalCount: number;
  page: number;
  pageSize: number;
}
