import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ServiceOrdersService } from './service-order.service';
import { environment } from '../../../../environments/environment';
import { ServiceOrderParams, ServiceOrder, ServiceOrderDetails, OrderStatus } from '../models/service-order.interface';
import { provideHttpClient, withFetch } from '@angular/common/http';

describe('ServiceOrdersService', () => {
  let service: ServiceOrdersService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/serviceorders`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ServiceOrdersService,
        provideHttpClient(withFetch()),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(ServiceOrdersService);
    httpMock = TestBed.inject(HttpTestingController);
  });



  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getServiceOrders', () => {
    it('should return service orders with correct params', () => {
      const mockParams: ServiceOrderParams = {
        pageNumber: 1,
        pageSize: 10,
        sort: 'dateIn',
        search: 'test',
        excludeFinished: true,
        excludeInvoiced: false,
        startDate: '', 
  endDate: ''
      };
  
      const mockResponse = {
        data: [
          {
            billingInvoiceId: null,
            serviceOrderId: 1,
            orderNumber: 'OS001',
            dateIn: '2024-01-01',
            patientName: 'Test Patient',
            status: OrderStatus.Production,
            clientName: 'Test Client',
            clientId: 1,
            orderTotal: 100,
            currentSectorName: 'Setor A',
            lastMovementDate: '2024-01-01'
          }
        ],
        totalItems: 1,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1
      };
  
      service.getServiceOrders(mockParams).subscribe(response => {
        expect(response).toEqual(mockResponse);
      });
  
      const req = httpMock.expectOne(`${apiUrl}?pageNumber=1&pageSize=10&sort=dateIn&search=test&clientId=&excludeFinished=true&excludeInvoiced=false&status=`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });

    describe('getServiceOrderById', () => {
      it('should return service order details', () => {
        const mockOrderId = 1;
        const mockResponse: ServiceOrderDetails = {
          billingInvoiceId: null,
          serviceOrderId: 1,
          orderNumber: 'OS001',
          dateIn: '2024-01-01',
          dateOut: '2024-01-02',
          patientName: 'Test Patient',
          status: OrderStatus.Production,
          clientName: 'Test Client',
          clientId: 1,
          orderTotal: 100,
          client: {
            clientId: 1,
            clientName: 'Test Client',
            address: {
              street: 'Test Street',
              number: 123,
              complement: '',
              cep: '12345-678',
              neighborhood: 'Test Neighborhood',
              city: 'Test City'
            }
          },
          works: [],
          stages: []
        };
    
        service.getServiceOrderById(mockOrderId).subscribe(response => {
          expect(response).toEqual(mockResponse);
        });
    
        const req = httpMock.expectOne(`${apiUrl}/1`);
        expect(req.request.method).toBe('GET');
        req.flush(mockResponse);
      });
    });

    describe('createServiceOrder', () => {
      it('should create service order', () => {
        const mockDto = {
          clientId: 1,
          patientName: 'Test Patient',
          dateIn: '2024-01-01',
          firstSectorId: 1,
          works: []
        };
    
        const mockResponse: ServiceOrderDetails = {
          billingInvoiceId: null,
          serviceOrderId: 1,
          orderNumber: 'OS001',
          dateIn: '2024-01-01',
          dateOut: '2024-01-02',
          patientName: 'Test Patient',
          status: OrderStatus.Production,
          clientName: 'Test Client',
          clientId: 1,
          orderTotal: 100,
          client: {
            clientId: 1,
            clientName: 'Test Client',
            address: {
              street: 'Test Street',
              number: 123,
              complement: '',
              cep: '12345-678',
              neighborhood: 'Test Neighborhood',
              city: 'Test City'
            }
          },
          works: [],
          stages: []
        };
    
        service.createServiceOrder(mockDto).subscribe(response => {
          expect(response).toEqual(mockResponse);
        });
    
        const req = httpMock.expectOne(apiUrl);
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual(mockDto);
        req.flush(mockResponse);
      });
    });

  describe('error handling', () => {
    it('should handle HTTP errors gracefully', () => {
      const mockParams: ServiceOrderParams = {
        pageNumber: 1,
        pageSize: 10,
        sort: 'dateIn',
        search: 'test',
        excludeFinished: true,
        excludeInvoiced: false,
        startDate: '', 
  endDate: ''

      };

      service.getServiceOrders(mockParams).subscribe({
        next: () => fail('Should have failed'),
        error: (error) => {
          expect(error.status).toBe(500);
        }
      });

      const req = httpMock.expectOne(`${apiUrl}?pageNumber=1&pageSize=10&sort=dateIn&search=&clientId=&excludeFinished=false&excludeInvoiced=false&status=`);
      req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' });
    });
  });
});
});