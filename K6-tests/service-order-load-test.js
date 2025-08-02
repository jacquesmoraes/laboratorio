import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const BASE_URL = 'https://localhost:7058/api/serviceorders';
const HEADERS = {
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
};

const errorRate = new Rate('errors');

export const options = {
  stages: [
    { duration: '30s', target: 5 },
    { duration: '1m', target: 10 },
    { duration: '30s', target: 0 },
  ],
  thresholds: {
    http_req_duration: ['p(95)<3000'],
    http_req_failed: ['rate<0.05'],
    errors: ['rate<0.20'],
  },
};

function safeJsonParse(response) {
  try {
    return JSON.parse(response.body);
  } catch (_) {
    return null;
  }
}

function fail(groupName, res) {
  errorRate.add(1);
  console.log(`${groupName} check failed: ${res.status} ${res.timings.duration}`);
}

function getRandomQueryParams() {
  return {
    pageNumber: Math.floor(Math.random() * 3) + 1,
    pageSize: [10, 20][Math.floor(Math.random() * 2)],
    sort: ['DateIn', '-DateIn'][Math.floor(Math.random() * 2)],
    excludeFinished: Math.random() > 0.5,
    excludeInvoiced: Math.random() > 0.5,
  };
}

function buildQueryString(params) {
  return '?' + Object.entries(params)
    .filter(([, v]) => v !== undefined && v !== null && v !== '')
    .map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(v)}`)
    .join('&');
}

export default function () {
  group('Service Orders List', () => {
    const res = http.get(`${BASE_URL}${buildQueryString(getRandomQueryParams())}`, HEADERS);
    const data = safeJsonParse(res);

    const success = check(res, {
      'status 200': r => r.status === 200,
      'response < 1000ms': r => r.timings.duration < 1000,
      'has pagination': () => !!(data?.data && data.totalItems !== undefined),
      'has service orders': () => !!(data?.data?.length >= 0),
      'has optimized fields': () => typeof data?.pageNumber === 'number',
    });

    if (!success) fail('List', res);
  });

  sleep(1);

  group('Form Basic Data', () => {
    const res = http.get(`${BASE_URL}/form/basic-data`, HEADERS);
    const data = safeJsonParse(res);

    const success = check(res, {
      'status 200': r => r.status === 200,
      'response < 500ms': r => r.timings.duration < 500,
      'clients loaded': () => Array.isArray(data?.clients),
      'sectors loaded': () => Array.isArray(data?.sectors),
      'client shape ok': () => data?.clients?.[0]?.clientId !== undefined,
      'sector shape ok': () => data?.sectors?.[0]?.sectorId !== undefined,
    });

    if (!success) fail('Form Basic', res);
  });

  sleep(1);

  group('Works Data', () => {
    const res = http.get(`${BASE_URL}/form/works-data`, HEADERS);
    const data = safeJsonParse(res);

    const success = check(res, {
      'status 200': r => r.status === 200,
      'response < 1000ms': r => r.timings.duration < 1000,
      'workTypes loaded': () => Array.isArray(data?.workTypes),
      'scales loaded': () => Array.isArray(data?.scales),
      'shades loaded': () => Array.isArray(data?.shades),
      'workTypes shape ok': () => data?.workTypes?.[0]?.id !== undefined,
      'scales shape ok': () => data?.scales?.[0]?.id !== undefined,
      'shades shape ok': () => data?.shades?.[0]?.id !== undefined,
    });

    if (!success) fail('Works Data', res);
  });

  sleep(1);

  group('Service Order Details', () => {
    const orderIds = [1, 2, 3, 4, 5];
    const id = orderIds[Math.floor(Math.random() * orderIds.length)];
    const res = http.get(`${BASE_URL}/${id}`, HEADERS);
    const data = safeJsonParse(res);

    // 404 não conta como erro aqui
    if (res.status === 404) return;

    const success = check(res, {
      'status 200': r => r.status === 200,
      'response < 2000ms': r => r.timings.duration < 2000,
      'has serviceOrderId': () => data?.serviceOrderId !== undefined,
      'has works array': () => Array.isArray(data?.works),
      'has stages array': () => Array.isArray(data?.stages),
    });

    if (!success) fail('Details', res);
  });

  sleep(1);

  group('Try-In Alerts', () => {
    const days = Math.floor(Math.random() * 10) + 1;
    const res = http.get(`${BASE_URL}/alert/tryin?days=${days}`, HEADERS);
    const data = safeJsonParse(res);

    const success = check(res, {
      'status 200': r => r.status === 200,
      'response < 1000ms': r => r.timings.duration < 1000,
      'returns array': () => Array.isArray(data),
    });

    if (!success) fail('Alerts', res);
  });

  sleep(1);
}
