import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { TablePriceFormComponent } from './table-price-form.component';
import { TablePriceService } from '../../services/table-price.services';

describe('TablePriceFormComponent', () => {
  let component: TablePriceFormComponent;
  let fixture: ComponentFixture<TablePriceFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        TablePriceFormComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [TablePriceService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TablePriceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
