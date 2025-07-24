import http from 'k6/http';
import { check, group, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 5 },
        { duration: '1m', target: 10 },
        { duration: '30s', target: 0 },
    ],
    thresholds: {
        http_req_duration: ['p(95)<2000'],
        http_req_failed: ['rate<0.1'],
    },
};

const BASE_URL = 'https://localhost:7058/api/client-area';
const HEADERS = {
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
    }
};

export default function () {
    group('ClientArea Order Details Only', () => {
        const orderIds = [23, 15, 16,  14, 19];
        const randomOrderId = orderIds[Math.floor(Math.random() * orderIds.length)];
        
        const res = http.get(`${BASE_URL}/orders/${randomOrderId}`, HEADERS);
        
        check(res, {
            'order details status 200': (r) => r.status === 200,
            'order details response time < 2000ms': (r) => r.timings.duration < 2000,
            'order details has service order data': (r) => {
                const data = JSON.parse(r.body);
                return !!(data && data.serviceOrderId !== undefined && data.orderNumber !== undefined);
            },
        });
    });

    sleep(1);
}