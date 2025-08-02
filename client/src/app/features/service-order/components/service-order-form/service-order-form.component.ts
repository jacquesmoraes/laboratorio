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
  ServiceOrderDetails
} from '../../models/service-order.interface';
import { ServiceOrdersService } from '../../services/service-order.service';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { ScaleService } from '../../../production/services/scale.service';
import { ShadeService } from '../../../production/services/shade.service';
import { ClientService } from '../../../clients/services/clients.services';
import { SectorService } from '../../../sectors/service/sector.service';
import { WorkTypeService } from '../../../works/services/work-type.service';
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
import { ClientFormData, ScaleFormData, SectorFormData, ShadeFormData, WorkTypeFormData } from '../../../sectors/models/serviceOrderForm.interface';

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

  serviceOrderForm!: FormGroup;
  isEditMode = signal(false);
  loading = signal(false);
  serviceOrderId = signal<number | null>(null);
  shadesByWorkIndex = signal<Record<number, { shadeId: number; shadeColor: string }[]>>({});
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
    this.loadBasicData();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.serviceOrderId.set(Number(id));
      this.loadServiceOrder(Number(id));
    }
    this.setupFormListeners();
  }

  initializeForm() {
    this.serviceOrderForm = this.fb.group({
      clientId: ['', Validators.required],
      dateIn: ['', Validators.required],
      patientName: ['', [Validators.required, Validators.minLength(3)]],
      firstSectorId: ['', Validators.required],
      works: this.fb.array([])
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

  loadBasicData() {
    
    this.loading.set(true);

    this.serviceOrdersService.getBasicFormData().subscribe({
      next: (data) => {
        

        // ✅ SIMPLIFICADO: Usar objetos completos da API
        this.clients.set(data.clients);
        this.sectors.set(data.sectors);

        this.basicDataLoaded.set(true);
        this.loading.set(false);
        
      },
      error: (error) => {
        
        this.showError('Erro ao carregar dados básicos');
        this.loading.set(false);
      }
    });
  }

  loadServiceOrder(id: number) {
    this.loading.set(true);
    this.serviceOrdersService.getServiceOrderById(id).subscribe({
      next: order => {
        this.populateForm(order);
        this.loading.set(false);
      },
      error: () => this.showError('Erro ao carregar ordem de serviço')
    });
  }

  populateForm(order: ServiceOrderDetails) {
    this.serviceOrderForm.patchValue({
      clientId: order.client.clientId,
      dateIn: new Date(order.dateIn),
      patientName: order.patientName,
      firstSectorId: this.getSectorIdByName(order.currentSectorName) ?? ''
    });

    const worksArray = this.worksArray;
    worksArray.clear();

    order.works.forEach(work => {
      worksArray.push(
        this.createWorkFormGroup({
          workTypeId: work.workTypeId,
          quantity: work.quantity,
          priceUnit: work.priceUnit,
          shadeId: this.getShadeIdByName(work.shadeColor),
          scaleId: this.getScaleIdByName(work.scaleName),
          notes: work.notes || ''
        })
      );
    });
  }


  loadWorksDataIfNeeded() {
    if (this.worksDataLoaded() || this.loadingWorksData()) return;

    const clientId = this.serviceOrderForm.get('clientId')?.value;
    const sectorId = this.serviceOrderForm.get('firstSectorId')?.value;

    if (!clientId || !sectorId) return;

    this.loadingWorksData.set(true);

    this.serviceOrdersService.getWorksFormData().subscribe({
      next: (data) => {
        console.log('🔵 ServiceOrderFormComponent - getWorksFormData sucesso:', data);

        // ✅ SIMPLIFICADO: Usar objetos completos da API
        this.workTypes.set(data.workTypes);
        this.scales.set(data.scales);
        this.shades.set(data.shades);

        this.worksDataLoaded.set(true);
        this.loadingWorksData.set(false);
        console.log('✅ ServiceOrderFormComponent - dados de works carregados:', data);
      },
      error: (error) => {
        console.error('❌ ServiceOrderFormComponent - getWorksFormData erro:', error);
        this.showError('Erro ao carregar dados de works');
        this.loadingWorksData.set(false);
      }
    });
  }



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

  setupFormListeners() {
    // Listener para cliente
    this.serviceOrderForm.get('clientId')?.valueChanges.subscribe(() => {
      this.checkAndLoadWorksData();
    });

    // Listener para setor
    this.serviceOrderForm.get('firstSectorId')?.valueChanges.subscribe(() => {
      this.checkAndLoadWorksData();
    });
  }

  checkAndLoadWorksData() {
    const clientId = this.serviceOrderForm.get('clientId')?.value;
    const sectorId = this.serviceOrderForm.get('firstSectorId')?.value;

    if (clientId && sectorId && !this.worksDataLoaded()) {
      this.loadWorksDataIfNeeded();
    }
  }


  addWork() {
    this.worksArray.push(this.createWorkFormGroup());
  }

  removeWork(index: number) {
    this.worksArray.removeAt(index);
  }

  onSubmit() {
    this.worksArray.controls.forEach((workGroup, index) => {
    });
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
      works: formValue.works
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
            sectorName: s.name // ✅ CORRIGIDO: Usar s.name em vez de s.sectorName
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

          // Depois do modal, navegue para detalhes da OS
          this.router.navigate(['service-orders', result.serviceOrderId]);
        });

      },
      error: (error) => {
        this.showError('Erro ao salvar ordem de serviço');
      }
    });
  }

  markFormGroupTouched() {
    Object.values(this.serviceOrderForm.controls).forEach(control => {
      control.markAsTouched();
      if ((control as FormGroup | FormArray).controls) {
        Object.values((control as FormGroup | FormArray).controls).forEach(c =>
          c.markAsTouched()
        );
      }
    });
  }

  cancel() {
    this.router.navigate(['service-orders']);
  }
  getShadesForWork(index: number) {
    return this.shadesByWorkIndex()[index] || this.shades();
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

  isWorkTypeDisabled(workGroup: AbstractControl): boolean {
    return this.loading() || (workGroup as FormGroup).get('workTypeId')?.disabled || false;
  }

  trackByIndex(index: number): number {
    return index;
  }
  trackByShadeId(index: number, shade: { shadeId: number; shadeColor: string }): number {
    return shade.shadeId;
  }
  









  private getShadeIdByName(shadeColor?: string): number | undefined {
    return this.shades().find(s => s.color === shadeColor)?.id;
  }

  private getSectorIdByName(sectorName?: string): number | undefined {
    return this.sectors().find(s => s.name === sectorName)?.sectorId;
  }

  private getScaleIdByName(scaleName?: string): number | undefined {
    return this.scales().find(s => s.name === scaleName)?.id;
  }
  get worksArray() {
    return this.serviceOrderForm.get('works') as FormArray;
  }
  private showError(message: string) {
    this.loading.set(false);
    this.snackBar.open(message, 'Fechar', {
      duration: 3000,
      panelClass: ['error-snackbar']
    });
  }

  private filterShadesForWork(scaleId: number | null | undefined, index: number) {
    if (!scaleId) {
      const mappedShades = this.shades().map(shade => ({
        shadeId: shade.id,
        shadeColor: shade.color
      }));
      this.shadesByWorkIndex.update(state => ({ ...state, [index]: mappedShades }));
      return;
    }

    const filtered = this.shades()
      .filter(shade => shade.scaleId === scaleId)
      .map(shade => ({
        shadeId: shade.id,
        shadeColor: shade.color
      }));
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
      next: (result) => {
        if (result?.price != null) {
          workGroup.patchValue({ priceUnit: result.price });
        }
      }
      ,
      error: () => {
        console.log('Erro ao buscar preço');
      }
    });

  }
}
