
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  Bonus,
  BonusFilters,
  BonusOperationResponse,
  CreateBonusRequest,
  CreateBonusResponse
} from '../models/bonus';
import { PagedResponse } from '../models/paged-response';

@Injectable({ providedIn: 'root' })
export class BonusApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/Bonuses`;

  // Consultar bonos con filtros y paginación
  getBonuses(filters: BonusFilters = {}): Observable<PagedResponse<Bonus>> {
    let params = new HttpParams();

    if (filters.departmentCode) {
      params = params.set('departmentCode', filters.departmentCode);
    }

    if (filters.employeeNumber != null) {
      params = params.set('employeeNumber', filters.employeeNumber);
    }

    if (filters.period) {
      params = params.set('period', filters.period);
    }

    if (filters.stateCode) {
      params = params.set('stateCode', filters.stateCode);
    }

    params = params.set('page', filters.page ?? 1);
    params = params.set('pageSize', filters.pageSize ?? 10);

    return this.http.get<PagedResponse<Bonus>>(
      this.baseUrl,
      { params }
    );
  }

  // Consultar bonos de un empleado
  getBonusesByEmployee(
    departmentCode: string,
    employeeNumber: number
  ): Observable<Bonus[]> {
    return this.http.get<Bonus[]>(
      `${this.baseUrl}/employee/${encodeURIComponent(departmentCode)}/${employeeNumber}`
    );
  }

  // Registrar un bono
  createBonus(
    request: CreateBonusRequest
  ): Observable<CreateBonusResponse> {
    return this.http.post<CreateBonusResponse>(
      this.baseUrl,
      request
    );
  }

  // Anular un bono
  cancelBonus(
    bonusId: number,
    modifiedBy: string
  ): Observable<BonusOperationResponse> {
    return this.http.put<BonusOperationResponse>(
      `${this.baseUrl}/${bonusId}/cancel`,
      { modifiedBy }
    );
  }

  // Eliminar lógicamente un bono
  deleteBonus(
    bonusId: number,
    modifiedBy: string
  ): Observable<BonusOperationResponse> {
    const params = new HttpParams().set('modifiedBy', modifiedBy);

    return this.http.delete<BonusOperationResponse>(
      `${this.baseUrl}/${bonusId}`,
      { params }
    );
  }
}
