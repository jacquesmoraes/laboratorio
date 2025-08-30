import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ClientFormComponent } from './client-form.component';
import { ClientService } from '../../services/clients.service';
import { TablePriceService } from '../../../table-price/services/table-price.services';
import { CepService } from '../../../../core/services/cep.service';
import { ErrorService } from '../../../../core/services/error.service';
import { ErrorMappingService } from '../../../../core/services/error.mapping.service';

describe('ClientFormComponent', () => {
  let component: ClientFormComponent;
  let fixture: ComponentFixture<ClientFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ClientFormComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [
        ClientService,
        TablePriceService,
        CepService,
        ErrorService,
        ErrorMappingService
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
