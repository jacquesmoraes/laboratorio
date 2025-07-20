import http from 'k6/http';
import { check, group, sleep } from 'k6';

function safeJsonParse(response) {
    try {
        return JSON.parse(response.body);
    } catch {
        return null;
    }
}
export const options = {
    // Configurações de teste
    stages: [
        { duration: '30s', target: 5 },    // Rampa de subida
        { duration: '1m', target: 10 },    // Pico de carga
        { duration: '30s', target: 0 },    // Rampa de descida
    ],
    thresholds: {
        http_req_duration: ['p(95)<2000'], // 95% das requisições devem ser < 2s
        http_req_failed: ['rate<0.1'],     // Taxa de erro < 10%
    },
};

const BASE_URL = 'https://localhost:7058/api/client-area';

// Headers sem autenticação (sistema usa clientId hardcoded = 6)
const HEADERS = {
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
    }
};

// Função para construir query string manualmente (K6 não tem URLSearchParams)
function buildQueryString(params) {
    const queryParts = [];
    
    for (const [key, value] of Object.entries(params)) {
        if (value !== undefined && value !== null && value !== '') {
            queryParts.push(`${encodeURIComponent(key)}=${encodeURIComponent(value)}`);
        }
    }
    
    return queryParts.length > 0 ? '?' + queryParts.join('&') : '';
}

// Função para gerar parâmetros de query aleatórios
function getRandomQueryParams() {
    const pageSizes = [5, 10, 20];
    const sorts = ['dateIn', '-dateIn', 'patientName', '-patientName', 'status', '-status'];
    
    return {
        pageNumber: Math.floor(Math.random() * 3) + 1,
        pageSize: pageSizes[Math.floor(Math.random() * pageSizes.length)],
        sort: sorts[Math.floor(Math.random() * sorts.length)],
        search: Math.random() > 0.7 ? 'test' : '', // 30% das vezes inclui busca
    };
}

export default function () {
    // Grupo 1: Dashboard (endpoint mais crítico)
    group('ClientArea Dashboard', () => {
        const res = http.get(`${BASE_URL}/dashboard`, HEADERS);
        
        check(res, {
            'dashboard status 200': (r) => r.status === 200,
            'dashboard response time < 1000ms': (r) => r.timings.duration < 1000,
            'dashboard has client data': (r) => {
                const data = safeJsonParse(r);
                return !!(data && data.clientName !== undefined && data.totalInvoiced !== undefined);
            },
        });
        
    });

    sleep(0.5);

    // Grupo 2: Payments com diferentes parâmetros
    group('ClientArea Payments', () => {
        const params = getRandomQueryParams();
        const queryString = buildQueryString(params);
        
        const res = http.get(`${BASE_URL}/payments${queryString}`, HEADERS);
        
        check(res, {
            'payments status 200': (r) => r.status === 200,
            'payments response time < 1500ms': (r) => r.timings.duration < 1500,
            'payments has pagination': (r) => {
                const data = safeJsonParse(r);
                return !!(data && Array.isArray(data.data) && data.totalItems !== undefined);
            },
        });
        
        
    });

    sleep(0.5);

    // Grupo 3: Orders com filtros
    group('ClientArea Orders', () => {
        const params = getRandomQueryParams();
        // Adiciona filtros específicos para orders
        params.excludeFinished = Math.random() > 0.5;
        params.excludeInvoiced = Math.random() > 0.5;
        
        const queryString = buildQueryString(params);
        
        const res = http.get(`${BASE_URL}/orders${queryString}`, HEADERS);
        
        check(res, {
            'orders status 200': (r) => r.status === 200,
            'orders response time < 1500ms': (r) => r.timings.duration < 1500,
            'orders has service orders': (r) => {
                const data = safeJsonParse(r);
                return !!(data && Array.isArray(data.data) && data.totalItems !== undefined);
            },
        });
        
        
    });

    sleep(0.5);

    // Grupo 4: Invoices com filtros de data
    group('ClientArea Invoices', () => {
        const params = getRandomQueryParams();
        
        // Adiciona filtros de data (últimos 30 dias)
        const endDate = new Date();
        const startDate = new Date();
        startDate.setDate(startDate.getDate() - 30);
        
        params.startDate = startDate.toISOString().split('T')[0];
        params.endDate = endDate.toISOString().split('T')[0];
        
        const queryString = buildQueryString(params);
        
        const res = http.get(`${BASE_URL}/invoices${queryString}`, HEADERS);
        
        check(res, {
            'invoices status 200': (r) => r.status === 200,
            'invoices response time < 1500ms': (r) => r.timings.duration < 1500,
            'invoices has billing data': (r) => {
                const data = safeJsonParse(r);
                return !!(data && Array.isArray(data.data) && data.totalItems !== undefined);
            },
        });
        
        
    });

    sleep(0.5);

    // Grupo 5: Download de PDF (teste mais pesado)
    group('ClientArea Download Invoice PDF', () => {
        // ⚠️ IMPORTANTE: Substitua por IDs válidos de invoices que existem no banco para o clientId 6
        const invoiceIds = [1, 2, 3, 4, 5]; // IDs de exemplo - substitua pelos reais do clientId 6
        const randomInvoiceId = invoiceIds[Math.floor(Math.random() * invoiceIds.length)];
        
        const res = http.get(`${BASE_URL}/invoices/${randomInvoiceId}/download`, HEADERS);
        
        check(res, {
            'pdf status 200 or 403/404': (r) => [200, 403, 404].includes(r.status),
            'pdf response time < 3000ms': (r) => r.timings.duration < 3000,
            'pdf content type correct': (r) => {
                if (r.status === 200) {
                    return r.headers['Content-Type']?.includes('application/pdf');
                }
                return true;
            },
        });
        
    });

    // Pausa entre iterações para simular comportamento real
    sleep(1);
}

// Função de setup para verificar se a API está rodando
export function setup() {
    console.log('🔍 Verificando se a API está rodando...');
    
    const healthCheck = http.get(`${BASE_URL}/dashboard`, HEADERS);
    
    if (healthCheck.status === 200) {
        console.log('✅ API está rodando e respondendo');
        return { status: 'ready' };
    } else {
        console.log(`❌ API não está respondendo. Status: ${healthCheck.status}`);
        throw new Error('API não está disponível');
    }
}