export function parseVnd(value) {
    if (value === null || value === undefined || value === '') return null;
    const digits = String(value).replace(/\D/g, '');
    return digits ? Number(digits) : null;
}

export function formatVndNumber(value) {
    const amount = parseVnd(value);
    return amount === null ? '' : amount.toLocaleString('vi-VN');
}

export function formatVnd(value) {
    return `${formatVndNumber(value || 0)} đ`;
}
