
import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';

import { MasterStateCodes } from '../../core/models/catalog';
import { Bonus, BonusFilters } from '../../core/models/bonus';
import { CatalogApi } from '../../core/services/catalog-api';
import { BonusApi } from '../../core/services/bonus-api';

@Component({
  selector: 'app-bonuses',
  imports: [DecimalPipe],
  templateUrl: './bonuses.html',
  styleUrl: './bonuses.css',
})
export class Bonuses {
  private readonly catalogApi = inject(CatalogApi);
  private readonly bonusApi = inject(BonusApi);

  // Catálogos existentes
  protected readonly departments = toSignal(
    this.catalogApi.getDepartments(),
    { initialValue: [] }
  );

  protected readonly bonusStates = toSignal(
    this.catalogApi.getStates(MasterStateCodes.Bonus),
    { initialValue: [] }
  );

  // Datos del listado
  protected readonly bonuses = signal<Bonus[]>([]);
  protected readonly totalCount = signal(0);
  protected readonly totalPages = computed(() =>
  Math.max(1, Math.ceil(this.totalCount() / this.pageSize()))
);
  protected readonly page = signal(1);
  protected readonly pageSize = signal(10);

  // Estado de la consulta
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');

  // Filtros
  protected readonly filters = signal<BonusFilters>({
    page: 1,
    pageSize: 10
  });

  constructor() {
    this.loadBonuses();
  }


protected onDepartmentChange(event: Event): void {
  const select = event.target as HTMLSelectElement;
  const departmentCode = select.value;

  this.filters.update(current => ({
    ...current,
    departmentCode: departmentCode || undefined,
    page: 1
  }));

  this.loadBonuses();
}


protected onStateChange(event: Event): void {
  const select = event.target as HTMLSelectElement;
  const stateCode = select.value;

  this.filters.update(current => ({
    ...current,
    stateCode: stateCode || undefined,
    page: 1
  }));

  this.loadBonuses();
}


protected nextPage(): void {
  const totalPages = Math.ceil(
    this.totalCount() / this.pageSize()
  );

  if (this.page() < totalPages) {
    this.filters.update(current => ({
      ...current,
      page: this.page() + 1
    }));

    this.loadBonuses();
  }
}

protected previousPage(): void {
  if (this.page() > 1) {
    this.filters.update(current => ({
      ...current,
      page: this.page() - 1
    }));

    this.loadBonuses();
  }
}


  // Consultar bonos desde el backend
  protected loadBonuses(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.bonusApi.getBonuses(this.filters()).subscribe({
      next: (response) => {
        this.bonuses.set(response.items);
        this.totalCount.set(response.totalCount);
        this.page.set(response.page);
        this.pageSize.set(response.pageSize);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set(
          'No se pudieron cargar los bonos.'
        );
        this.loading.set(false);
      }
    });
  }
}
