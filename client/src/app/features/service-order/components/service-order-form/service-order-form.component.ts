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

  private clientService = inject(ClientService);
  private sectorService = inject(SectorService);
  private workTypeService = inject(WorkTypeService);
  private scaleService = inject(ScaleService);
  private shadeService = inject(ShadeService);
  private tablePriceService = inject(TablePriceService);

  serviceOrderForm!: FormGroup;
  isEditMode = signal(false);
  loading = signal(false);
  serviceOrderId = signal<number | null>(null);
  shadesByWorkIndex = signal<Record<number, { shadeId: number; shadeColor: string }[]>>({});
  clients = signal<{ clientId: number; clientName: string }[]>([]);
  sectors = signal<{ sectorId: number; sectorName: string }[]>([]);
  workTypes = signal<{ workTypeId: number; workTypeName: string }[]>([]);
  shades = signal<{ shadeId: number; shadeColor: string; scaleId: number | null }[]>([]);

  scales = signal<{ scaleId: number; scaleName: string }[]>([]);

  ngOnInit() {
    this.initializeForm();
    this.loadFormData();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.serviceOrderId.set(Number(id));
      this.loadServiceOrder(Number(id));
    }
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

  loadFormData() {
    this.loading.set(true);

    this.clientService.getClients().subscribe({
      next: clients => this.clients.set(clients),
      error: () => this.showError('Erro ao carregar clientes')
    });

    this.sectorService.getAll().subscribe({
      next: sectors => this.sectors.set(
        sectors.map(s => ({ sectorId: s.id, sectorName: s.name }))
      ),
      error: () => this.showError('Erro ao carregar setores')
    });

    this.workTypeService.getAll().subscribe({
      next: workTypes => this.workTypes.set(
        workTypes.map(w => ({ workTypeId: w.id, workTypeName: w.name }))
      ),
      error: () => this.showError('Erro ao carregar tipos de trabalho')
    });

    this.scaleService.getAll().subscribe({
      next: scales => this.scales.set(
        scales.map(s => ({ scaleId: s.id, scaleName: s.name }))
      ),
      error: () => this.showError('Erro ao carregar escalas')
    });

    this.shadeService.getAll().subscribe({
      next: shades => this.shades.set(
        shades.map(s => ({ shadeId: s.id, shadeColor: s.color ?? '', scaleId: s.scaleId ?? null  }))
      ),
      error: () => this.showError('Erro ao carregar cores')
    });

    this.loading.set(false);
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

  private getShadeIdByName(shadeColor?: string): number | undefined {
    return this.shades().find(s => s.shadeColor === shadeColor)?.shadeId;
  }

  private getSectorIdByName(sectorName?: string): number | undefined {
    return this.sectors().find(s => s.sectorName === sectorName)?.sectorId;
  }

  private getScaleIdByName(scaleName?: string): number | undefined {
    return this.scales().find(s => s.scaleName === scaleName)?.scaleId;
  }

  get worksArray() {
    return this.serviceOrderForm.get('works') as FormArray;
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


  private filterShadesForWork(scaleId: number | null | undefined, index: number) {
  if (!scaleId) {
    this.shadesByWorkIndex.update(state => ({ ...state, [index]: this.shades() }));
    return;
  }

  const filtered = this.shades().filter(shade => shade.scaleId === scaleId);
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

  addWork() {
    this.worksArray.push(this.createWorkFormGroup());
  }

  removeWork(index: number) {
    this.worksArray.removeAt(index);
  }

  onSubmit() {
  console.log('=== INÍCIO DO SUBMIT ===');
  console.log('onSubmit() foi chamado');
  console.log('Form válido:', this.serviceOrderForm.valid);
  console.log('Form inválido:', this.serviceOrderForm.invalid);
  console.log('Loading state:', this.loading());
  console.log('Edit mode:', this.isEditMode());
  
  // Log detalhado do estado do formulário
  console.log('=== ESTADO DO FORMULÁRIO ===');
  console.log('Form completo:', this.serviceOrderForm);
  console.log('Form value:', this.serviceOrderForm.value);
  console.log('Form errors:', this.serviceOrderForm.errors);
  
  // Log dos controles individuais
  console.log('=== CONTROLES INDIVIDUAIS ===');
  console.log('clientId:', this.serviceOrderForm.get('clientId')?.value);
  console.log('clientId válido:', this.serviceOrderForm.get('clientId')?.valid);
  console.log('clientId errors:', this.serviceOrderForm.get('clientId')?.errors);
  
  console.log('dateIn:', this.serviceOrderForm.get('dateIn')?.value);
  console.log('dateIn válido:', this.serviceOrderForm.get('dateIn')?.valid);
  console.log('dateIn errors:', this.serviceOrderForm.get('dateIn')?.errors);
  
  console.log('patientName:', this.serviceOrderForm.get('patientName')?.value);
  console.log('patientName válido:', this.serviceOrderForm.get('patientName')?.valid);
  console.log('patientName errors:', this.serviceOrderForm.get('patientName')?.errors);
  
  console.log('firstSectorId:', this.serviceOrderForm.get('firstSectorId')?.value);
  console.log('firstSectorId válido:', this.serviceOrderForm.get('firstSectorId')?.valid);
  console.log('firstSectorId errors:', this.serviceOrderForm.get('firstSectorId')?.errors);
  
  // Log dos trabalhos
  console.log('=== TRABALHOS ===');
  console.log('Número de trabalhos:', this.worksArray.length);
  console.log('Works array:', this.worksArray.value);
  
  this.worksArray.controls.forEach((workGroup, index) => {
    console.log(`Trabalho ${index + 1}:`, workGroup.value);
    console.log(`Trabalho ${index + 1} válido:`, workGroup.valid);
    console.log(`Trabalho ${index + 1} errors:`, workGroup.errors);
  });
  
  if (this.serviceOrderForm.invalid) {
    console.log('=== FORMULÁRIO INVÁLIDO ===');
    console.log('Marcando todos os campos como touched');
    this.markFormGroupTouched();
    return;
  }

  console.log('=== FORMULÁRIO VÁLIDO - INICIANDO ENVIO ===');
  this.loading.set(true);

  const formValue = this.serviceOrderForm.value;
  console.log('=== DADOS DO FORMULÁRIO ===');
  console.log('Form value completo:', formValue);
  console.log('Data original:', formValue.dateIn);
  console.log('Tipo da data:', typeof formValue.dateIn);
  
  const serviceOrderData: CreateServiceOrderDto = {
    clientId: formValue.clientId,
    dateIn: (formValue.dateIn as Date).toISOString(),
    patientName: formValue.patientName,
    firstSectorId: formValue.firstSectorId,
    works: formValue.works
  };

  console.log('=== DADOS PARA ENVIAR ===');
  console.log('serviceOrderData:', serviceOrderData);
  console.log('Data convertida:', serviceOrderData.dateIn);
  console.log('Tipo da data convertida:', typeof serviceOrderData.dateIn);

  const request = this.isEditMode()
    ? this.serviceOrdersService.updateServiceOrder(this.serviceOrderId()!, serviceOrderData)
    : this.serviceOrdersService.createServiceOrder(serviceOrderData);

  console.log('=== INICIANDO REQUISIÇÃO ===');
  console.log('URL da requisição:', this.isEditMode() ? 'UPDATE' : 'CREATE');
  console.log('Service Order ID (se edit):', this.serviceOrderId());

  request.subscribe({
    next: result => {
      console.log('=== SUCESSO ===');
      console.log('Resultado recebido:', result);
      this.loading.set(false);
      this.snackBar.open(
        `Ordem de serviço ${this.isEditMode() ? 'atualizada' : 'criada'} com sucesso!`,
        'Fechar',
        { duration: 3000, panelClass: ['success-snackbar'] }
      );
      this.router.navigate(['service-orders', result.serviceOrderId]);
    },
    error: (error) => {
      console.log('=== ERRO ===');
      console.error('Erro completo:', error);
      console.error('Status do erro:', error.status);
      console.error('Mensagem do erro:', error.message);
      console.error('Response do erro:', error.error);
      this.showError('Erro ao salvar ordem de serviço');
    }
  });
  
  console.log('=== FIM DO SUBMIT ===');
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

  private showError(message: string) {
    this.loading.set(false);
    this.snackBar.open(message, 'Fechar', {
      duration: 3000,
      panelClass: ['error-snackbar']
    });
  }
}
