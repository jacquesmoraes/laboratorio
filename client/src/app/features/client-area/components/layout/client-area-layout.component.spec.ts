import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClientAreaLayoutComponent } from './client-area-layout.component';

describe('ClientAreaLayoutComponent', () => {
  let component: ClientAreaLayoutComponent;
  let fixture: ComponentFixture<ClientAreaLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientAreaLayoutComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientAreaLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
