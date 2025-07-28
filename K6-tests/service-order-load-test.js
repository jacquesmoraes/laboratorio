import http from 'k6/http';
import { check,group, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const BASE_URL = 'https://localhost:7058/api/serviceorders';
const HEADERS = {
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
    }
};

// Métricas customizadas
const errorRate = new Rate('errors');

export const options = {
    stages: [
        { duration: '30s', target: 5 },    // Rampa de subida
        { duration: '1m', target: 15 },    // Pico de carga
        { duration: '30s', target: 0 },    // Rampa de descida
    ],
    thresholds: {
        http_req_duration: ['p(95)<1500'], // 95% das requisições devem ser < 1.5s
        http_req_failed: ['rate<0.05'],    // Taxa de erro < 5%
        'errors': ['rate<0.05'],
    },
};

function safeJsonParse(response) {
    try {
        return JSON.parse(response.body);
    } catch (e) {
        return null;
    }
}

function getRandomQueryParams() {
    return {
        pageNumber: Math.floor(Math.random() * 5) + 1,
        pageSize: [10, 20, 50][Math.floor(Math.random() * 3)],
        sort: ['dateIn', '-dateIn', 'patientName'][Math.floor(Math.random() * 3)],
        search: Math.random() > 0.7 ? 'test' : '',
        excludeFinished: Math.random() > 0.5,
        excludeInvoiced: Math.random() > 0.5,
        status: Math.random() > 0.8 ? 'Production' : undefined
    };
}

function buildQueryString(params) {
    const queryParts = [];
    
    for (const [key, value] of Object.entries(params)) {
        if (value !== undefined && value !== null && value !== '') {
            queryParts.push(`${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
        }
    }
    
    return queryParts.length > 0 ? '?' + queryParts.join('&') : '';
}

export default function () {
    // Grupo 1: Listagem de Service Orders (endpoint mais crítico)
    group('Service Orders List', () => {
        const params = getRandomQueryParams();
        const queryString = buildQueryString(params);
        
        const res = http.get(`${BASE_URL}${queryString}`, HEADERS);
        
        const success = check(res, {
            'list status 200': (r) => r.status === 200,
            'list response time < 1000ms': (r) => r.timings.duration < 1000,
            'list has pagination': (r) => {
                const data = safeJsonParse(r);
                return !!(data && Array.isArray(data.data) && data.totalItems !== undefined);
            },
            'list has service orders': (r) => {
                const data = safeJsonParse(r);
                return !!(data && data.data && data.data.length > 0);
            },
        });
        
        if (!success) {
            errorRate.add(1);
        }
    });

    sleep(0.5);

    // Grupo 2: Detalhes de Service Order
    group('Service Order Details', () => {
        // ⚠️ IMPORTANTE: Substitua por IDs válidos que existem no banco
        const orderIds = [1, 2, 3, 4, 5]; // IDs de exemplo - substitua pelos reais
        const randomOrderId = orderIds[Math.floor(Math.random() * orderIds.length)];
        
        const res = http.get(`${BASE_URL}/${randomOrderId}`, HEADERS);
        
        const success = check(res, {
            'details status 200': (r) => r.status === 200,
            'details response time < 1500ms': (r) => r.timings.duration < 1500,
            'details has service order data': (r) => {
                const data = safeJsonParse(r);
                return !!(data && data.serviceOrderId !== undefined && data.orderNumber !== undefined);
            },
            'details has works array': (r) => {
                const data = safeJsonParse(r);
                return !!(data && Array.isArray(data.works));
            },
            'details has stages array': (r) => {
                const data = safeJsonParse(r);
                return !!(data && Array.isArray(data.stages));
            },
        });
        
        if (!success) {
            errorRate.add(1);
        }
    });

    sleep(0.5);

    // Grupo 3: Alertas de Try-In
    group('Try-In Alerts', () => {
        const days = Math.floor(Math.random() * 10) + 1;
        const res = http.get(`${BASE_URL}/alert/tryin?days=${days}`, HEADERS);
        
        const success = check(res, {
            'alerts status 200': (r) => r.status === 200,
            'alerts response time < 800ms': (r) => r.timings.duration < 800,
            'alerts returns array': (r) => {
                const data = safeJsonParse(r);
                return Array.isArray(data);
            },
        });
        
        if (!success) {
            errorRate.add(1);
        }
    });

    sleep(1);
}
