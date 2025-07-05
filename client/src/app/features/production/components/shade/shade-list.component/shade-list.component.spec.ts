import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShadeListComponent } from './shade-list.component';

describe('ShadeListComponent', () => {
  let component: ShadeListComponent;
  let fixture: ComponentFixture<ShadeListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShadeListComponent]
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
