import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { SectorListComponent } from './sector-list.component';
import { SectorService } from '../service/sector.service';


describe('SectorListComponent', () => {
  let component: SectorListComponent;
  let fixture: ComponentFixture<SectorListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        SectorListComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [SectorService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SectorListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
