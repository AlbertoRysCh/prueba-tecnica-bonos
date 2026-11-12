
export interface Bonus {
  bonusId: number;
  departmentCode: string;
  employeeNumber: number;
  employeeName: string;
  period: string;
  bonusTypeId: number;
  bonusTypeDescription: string;
  bonusValue: number;
  reasonCode: string;
  reasonDescription: string;
  stateCode: string;
  grossAmount: number;
  taxAmount: number;
  netAmount: number;
  createdAt: string;
}

export interface CreateBonusRequest {
  departmentCode: string;
  employeeNumber: number;
  period: string;
  bonusTypeId: number;
  bonusValue: number;
  reasonCode: string;
}

export interface BonusFilters {
  departmentCode?: string;
  employeeNumber?: number;
  period?: string;
  stateCode?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateBonusResponse {
  bonusId: number;
  departmentCode: string;
  employeeNumber: number;
  period: string;
  bonusTypeId: number;
  bonusValue: number;
  reasonCode: string;
  stateCode: string;
  grossAmount: number;
  taxAmount: number;
  netAmount: number;
}

export interface BonusOperationResponse {
  bonusId: number;
  stateCode: string;
  modifiedBy: string;
  modifiedAt: string;
  isDeleted?: boolean;
}
