import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { ServiceOrderListComponent } from './service-order-list.component';
import { ServiceOrderListService } from './services/service-order-list.service';
import { ServiceOrdersService } from '../../services/service-order.service';
import { signal, computed } from '@angular/core';
import { of } from 'rxjs';
import { OrderStatus } from '../../models/service-order.interface';

describe('ServiceOrderListComponent', () => {
  let component: ServiceOrderListComponent;
  let fixture: ComponentFixture<ServiceOrderListComponent>;
  let serviceOrderListService: jasmine.SpyObj<ServiceOrderListService>;
  let serviceOrdersService: jasmine.SpyObj<ServiceOrdersService>;

  beforeEach(async () => {
    const serviceOrderListServiceSpy = jasmine.createSpyObj('ServiceOrderListService', [
      'loadServiceOrders', 'updateParams', 'toggleSelection', 'destroy'
    ], {
      serviceOrders: signal([]),
      selectedOrderIds: signal([]),
      pagination: signal(null),
      loading: signal(false),
      sectors: signal([]),
      currentParams: signal({}),
      totalPages: computed(() => 1),
      totalItems: computed(() => 0),
      hasSelection: computed(() => false)
    });

    const serviceOrdersServiceSpy = jasmine.createSpyObj('ServiceOrdersService', [
      'getServiceOrders', 'getServiceOrderById', 'createServiceOrder'
    ]);

    await TestBed.configureTestingModule({
      imports: [
        ServiceOrderListComponent,
        NoopAnimationsModule,
        RouterTestingModule,
        HttpClientTestingModule,
        MatDialogModule,
        MatSnackBarModule
      ],
      providers: [
        { provide: ServiceOrderListService, useValue: serviceOrderListServiceSpy },
        { provide: ServiceOrdersService, useValue: serviceOrdersServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ServiceOrderListComponent);
    component = fixture.componentInstance;
    serviceOrderListService = TestBed.inject(ServiceOrderListService) as jasmine.SpyObj<ServiceOrderListService>;
    serviceOrdersService = TestBed.inject(ServiceOrdersService) as jasmine.SpyObj<ServiceOrdersService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load service orders on init', () => {
    serviceOrderListService.loadServiceOrders.and.returnValue(of({
      pageNumber: 1,
      pageSize: 10,
      totalItems: 0,
      totalPages: 1,
      data: []
    }));
    component.ngOnInit();
    
    expect(serviceOrderListService.loadServiceOrders).toHaveBeenCalled();
  });

  it('should handle filter changes', () => {
    const filterEvent = {
      search: 'test',
      status: OrderStatus.Production,
      sortBy: 'dateIn',
      showFinishedOrders: false
    };

    serviceOrderListService.updateParams.and.returnValue();
    serviceOrderListService.loadServiceOrders.and.returnValue(of({
      pageNumber: 1,
      pageSize: 10,
      totalItems: 0,
      totalPages: 1,
      data: []
    }));

    component.onFilterChange(filterEvent);

    expect(serviceOrderListService.updateParams).toHaveBeenCalledWith({
      search: 'test',
      status: OrderStatus.Production,
      sort: 'dateIn',
      excludeFinished: true,
      pageNumber: 1
    });
    expect(serviceOrderListService.loadServiceOrders).toHaveBeenCalled();
  });

  it('should handle selection changes', () => {
    const orderId = 1;
    
    component.onSelectionChange(orderId);
    
    expect(serviceOrderListService.toggleSelection).toHaveBeenCalledWith(orderId);
  });

  it('should handle page changes', () => {
    const pageEvent = { pageIndex: 1, pageSize: 20, length: 100 };
    
    serviceOrderListService.updateParams.and.returnValue();
    serviceOrderListService.loadServiceOrders.and.returnValue(of({
      pageNumber: 1,
      pageSize: 10,
      totalItems: 0,
      totalPages: 1,
      data: []
    }));

    component.onPageChange(pageEvent);

    expect(serviceOrderListService.updateParams).toHaveBeenCalledWith({
      pageNumber: 2,
      pageSize: 20
    });
    expect(serviceOrderListService.loadServiceOrders).toHaveBeenCalled();
  });

  it('should destroy service on component destroy', () => {
    component.ngOnDestroy();
    
    expect(serviceOrderListService.destroy).toHaveBeenCalled();
  });
});