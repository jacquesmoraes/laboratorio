import {
  Component,
  signal,
  inject,
  OnInit,
  ChangeDetectionStrategy
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormArray,
  Validators,
  ReactiveFormsModule,
  AbstractControl
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import {
  CreateServiceOrderDto,
  CreateWorkDto,
  RepeatResponsible
} from '../../models/service-order.interface';
import { ServiceOrdersService } from '../../services/service-order.service';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { TablePriceService } from '../../../table-price/services/table-price.services';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { ScheduleDeliveryModalComponent } from '../schedule-delivery-modal/schedule-delivery-modal.component';
import {
  ClientFormData,
  ScaleFormData,
  SectorFormData,
  ShadeFormData,
  WorkTypeFormData
} from '../../../sectors/models/serviceOrderForm.interface';
import { MatCheckboxModule } from '@angular/material/checkbox';


@Component({
  selector: 'app-service-order-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDividerModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './service-order-form.component.html',
  styleUrls: ['./service-order-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ServiceOrderFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private serviceOrdersService = inject(ServiceOrdersService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);
  private tablePriceService = inject(TablePriceService);
  RepeatResponsible = RepeatResponsible;
  serviceOrderForm!: FormGroup;
  isEditMode = signal(false);
  loading = signal(false);
  serviceOrderId = signal<number | null>(null);
  shadesByWorkIndex = signal<Record<number, ShadeFormData[]>>({});
  clients = signal<ClientFormData[]>([]);
  sectors = signal<SectorFormData[]>([]);
  workTypes = signal<WorkTypeFormData[]>([]);
  shades = signal<ShadeFormData[]>([]);
  scales = signal<ScaleFormData[]>([]);
  basicDataLoaded = signal(false);
  worksDataLoaded = signal(false);
  loadingWorksData = signal(false);

  ngOnInit() {
    this.initializeForm();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      // ======== MODO EDIÇÃO ========
      this.isEditMode.set(true);
      this.serviceOrderId.set(Number(id));
      this.loadEditMode(Number(id));
    } else {
      // ======== MODO CRIAÇÃO ========
      this.loadBasicData();
    }

    this.setupFormListeners();
  }

  // ======== FORM INITIALIZATION ========
  initializeForm() {
    this.serviceOrderForm = this.fb.group({
      clientId: ['', Validators.required],
      dateIn: ['', Validators.required],
      patientName: ['', [Validators.required, Validators.minLength(3)]],
      firstSectorId: ['', Validators.required],
      isRepeat: [false],
      repeatResponsible: [null],
      works: this.fb.array([])
    });
    this.serviceOrderForm.get('isRepeat')?.valueChanges.subscribe(isRepeat => {
      const rr = this.serviceOrderForm.get('repeatResponsible');
      if (isRepeat) {
        rr?.addValidators(Validators.required);
      } else {
        rr?.clearValidators();
        rr?.setValue(null);
      }
      rr?.updateValueAndValidity();
    });

    this.serviceOrderForm.get('clientId')?.valueChanges.subscribe(clientId => {
      this.worksArray.clear();
      if (!clientId) {
        this.snackBar.open('Selecione um cliente para adicionar trabalhos', 'Fechar', {
          duration: 3000,
          panelClass: ['warning-snackbar']
        });
      }
    });
  }

  // ======== BASIC DATA ========
  loadBasicData() {
    this.loading.set(true);
    this.serviceOrdersService.getBasicFormData().subscribe({
      next: data => {
        this.clients.set(data.clients);
        this.sectors.set(data.sectors);
        this.basicDataLoaded.set(true);
        this.loading.set(false);
      },
      error: () => {
        this.showError('Erro ao carregar dados básicos');
      }
    });
  }

  // ======== MODO EDIÇÃO ========
  private loadEditMode(id: number) {
    this.loading.set(true);

    // No modo edição, carregamos TUDO de uma vez
    this.serviceOrdersService.getBasicFormData().subscribe({
      next: basicData => {
        this.clients.set(basicData.clients);
        this.sectors.set(basicData.sectors);
        this.basicDataLoaded.set(true);

        // Carregar works imediatamente
        this.serviceOrdersService.getWorksFormData().subscribe({
          next: worksData => {
            this.workTypes.set(worksData.workTypes);
            this.scales.set(worksData.scales);
            this.shades.set(worksData.shades);
            this.worksDataLoaded.set(true);

            // Agora carregar a OS específica
            this.serviceOrdersService.getServiceOrderById(id).subscribe({
              next: order => {
                // Patch básicos

                this.serviceOrderForm.patchValue({
                  clientId: order.client.clientId,
                  dateIn: new Date(order.dateIn),
                  patientName: order.patientName,
                  firstSectorId: this.getSectorIdByName(order.currentSectorName) ?? '',
                  isRepeat: (order as any).isRepeat ?? false,
                  repeatResponsible: order.repeatResponsible
                    ? (order.repeatResponsible === 'Laboratory'
                      ? RepeatResponsible.Laboratory
                      : RepeatResponsible.Client)
                    : null
                });

                // Adicionar works existentes
                const worksArray = this.worksArray;
                worksArray.clear();

                order.works.forEach((work, i) => {
                  const group = this.createWorkFormGroup({
                    workTypeId: work.workTypeId,
                    quantity: work.quantity,
                    priceUnit: work.priceUnit,
                    shadeId: work.shadeId ?? undefined,
                    scaleId: work.scaleId ?? undefined,
                    notes: work.notes || ''
                  });
                  worksArray.push(group);
                  this.filterShadesForWork(work.scaleId ?? null, i);
                });

                this.loading.set(false);
              },
              error: () => {
                this.loading.set(false);
                this.showError('Erro ao carregar ordem de serviço');
              }
            });
          },
          error: () => {
            this.loading.set(false);
            this.showError('Erro ao carregar dados de works');
          }
        });
      },
      error: () => {
        this.loading.set(false);
        this.showError('Erro ao carregar dados básicos');
      }
    });
  }

  // ======== MODO CRIAÇÃO ========
  checkAndLoadWorksData() {
    if (this.isEditMode()) return; // no modo edição já carregamos no início

    const clientId = this.serviceOrderForm.get('clientId')?.value;
    const sectorId = this.serviceOrderForm.get('firstSectorId')?.value;

    if (clientId && sectorId && !this.worksDataLoaded()) {
      this.loadWorksDataIfNeeded();
    }
  }

  loadWorksDataIfNeeded() {
    if (this.worksDataLoaded() || this.loadingWorksData()) return;

    const clientId = this.serviceOrderForm.get('clientId')?.value;
    const sectorId = this.serviceOrderForm.get('firstSectorId')?.value;
    if (!clientId || !sectorId) return;

    this.loadingWorksData.set(true);
    this.serviceOrdersService.getWorksFormData().subscribe({
      next: data => {
        this.workTypes.set(data.workTypes);
        this.scales.set(data.scales);
        this.shades.set(data.shades);
        this.worksDataLoaded.set(true);
        this.loadingWorksData.set(false);
      },
      error: () => {
        this.showError('Erro ao carregar dados de works');
        this.loadingWorksData.set(false);
      }
    });
  }

  // ======== WORKS ========
  createWorkFormGroup(work?: Partial<CreateWorkDto>): FormGroup {
    const workGroup = this.fb.group({
      workTypeId: [work?.workTypeId || '', Validators.required],
      quantity: [work?.quantity || 1, [Validators.required, Validators.min(1)]],
      priceUnit: [work?.priceUnit ?? null, [Validators.min(0)]],
      shadeId: [work?.shadeId],
      scaleId: [work?.scaleId],
      notes: [work?.notes || '']
    });

    workGroup.get('workTypeId')?.valueChanges.subscribe(workTypeId => {
      if (workTypeId) {
        this.loadPriceForWorkType(workGroup, Number(workTypeId));
      }
    });

    workGroup.get('scaleId')?.valueChanges.subscribe(scaleId => {
      this.filterShadesForWork(scaleId, this.worksArray.controls.indexOf(workGroup));
    });

    return workGroup;
  }

  addWork() {
    this.worksArray.push(this.createWorkFormGroup());
  }

  removeWork(index: number) {
    this.worksArray.removeAt(index);
  }

  // ======== FORM SUBMIT ========
  onSubmit() {
    if (this.serviceOrderForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.loading.set(true);
    const formValue = this.serviceOrderForm.value;
    const serviceOrderData: CreateServiceOrderDto = {
      clientId: formValue.clientId,
      dateIn: (formValue.dateIn as Date).toISOString(),
      patientName: formValue.patientName,
      firstSectorId: formValue.firstSectorId,
      works: formValue.works,
      isRepeat: !!formValue.isRepeat,
      repeatResponsible: formValue.isRepeat ? formValue.repeatResponsible : null
    };

    const request = this.isEditMode()
      ? this.serviceOrdersService.updateServiceOrder(this.serviceOrderId()!, serviceOrderData)
      : this.serviceOrdersService.createServiceOrder(serviceOrderData);

    request.subscribe({
      next: result => {
        this.loading.set(false);
        this.snackBar.open(
          `Ordem de serviço ${this.isEditMode() ? 'atualizada' : 'criada'} com sucesso!`,
          'Fechar',
          { duration: 3000, panelClass: ['success-snackbar'] }
        );

        const dialogRef = this.dialog.open(ScheduleDeliveryModalComponent, {
          width: '400px',
          data: {
            serviceOrderId: result.serviceOrderId,
            sectors: this.sectors().map(s => ({
              sectorId: s.sectorId,
              sectorName: s.name
            }))
          }
        });

        dialogRef.afterClosed().subscribe(scheduleSuccess => {
          if (scheduleSuccess) {
            this.snackBar.open('Entrega agendada com sucesso!', 'Fechar', {
              duration: 3000,
              panelClass: ['success-snackbar']
            });
          }
          this.router.navigate(['/admin/service-orders', result.serviceOrderId]);
        });
      },
      error: () => {
        this.showError('Erro ao salvar ordem de serviço');
      }
    });
  }

  // ======== HELPERS ========
  get worksArray() {
    return this.serviceOrderForm.get('works') as FormArray;
  }

  private getSectorIdByName(sectorName?: string): number | undefined {
    return this.sectors().find(s => s.name === sectorName)?.sectorId;
  }

  private filterShadesForWork(scaleId: number | null | undefined, index: number) {
    const filtered = scaleId
      ? this.shades().filter(shade => shade.scaleId === scaleId)
      : this.shades();

    // Não precisa mais do mapeamento!
    this.shadesByWorkIndex.update(state => ({ ...state, [index]: filtered }));
  }

  private loadPriceForWorkType(workGroup: FormGroup, workTypeId: number) {
    const clientId = this.serviceOrderForm.get('clientId')?.value;
    if (!clientId) {
      this.snackBar.open('Selecione um cliente primeiro', 'Fechar', {
        duration: 3000,
        panelClass: ['warning-snackbar']
      });
      return;
    }

    this.tablePriceService.getPriceByClientAndWorkType(clientId, workTypeId).subscribe({
      next: result => {
        if (result?.price != null) {
          workGroup.patchValue({ priceUnit: result.price });
        }
      },
      error: () => console.log('Erro ao buscar preço')
    });
  }

  private markFormGroupTouched() {
    Object.values(this.serviceOrderForm.controls).forEach(control => {
      control.markAsTouched();
      if ((control as FormGroup | FormArray).controls) {
        Object.values((control as FormGroup | FormArray).controls).forEach(c =>
          c.markAsTouched()
        );
      }
    });
  }

  private showError(message: string) {
    this.loading.set(false);
    this.snackBar.open(message, 'Fechar', {
      duration: 3000,
      panelClass: ['error-snackbar']
    });
  }

  setupFormListeners() {
    this.serviceOrderForm.get('clientId')?.valueChanges.subscribe(() => {
      this.checkAndLoadWorksData();
    });

    this.serviceOrderForm.get('firstSectorId')?.valueChanges.subscribe(() => {
      this.checkAndLoadWorksData();
    });
  }

  getErrorMessage(controlName: string): string {
    const control = this.serviceOrderForm.get(controlName);
    if (control?.hasError('required')) return 'Este campo é obrigatório';
    if (control?.hasError('minlength')) {
      return `Mínimo de ${control.errors?.['minlength'].requiredLength} caracteres`;
    }
    if (control?.hasError('min')) {
      return `Valor mínimo é ${control.errors?.['min'].min}`;
    }
    return '';
  }

  getWorkErrorMessage(index: number, controlName: string): string {
    const control = (this.worksArray.at(index) as FormGroup).get(controlName);
    if (control?.hasError('required')) return 'Este campo é obrigatório';
    if (control?.hasError('min')) {
      return `Valor mínimo é ${control.errors?.['min'].min}`;
    }
    return '';
  }

  getShadesForWork(index: number) {
    const filteredShades = this.shadesByWorkIndex()[index];
    if (filteredShades) {
      return filteredShades;
    }
    return this.shades();
  }

  cancel() {
    this.router.navigate(['/admin/service-orders']);
  }

}
