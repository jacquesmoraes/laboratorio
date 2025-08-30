import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ShadeListComponent } from './shade-list.component';
import { ShadeService } from '../../../services/shade.service';

describe('ShadeListComponent', () => {
  let component: ShadeListComponent;
  let fixture: ComponentFixture<ShadeListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ShadeListComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [ShadeService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShadeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
