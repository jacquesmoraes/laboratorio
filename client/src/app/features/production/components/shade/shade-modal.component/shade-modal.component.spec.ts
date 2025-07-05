import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShadeModalComponent } from './shade-modal.component';

describe('ShadeModalComponent', () => {
  let component: ShadeModalComponent;
  let fixture: ComponentFixture<ShadeModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShadeModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShadeModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
